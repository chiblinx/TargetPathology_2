namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents the Message Terminator Record of an Orders List Entry Request.
	/// </summary>
	public class MessageTerminatorRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the record ID. This must be the single letter 'L' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the sequence number. This shall be the single digit '1'.
		/// </summary>
		public string SequenceNumber { get; set; }

		/// <summary>
		/// Gets or sets the termination code. This must be either 'N', 'T', or 'E'.
		/// If the Termination Code is 'N', the message shall be processed normally.
		/// If the Termination Code is 'T' or 'E', the entire message should/shall be discarded.
		/// </summary>
		public string TerminationCode { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a MessageTerminatorRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Message Terminator Record.</param>
		/// <returns>The MessageTerminatorRecord entity parsed from the provided string.</returns>
		public static MessageTerminatorRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			return new MessageTerminatorRecord
			{
				RecordID = fields[0],
				SequenceNumber = fields[1],
				TerminationCode = fields[2]
			};
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordID}|{SequenceNumber}|{TerminationCode}";
		}
	}
}