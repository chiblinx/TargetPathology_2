using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class TestOrderRecordTests
	{
		[Fact]
		public void FromString_ValidAsciiString_ParsesCorrectly()
		{
			// Arrange
			var testInput = "O|1|19345||^^^CBC^5^0|||19960810153028||||||||Patient^Human";

			// Act
			var result = TestOrderRecord.FromString(testInput);

			// Assert
			Assert.Equal("O", result.RecordID);
			Assert.Equal(1, result.TORSeqNumber);
			Assert.Equal("19345", result.SpecimenID);
			Assert.Equal("CBC", result.UniversalTestId.TestSelection);
			Assert.Equal(5, result.UniversalTestId.ParameterSetSelection);
			Assert.Equal(0, result.UniversalTestId.LimitSetSelection);
			Assert.Equal("19960810153028", result.CollectionDateTime);
			Assert.Equal("Patient", result.SpecimenType);
			Assert.Equal("Human", result.SpecimenSubtype);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var testOrderRecord = new TestOrderRecord
			{
				RecordID = "O",
				TORSeqNumber = 1,
				SpecimenID = "19345",
				CollectionDateTime = "19960810153028",
				SpecimenType = "Patient",
				SpecimenSubtype = "Human"
			};

			testOrderRecord.UniversalTestId = new UniversalTestId
			{
				TestSelection = "CBC",
				ParameterSetSelection = 5,
				LimitSetSelection = 0
			};

			// Act
			var result = testOrderRecord.ToString();

			// Assert
			Assert.Equal("O|1|19345||^^^CBC^5^0|||19960810153028||||||||Patient^Human", result);
		}
	}
}
