using TargetPathology.Core.Messaging;
using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging
{
	public class MessageConversionHelpersTest
	{
		[Theory]
		[InlineData("H|\\^&|||40009603A^CDRuby^R5-4H^1.0|||||||P|LIS2-A", "H")]
		[InlineData("L|1|N", "L")]
		[InlineData("Q|1|^ALL", "Q")]
		[InlineData("P|1||A56342||David Jones||19640315|M|||||Roberts||Phillips^sickle crisis", "P")]
		[InlineData("O|1|19345||^^^CBC^5^0|||19960810153028||||||||Patient^Human", "O")]
		[InlineData("C|1|I|Order List Add Failed^RecordID|G", "C")]
		[InlineData("R|1|^^^CBC^^^MCHC|33.21|||||F|||19960810082234||40009603A", "R")]
		public void GetRecordType_DetectsHeaderType(string input, string expectedHeaderType)
		{
			// Act
			var recordType = MessageConversionHelpers.GetRecordType(input);

			// Assert
			Assert.Equal(expectedHeaderType, recordType);
		}

		[Theory]
		[InlineData("H|\\^&|||40009603A^CDRuby^R5-4H^1.0|||||||P|LIS2-A", typeof(HeaderRecord))]
		[InlineData("L|1|N", typeof(MessageTerminatorRecord))]
		[InlineData("Q|1|^ALL", typeof(RequestInformationRecord))]
		[InlineData("P|1||A56342||David Jones||19640315|M|||||Roberts||Phillips^sickle crisis", typeof(PatientInformationRecord))]
		[InlineData("O|1|19345||^^^CBC^5^0|||19960810153028||||||||Patient^Human", typeof(TestOrderRecord))]
		[InlineData("C|1|I|Order List Add Failed^RecordID|G", typeof(CommentRecord))]
		[InlineData("R|1|^^^CBC^^^MCHC|33.21|||||F|||19960810082234||40009603A", typeof(ResultsRecord))]
		public void GetRecord_GetsCorrectRecord(string input, Type expectedRecordType)
		{
			// Act
			var record = MessageConversionHelpers.GetRecord(input);

			// Assert
			Assert.Equal(expectedRecordType, record!.GetType());
		}

		[Fact]
		public void GetRecord_ReturnsNullOnUnknownRecord()
		{
			// Arrange
			var input = "test input";

			// Act
			var record = MessageConversionHelpers.GetRecord(input);

			// Assert
			Assert.Null(record);
		}
	}
}
