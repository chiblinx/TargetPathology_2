using TargetPathology.Core.Messaging.Records;
using static TargetPathology.Core.Messaging.Records.PatientInformationRecord;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class PatientInformationRecordTests
	{
		[Fact]
		public void FromString_ValidAsciiString_ParsesCorrectly()
		{
			// Arrange
			var testInput = "P|1||A56342||David Jones||19640315|M|||||Roberts||Phillips^sickle crisis";

			// Act
			var result = FromString(testInput);

			// Assert
			Assert.Equal("P", result.RecordID);
			Assert.Equal(1, result.SequenceNumber);
			Assert.Equal("A56342", result.LaboratoryAssignedPatientID);
			Assert.Equal("David Jones", result.PatientName);
			Assert.Equal("19640315", result.Birthdate);
			Assert.Equal('M', result.PatientSex);
			Assert.Equal("Roberts", result.AttendingPhysician);
			Assert.Equal("Phillips", result.SpecialField2.UF1);
			Assert.Equal("sickle crisis", result.SpecialField2.UF2);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var record = new PatientInformationRecord
			{
				RecordID = "P",
				SequenceNumber = 1,
				LaboratoryAssignedPatientID = "A56342",
				PatientName = "David Jones",
				Birthdate = "19640315",
				PatientSex = 'M',
				AttendingPhysician = "Roberts",
				SpecialField2 = new SpecialField()
				{
					UF1 = "Phillips",
					UF2 = "sickle crisis"
				}
			};

			// Act
			var result = record.ToString();

			// Assert
			Assert.Equal("P|1||A56342||David Jones||19640315|M|||||Roberts||Phillips^sickle crisis", result);
		}
	}
}
