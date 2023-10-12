namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents the Test Order Record (TOR) of an Orders List Entry Request.
	/// </summary>
	public class TestOrderRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the Record ID. This must be the single letter 'O' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the TOR Sequence Number. This must be the single digit 1.
		/// </summary>
		public int TORSeqNumber { get; set; }

		/// <summary>
		/// Gets or sets the Specimen ID.
		/// </summary>
		public string SpecimenID { get; set; }

		/// <summary>
		/// Gets or sets the Universal Test ID.
		/// </summary>
		public UniversalTestId UniversalTestId { get; set; }

		/// <summary>
		/// Gets or sets the Specimen Collection Date & Time in the format YYYYMMDDHHMMSS.
		/// </summary>
		public string CollectionDateTime { get; set; }

		/// <summary>
		/// Gets or sets the SpecimenType.
		/// </summary>
		public string SpecimenType { get; set; }

		/// <summary>
		/// Gets or sets the Specimen Subtype.
		/// </summary>
		public string SpecimenSubtype { get; set; }

		/// <summary>
		/// Gets or sets the Report Type.
		/// </summary>
		public string ReportType { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a TestOrderRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Test Order Record.</param>
		/// <returns>The TestOrderRecord entity parsed from the provided string.</returns>
		public static TestOrderRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			var record = new TestOrderRecord {
				RecordID = fields[0],
				TORSeqNumber = int.Parse(fields[1]),
				SpecimenID = fields[2],
				CollectionDateTime = fields[7]
			};

			if (string.IsNullOrEmpty(fields[4]) == false)
				record.UniversalTestId = UniversalTestId.FromString(fields[4]);

			if (string.IsNullOrEmpty(fields[15]) == false)
			{
				var specimenDescriptorFields = fields[15].Split('^');

				record.SpecimenType = specimenDescriptorFields[0];
				record.SpecimenSubtype = specimenDescriptorFields[1];
			}

			return record;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordID}|{TORSeqNumber}|{SpecimenID}||{UniversalTestId}|||{CollectionDateTime}||||||||{SpecimenType}^{SpecimenSubtype}";
		}
	}
}