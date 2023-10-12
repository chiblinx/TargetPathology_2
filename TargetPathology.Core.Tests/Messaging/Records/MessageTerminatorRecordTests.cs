using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class MessageTerminatorRecordTests
	{
		[Fact]
		public void FromString_ValidString_ParsesCorrectly()
		{
			// Arrange
			var testInput = "L|1|N";

			// Act
			var result = MessageTerminatorRecord.FromString(testInput);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("L", result.RecordID);
			Assert.Equal("1", result.SequenceNumber);
			Assert.Equal("N", result.TerminationCode);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var messageTerminatorRecord = new MessageTerminatorRecord
			{
				RecordID = "L",
				SequenceNumber = "1",
				TerminationCode = "N"
			};
			var expectedOutput = "L|1|N";

			// Act
			var result = messageTerminatorRecord.ToString();

			// Assert
			Assert.Equal(expectedOutput, result);
		}
	}
}
