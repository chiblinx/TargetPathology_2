namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents the Request Information Record (RIR) of an Orders List Entry Request.
	/// </summary>
	public class RequestInformationRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the Record ID. This must be the single letter 'Q' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the Request Information Record (RIR) Sequence Number. The RIR Sequence Number shall be the single digit 1.
		/// </summary>
		public string RIRSequenceNumber { get; set; }

		/// <summary>
		/// Gets or sets the Starting Range ID Number
		/// </summary>
		public StartingRangeIDNumber RangeID { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a RequestInformationRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Request Information Record.</param>
		/// <returns>The RequestInformationRecord entity parsed from the provided string.</returns>
		public static RequestInformationRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			return new RequestInformationRecord
			{
				RecordID = fields[0],
				RIRSequenceNumber = fields[1],
				RangeID = new StartingRangeIDNumber
				{
					FirstComponent = fields[2].Split('^')[0],
					SecondComponent = fields[2].Split('^')[1]
				}
			};
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordID}|{RIRSequenceNumber}|{RangeID}";
		}
	}

	/// <summary>
	/// Represents the Starting Range ID Number component details for the Request Information Record.
	/// </summary>
	public class StartingRangeIDNumber
	{
		/// <summary>
		/// Gets or sets the first component of the Starting Range ID Number.
		/// This component shall always be null.
		/// </summary>
		public string FirstComponent { get; set; }

		/// <summary>
		/// Gets or sets the second component of the Starting Range ID Number.
		/// The second component can be the uppercase 3-character ASCII string “ALL” or a Specimen ID.
		/// </summary>
		public string SecondComponent { get; set; }

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{FirstComponent}^{SecondComponent}";
		}
	}
}
