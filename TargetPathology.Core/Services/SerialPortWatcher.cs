using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO.Ports;
using System.Text;
using TargetPathology.Core.Communications;
using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Services
{
	/// <summary>
	/// Monitors available serial ports in the system and manages a collection of those devices.
	/// </summary>
	public class SerialPortWatcher : BackgroundService
	{
		private readonly ISerialPortManager _serialPortManager;
		private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(5);
		private readonly ILogger<SerialPortWatcher> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPortWatcher"/> class.
		/// </summary>
		/// <param name="serialPortManager">The manager responsible for the lifecycle of serial ports.</param>
		public SerialPortWatcher(ISerialPortManager serialPortManager, ILogger<SerialPortWatcher> logger)
		{
			_serialPortManager = serialPortManager;
			_logger = logger;

			// check if in debug mode
			#if DEBUG
			var simulatedPort = new SimulationSerialPort
			{
				PortName = "SIMCOM1"
			};

			_serialPortManager.AddOrUpdatePort(simulatedPort);
			#endif
		}

#if DEBUG
		private bool _isWritingToSimulatedPort = false;

		private async Task WriteDataToSimulatedPort(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested) return;

			if (_serialPortManager.ActivePort is not SimulationSerialPort port) return;

			// give manager time to connect
			await Task.Delay(2000, cancellationToken);

			// write enquiry bytes
			port.SetSimulatedInputData("\u0005");
			await Task.Delay(500, cancellationToken);

			var buffer = new StringBuilder();

			// write header record
			var headerRecord = HeaderRecord.FromString("1H|\\^&|||  71114BG^CDRuby^2.3 ML^1.0|||||||P|LIS2-A\r\nP|1|||||||U|||||||^");
			buffer.Append(headerRecord + Environment.NewLine);

			// write patient record
			var patientRecord = PatientInformationRecord.FromString("P|1|||||||U|||||||^");
			buffer.Append(patientRecord + Environment.NewLine);

			// write test order record
			var testOrderRecord = TestOrderRecord.FromString("O|1|26804103|8958|^^^CBC^1^1|||||||||||Patient^||||||||||F");
			buffer.Append(testOrderRecord + Environment.NewLine);


			var resultsRecords = new List<ResultsRecord>
			{
				ResultsRecord.FromString("R|1|^^^CBC^^^WBC|11.71|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|2|^^^CBC^^^NEU|8.648|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|3|^^^CBC^^^LYM|2.009|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|4|^^^CBC^^^MONO|.0118|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|5|^^^CBC^^^EOS|.4841|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|6|^^^CBC^^^BASO|.5580|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|7|^^^CBC^^^RBC|.9298|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|8|^^^CBC^^^HGB|17.92|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|9|^^^CBC^^^HCT|6.255|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|10|^^^CBC^^^MCV|67.27|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|11|^^^CBC^^^MCH|192.7|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|12|^^^CBC^^^MCHC|286.5|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|13|^^^CBC^^^RDW|24.85|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|14|^^^CBC^^^PLT|>>>>>|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|15|^^^CBC^^^MPV|14.69|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|16|^^^CBC^^^PCT|-----|||||X||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|17|^^^CBC^^^PDW|21.99|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|18|^^^CBC^^^%N|73.85|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|19|^^^CBC^^^%L|17.15|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|20|^^^CBC^^^%M|.1005|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|21|^^^CBC^^^%E|4.134|||||W||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|22|^^^CBC^^^%B|4.765|||||F||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|23|^^^CBC^^^DFLT(N)|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|24|^^^CBC^^^DFLT(E)|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|25|^^^CBC^^^DFLT(L)|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|26|^^^CBC^^^IG|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|27|^^^CBC^^^BAND|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|28|^^^CBC^^^DFLT(M)|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|29|^^^CBC^^^WBC|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|30|^^^CBC^^^NRBC|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|31|^^^CBC^^^RRBC|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|32|^^^CBC^^^RBC MORPH|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|33|^^^CBC^^^LRI|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|34|^^^CBC^^^URI|FLAG|||||||Guest|20231005081747||  71114BG"),
				ResultsRecord.FromString("R|35|^^^CBC^^^MCHC|FLAG|||||||Guest|20231005081747||  71114BG")
			};

			foreach (var resultsRecord in resultsRecords)
			{
				// write results record
				buffer.Append(resultsRecord + Environment.NewLine);
			}

			// write message terminator record
			var messageTerminatorRecord = MessageTerminatorRecord.FromString("L|1|N");
			buffer.Append(messageTerminatorRecord + Environment.NewLine);

			port.SetSimulatedInputData(buffer.ToString());
			await Task.Delay(1000, cancellationToken);

			// write end of transmission bytes
			port.SetSimulatedInputData("\u0004");
			await Task.Delay(30000, cancellationToken);
		}
#endif

		/// <summary>
		/// Executes the background service task. This method runs the loop responsible for 
		/// monitoring and managing the available serial ports.
		/// </summary>
		/// <param name="stoppingToken">A cancellation token that can be used to request the background service to stop.</param>
		/// <returns>A task that represents the background service's execution.</returns>
		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (stoppingToken.IsCancellationRequested == false)
			{
				// check if in debug mode
#if DEBUG
				if (_isWritingToSimulatedPort == false)
				{
					var activePort = _serialPortManager.ActivePort;

					if (activePort is SimulationSerialPort simulationSerialPort)
					{
						_isWritingToSimulatedPort = true;
						_ = Task.Run(async () =>
						{
							try
							{
								await WriteDataToSimulatedPort(stoppingToken);
							}
							finally
							{
								_isWritingToSimulatedPort = false;
							}
						}, stoppingToken);
					}
				}
#endif

				try
				{
					var availablePortNames = SerialPort.GetPortNames().ToList();

					// detect newly connected serial devices
					foreach (var portName in availablePortNames.Except(_serialPortManager.GetAllPortNames()))
					{
						try
						{
							var serialPort = new StandardSerialPort(portName);
							_serialPortManager.AddOrUpdatePort(serialPort);
						}
						catch (Exception ex)
						{
							_logger.LogError($"Error processing newly connected device on port {portName}: {ex.Message}");
						}
					}

					// detect disconnected serial devices and dispose of them
					foreach (var portName in _serialPortManager.GetAllPortNames().Except(availablePortNames))
					{
						try
						{
							// check if in debug mode
#if DEBUG
							if (portName == "SIMCOM1")
								continue;
#endif

							if (_serialPortManager.TryGetPort(portName, out var serialPort))
							{
								serialPort.Dispose();
								_serialPortManager.RemovePort(portName);
							}
						}
						catch (Exception ex)
						{
							_logger.LogError($"Error processing disconnected device on port {portName}: {ex.Message}");
						}
					}
				}
				catch (Exception ex)
				{
					_logger.LogError($"Error during serial port detection cycle: {ex.Message}");
				}

				await Task.Delay(_checkInterval, stoppingToken);
			}
		}
	}
}