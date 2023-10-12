namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents the Universal Test ID of a Record.
	/// </summary>
	public class UniversalTestId
	{
		/// <summary>
		/// Gets or sets the Universal Test ID.
		/// </summary>
		public string TestCodeId { get; set; }
		
		/// <summary>
		/// Gets or sets the Universal Test Name.
		/// </summary>
		public string TestName { get; set; }

		/// <summary>
		/// Gets or sets the Universal Test Type.
		/// </summary>
		public string TestType { get; set; }

		/// <summary>
		/// Gets or sets the Test Selection.
		/// </summary>
		public string? TestSelection { get; set; }

		/// <summary>
		/// Gets or sets the Parameter Set Selection.
		/// </summary>
		public int? ParameterSetSelection { get; set; }

		/// <summary>
		/// Gets or sets the Limit Set Selection.
		/// </summary>
		public int? LimitSetSelection { get; set; }

		/// <summary>
		/// Gets or sets the Result Label.
		/// </summary>
		public string? ResultLabel { get; set; }

		public static UniversalTestId FromString(string asciiString)
		{
			var fields = asciiString.Split('^');

			var universalTestId = new UniversalTestId
			{
				TestCodeId = fields[0],
				TestName = fields[1],
				TestType = fields[2],
				TestSelection = fields[3],
				ParameterSetSelection = string.IsNullOrEmpty(fields[4]) ? null : Convert.ToInt32(fields[4]),
				LimitSetSelection = string.IsNullOrEmpty(fields[5]) ? null : Convert.ToInt32(fields[5])
			};

			if (fields.Length >= 7)
			{
				universalTestId.ResultLabel = fields[6];
			}

			return universalTestId;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var universalTestIdString = $"{TestCodeId}^{TestName}^{TestType}^{TestSelection}^{ParameterSetSelection}^{LimitSetSelection}";

			if (string.IsNullOrEmpty(ResultLabel) == false)
			{
				universalTestIdString += $"^{ResultLabel}";
			}

			return universalTestIdString;
		}
	}
}