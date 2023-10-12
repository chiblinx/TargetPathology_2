using Microsoft.Extensions.Logging;
using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Messaging
{
	public static class MessageConversionHelpers
	{
		public static List<IRecord> ConvertToMessage(string message, ILogger? logger = null)
		{
			var records = new List<IRecord>();
			var messageLines = message.Split(Environment.NewLine);

			foreach (var messageLine in messageLines)
			{
				// Don't process empty lines
				if (string.IsNullOrWhiteSpace(messageLine)) continue;

				try
				{
					var record = GetRecord(messageLine, logger);

					if (record != null)
						records.Add(record);
				}
				catch (Exception ex)
				{
					logger?.LogError(ex, $"Error converting message: {messageLine}");
				}
			}

			return records;
		}

		public static string? GetRecordType(string messageLine, char delimiter = '|', ILogger? logger = null)
		{
			try
			{
				var split = messageLine.Split(delimiter);

				if (split.Length == 0 || split[0].Length == 0)
					return null;

				var recordType = split[0].Length == 1 ? split[0] : split[0].Substring(1);

				return recordType;
			}
			catch (Exception ex)
			{
				logger?.LogError(ex, $"Error getting record type for message: {messageLine}");
				return null;
			}
		}

		public static IRecord? GetRecord(string messageLine, ILogger? logger = null)
		{
			var recordType = GetRecordType(messageLine, logger: logger);

			switch (recordType)
			{
				case "1":
				case "H":
					return HeaderRecord.FromString(messageLine);
				case "O":
					return TestOrderRecord.FromString(messageLine);
				case "P":
					return PatientInformationRecord.FromString(messageLine);
				case "Q":
					return RequestInformationRecord.FromString(messageLine);
				case "L":
					return MessageTerminatorRecord.FromString(messageLine);
				case "C":
					return CommentRecord.FromString(messageLine);
				case "R":
					return ResultsRecord.FromString(messageLine);
				default:
					return null;
			}
		}
	}
}
