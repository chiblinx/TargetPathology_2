namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents a Results Record.
	/// </summary>
	public class ResultsRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the Record Type. This should be the single character 'R' in either upper or lower case.
		/// </summary>
		public string RecordType { get; set; }

		/// <summary>
		/// Gets or sets the RR Sequence Number.
		/// </summary>
		public int RRSeqNum { get; set; }

		/// <summary>
		/// Gets or sets the Universal Test ID.
		/// </summary>
		public UniversalTestId UniversalTestId { get; set; }

		/// <summary>
		/// Gets or sets the Data or Measurement Value.
		/// </summary>
		public string DataOrMeasurementValue { get; set; }

		/// <summary>
		/// Gets or sets the Result Status.
		/// </summary>
		public char? ResultStatus { get; set; }

		/// <summary>
		/// Gets or sets the Operator ID.
		/// </summary>
		public string OperatorID { get; set; }

		/// <summary>
		/// Gets or sets the Date and Time Test Started.
		/// </summary>
		public string DateTimeTestStarted { get; set; }

		/// <summary>
		/// Gets or sets the Instrument Identification.
		/// </summary>
		public string InstrumentIdentification { get; set; }

		public static ResultsRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			var record = new ResultsRecord
			{
				RecordType = fields[0],
				RRSeqNum = int.Parse(fields[1]),
				DataOrMeasurementValue = fields[3],
				ResultStatus = string.IsNullOrEmpty(fields[8]) ? null : (char?)fields[8][0],
				OperatorID = fields[9],
				DateTimeTestStarted = fields[11],
				InstrumentIdentification = fields[13]
			};

			if (string.IsNullOrEmpty(fields[2]) == false)
				record.UniversalTestId = UniversalTestId.FromString(fields[2]);

			return record;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordType}|{RRSeqNum}|{UniversalTestId}|{DataOrMeasurementValue}|||||{ResultStatus?.ToString() ?? ""}|{OperatorID}||{DateTimeTestStarted}||{InstrumentIdentification}";
		}
	}
}