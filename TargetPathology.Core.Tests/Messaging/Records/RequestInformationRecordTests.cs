using TargetPathology.Core.Messaging.Records;
using static TargetPathology.Core.Messaging.Records.RequestInformationRecord;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class RequestInformationRecordTests
	{
		[Fact]
		public void FromString_ValidString_ParsesCorrectly()
		{
			// Arrange
			// Test with the "^ALL" example
			var allOrdersInput = "Q|1|^ALL";

			// Act
			var allOrdersResult = FromString(allOrdersInput);

			// Assert
			Assert.NotNull(allOrdersResult);
			Assert.Equal("Q", allOrdersResult.RecordID);
			Assert.Equal("1", allOrdersResult.RIRSequenceNumber);
			Assert.Empty(allOrdersResult.RangeID.FirstComponent);
			Assert.Equal("ALL", allOrdersResult.RangeID.SecondComponent);

			// Arrange
			// Test with the "^M34566" example
			var specificOrderInput = "Q|1|^M34566";

			// Act
			var specificOrderResult = FromString(specificOrderInput);

			// Assert
			Assert.NotNull(specificOrderResult);
			Assert.Equal("Q", specificOrderResult.RecordID);
			Assert.Equal("1", specificOrderResult.RIRSequenceNumber);
			Assert.Empty(specificOrderResult.RangeID.FirstComponent);
			Assert.Equal("M34566", specificOrderResult.RangeID.SecondComponent);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var requestInformationRecord = new RequestInformationRecord
			{
				RecordID = "Q",
				RIRSequenceNumber = "1",
				RangeID = new StartingRangeIDNumber
				{
					FirstComponent = "",
					SecondComponent = "ALL"
				}
			};
			var expectedOutput = "Q|1|^ALL";

			// Act
			var result = requestInformationRecord.ToString();

			// Assert
			Assert.Equal(expectedOutput, result);
		}
	}
}
