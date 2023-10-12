using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class CommentRecordTests
	{
		[Fact]
		public void FromString_ValidString_ParsesCorrectly()
		{
			// Arrange
			var testInput = "C|1|I|Order List Add Failed^RecordID|G";

			// Act
			var result = CommentRecord.FromString(testInput);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("C", result.RecordID);
			Assert.Equal(1, result.CRSequenceNumber);
			Assert.Equal("I", result.CommentSource);
			Assert.Equal("Order List Add Failed", result.CommentTextMainReason);
			Assert.Equal("RecordID", result.CommentTextSecondaryReason);
			Assert.Equal("G", result.CommentType);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var commentRecord = new CommentRecord
			{
				RecordID = "C",
				CRSequenceNumber = 1,
				CommentSource = "I",
				CommentTextMainReason = "Order List Add Failed",
				CommentTextSecondaryReason = "RecordID",
				CommentType = "G"
			};

			var expectedOutput = "C|1|I|Order List Add Failed^RecordID|G";

			// Act
			var result = commentRecord.ToString();

			// Assert
			Assert.Equal(expectedOutput, result);
		}
	}
}
