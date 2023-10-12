namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents a Header Record.
	/// </summary>
	public class HeaderRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the record ID. This must be the single letter 'H' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the delimiters used in the ASCII message format.
		/// The delimiters field contents must conform to LIS2 with no additional restrictions.
		/// </summary>
		public string Delimiters { get; set; }

		/// <summary>
		/// Gets or sets the sender ID which consists of Analyzer Serial Number, 
		/// CELL-DYN Ruby Model, CELL-DYN Ruby Software Version ID, and CELL-DYN Ruby Host Computer I/F Version ID.
		/// </summary>
		public SenderID Sender { get; set; }

		/// <summary>
		/// Gets or sets the processing ID. This must be the single letter 'P' in either upper or lower case.
		/// </summary>
		public string ProcessingID { get; set; }

		/// <summary>
		/// Gets or sets the CLSI version number, indicating the version of the standard/format being used.
		/// This field must be the string "LIS2-A".
		/// </summary>
		public string CLSIVersionNo { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a HeaderRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Header Record.</param>
		/// <returns>The HeaderRecord entity parsed from the provided string.</returns>
		public static HeaderRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			var record = new HeaderRecord
			{
				RecordID = fields[0],
				Delimiters = fields[1],
				ProcessingID = fields[11],
				CLSIVersionNo = fields[12]
			};

			if (string.IsNullOrEmpty(fields[4]) == false)
			{
				record.Sender = new SenderID
				{
					AnalyzerSerialNumber = fields[4].Split('^')[0],
					Model = fields[4].Split('^')[1],
					SoftwareVersionID = fields[4].Split('^')[2],
					HostComputerIFVersionID = fields[4].Split('^')[3]
				};
			}

			return record;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordID}|{Delimiters}|||{Sender.AnalyzerSerialNumber}^{Sender.Model}^{Sender.SoftwareVersionID}^{Sender.HostComputerIFVersionID}|||||||{ProcessingID}|{CLSIVersionNo}";
		}
	}

	/// <summary>
	/// Represents the sender details for the Header Record.
	/// </summary>
	public class SenderID
	{
		/// <summary>
		/// Gets or sets the Analyzer Serial Number. 
		/// This should be an ASCII character string of at least 1 and at most 9 characters 
		/// in the ASCII range 32-255, inclusive. It uniquely identifies the CELL-DYN Ruby (Analyzer) Instrument.
		/// </summary>
		public string AnalyzerSerialNumber { get; set; }

		/// <summary>
		/// Gets or sets the CELL-DYN Ruby Model. This must be the ASCII character string "CDRuby".
		/// </summary>
		public string Model { get; set; }

		/// <summary>
		/// Gets or sets the CELL-DYN Ruby Software Version ID.
		/// This should be an ASCII character string of at least 1 and at most 16 characters 
		/// in the ASCII range 32-255, inclusive. It uniquely identifies the version of the 
		/// CELL-DYN Ruby application software.
		/// </summary>
		public string SoftwareVersionID { get; set; }

		/// <summary>
		/// Gets or sets the CELL-DYN Ruby Host Computer I/F Version ID.
		/// This must be the 3-character ASCII character string "1.0".
		/// </summary>
		public string HostComputerIFVersionID { get; set; }
	}
}