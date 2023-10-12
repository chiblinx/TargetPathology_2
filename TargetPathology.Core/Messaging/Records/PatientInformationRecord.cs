namespace TargetPathology.Core.Messaging.Records
{
	/// <summary>
	/// Represents the Patient Information Record (PIR) of an Orders List Entry Request.
	/// </summary>
	public class PatientInformationRecord : IRecord
	{
		/// <summary>
		/// Gets or sets the record ID. This must be the single letter 'P' in either upper or lower case.
		/// </summary>
		public string RecordID { get; set; }

		/// <summary>
		/// Gets or sets the PIR sequence number.
		/// </summary>
		public int SequenceNumber { get; set; }

		/// <summary>
		/// Gets or sets the Laboratory Assigned Patient ID.
		/// </summary>
		public string LaboratoryAssignedPatientID { get; set; }

		/// <summary>
		/// Gets or sets the Patient Name.
		/// </summary>
		public string PatientName { get; set; }

		/// <summary>
		/// Gets or sets the Birthdate in YYYYMMDD format.
		/// </summary>
		public string Birthdate { get; set; }

		/// <summary>
		/// Gets or sets the Patient Sex. Valid values are "M", "F", and "U".
		/// </summary>
		public char PatientSex { get; set; }

		/// <summary>
		/// Gets or sets the Attending Physician's name.
		/// </summary>
		public string AttendingPhysician { get; set; }

		/// <summary>
		/// Gets or sets the User-Defined Demographics Special Field 2. 
		/// This field consists of two components UF1 and UF2.
		/// </summary>
		public SpecialField? SpecialField2 { get; set; }

		/// <summary>
		/// Converts an ASCII formatted string into a PatientInformationRecord entity.
		/// </summary>
		/// <param name="asciiString">The ASCII string representing the Patient Information Record.</param>
		/// <returns>The PatientInformationRecord entity parsed from the provided string.</returns>
		public static PatientInformationRecord FromString(string asciiString)
		{
			var fields = asciiString.Split('|');

			var record = new PatientInformationRecord
			{
				RecordID = fields[0],
				SequenceNumber = int.Parse(fields[1]),
				LaboratoryAssignedPatientID = fields[3],
				PatientName = fields[5],
				Birthdate = fields[7],
				PatientSex = Convert.ToChar(fields[8]),
				AttendingPhysician = fields[13],
			};

			if (fields.Length >= 16)
			{
				record.SpecialField2 = new SpecialField
				{
					UF1 = fields[15].Split('^')[0],
					UF2 = fields[15].Split('^')[1]
				};
			}

			return record;
		}

		/// <inheritdoc />
		public override string ToString()
		{
			var result = $"{RecordID}|{SequenceNumber}||{LaboratoryAssignedPatientID}||{PatientName}||{Birthdate}|{PatientSex}|||||{AttendingPhysician}||";

			if (SpecialField2 != null)
				result += $"{SpecialField2.UF1}^{SpecialField2.UF2}";

			return result;
		}
	}

	public class SpecialField
	{
		/// <summary>
		/// Gets or sets the User Field 1 component.
		/// </summary>
		public string UF1 { get; set; }

		/// <summary>
		/// Gets or sets the User Field 2 component.
		/// </summary>
		public string UF2 { get; set; }
	}
}