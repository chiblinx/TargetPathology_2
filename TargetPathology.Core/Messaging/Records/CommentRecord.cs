namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents a Comment Record.
	/// </summary>
	public class CommentRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the record ID. This must be the single character 'C' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the Comment Record Sequence Number. This must be the single digit 1.
		/// </summary>
		public int CRSequenceNumber { get; set; }

		/// <summary>
		/// Gets or sets the Comment Source. This must be the single character 'I' in either upper or lower case.
		/// </summary>
		public string CommentSource { get; set; }

		/// <summary>
		/// Gets or sets the Comment Text. 
		/// </summary>
		public string CommentTextMainReason { get; set; }
		public string CommentTextSecondaryReason { get; set; }

		/// <summary>
		/// Gets or sets the Comment Type. This must be the single character 'G' in either upper or lower case.
		/// </summary>
		public string CommentType { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a CommentRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Comment Record.</param>
		/// <returns>The CommentRecord entity parsed from the provided string.</returns>
		public static CommentRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');
			var commentTextFields = fields[3].Split('^');

			return new CommentRecord
			{
				RecordID = fields[0],
				CRSequenceNumber = int.Parse(fields[1]),
				CommentSource = fields[2],
				CommentTextMainReason = commentTextFields[0],
				CommentTextSecondaryReason = commentTextFields[1],
				CommentType = fields[4]
			};
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{RecordID}|{CRSequenceNumber}|{CommentSource}|{CommentTextMainReason}^{CommentTextSecondaryReason}|{CommentType}";
		}
	}
}
