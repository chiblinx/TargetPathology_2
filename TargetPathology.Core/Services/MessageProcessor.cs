using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using TargetPathology.Core.Messaging;
using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Services
{
	/// <summary>
	/// Provides a service for processing messages received over a serial port.
	/// </summary>
	public class MessageProcessor
	{
		private readonly ILogger<MessageProcessor> _logger;
		private readonly IConfiguration _configuration;
		private readonly StatisticsTracker _statisticsTracker;

		private readonly string _clientName = "Ruby_01";

		/// <summary>
		/// Initializes a new instance of the <see cref="MessageProcessor"/> class.
		/// </summary>
		public MessageProcessor(ILogger<MessageProcessor> logger, IConfiguration configuration, 
			StatisticsTracker statisticsTracker)
		{
			_logger = logger;
			_configuration = configuration;
			_statisticsTracker = statisticsTracker;

			_clientName = _configuration["MessageProcessor:ClientName"] ?? "Ruby_01";
		}

		/// <summary>
		/// Asynchronously processes the message.
		/// </summary>
		/// <param name="buffer">The message buffer.</param>
		/// <returns>A task representing the asynchronous operation.</returns>
		public async Task ProcessMessageAsync(string buffer)
		{
			var records = MessageConversionHelpers.ConvertToMessage(buffer, _logger);
			_statisticsTracker.IncrementRecordsRead(records.Count);

			var currentSpecimenId = "";

			foreach (var record in records)
			{
				try
				{
					switch (record)
					{
						case TestOrderRecord testOrderRecord:
							_statisticsTracker.IncrementTestOrderRecordsReceived();
							currentSpecimenId = testOrderRecord.SpecimenID;
							break;
						case ResultsRecord resultsRecord:
						{
							_statisticsTracker.IncrementResultsRecordsReceived();

							if (string.IsNullOrEmpty(currentSpecimenId))
							{
								_logger.LogWarning($"Attempting to process a results record, but no specimen has been identified yet." +
								                   $"{resultsRecord}");
								continue;
							}

							// parse the test's time completed
							var timeCompleted = DateTime.MinValue;

							try
							{
								timeCompleted = DateTime.ParseExact(resultsRecord.DateTimeTestStarted,
									resultsRecord.DateTimeTestStarted.Contains('-') == false ? "yyyyMMddHHmmss" : "yyyy-MM-dd HH:mm:ss",
									null);
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, "Error converting test result date time.");
							}

							var connectionString = _configuration.GetConnectionString("DefaultConnection");

							var wksUpdateQuery = "UPDATE wks SET " +
							                     "result = @result, " +
							                     "status = @status, " +
							                     "res_time = @res_time, " +
							                     "res_date = @res_date, " +
							                     "machine_name = @machine_name " +
							                     "WHERE lab_num = @lab_num AND " +
							                     "machine_assay_num = @machine_assay_num";

							await using var connection = new MySqlConnection(connectionString);
							await using var command = new MySqlCommand(wksUpdateQuery, connection);

							try
							{
								command.Parameters.AddWithValue("@result", resultsRecord.DataOrMeasurementValue);
								command.Parameters.AddWithValue("@status", "RESULTED");
								command.Parameters.AddWithValue("@res_time", timeCompleted.ToString("hh:mm:ss"));
								command.Parameters.AddWithValue("@res_date", timeCompleted.ToString("yyyy/MM/dd"));
								command.Parameters.AddWithValue("@machine_name", _clientName);
								command.Parameters.AddWithValue("@lab_num", currentSpecimenId);
								command.Parameters.AddWithValue("@machine_assay_num", resultsRecord.UniversalTestId.ResultLabel);

								await connection.OpenAsync();
								var recordsUpdated = await command.ExecuteNonQueryAsync();
								await connection.CloseAsync();

								_statisticsTracker.IncrementRecordsWritten();
							}
							catch (Exception ex)
							{
								_logger.LogError(ex, $"Error writing to database for test type {resultsRecord.UniversalTestId.ResultLabel}.");
								_statisticsTracker.IncrementDatabaseErrors();
							}

							break;
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Error processing record. {record}");
				}

				_statisticsTracker.IncrementRecordsProcessed();
			}
		}
	}
}