using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class HeaderRecordTests
	{
		[Fact]
		public void FromString_ValidString_ParsesCorrectly()
		{
			// Arrange
			var testInput = "H|\\^&|||40009603A^CDRuby^R5-4H^1.0|||||||P|LIS2-A";

			// Act
			var result = HeaderRecord.FromString(testInput);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("H", result.RecordID);
			Assert.Equal("\\^&", result.Delimiters);
			Assert.Equal("40009603A", result.Sender.AnalyzerSerialNumber);
			Assert.Equal("CDRuby", result.Sender.Model);
			Assert.Equal("R5-4H", result.Sender.SoftwareVersionID);
			Assert.Equal("1.0", result.Sender.HostComputerIFVersionID);
			Assert.Equal("P", result.ProcessingID);
			Assert.Equal("LIS2-A", result.CLSIVersionNo);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var headerRecord = new HeaderRecord
			{
				RecordID = "H",
				Delimiters = "\\^&",
				Sender = new SenderID
				{
					AnalyzerSerialNumber = "40009603A",
					Model = "CDRuby",
					SoftwareVersionID = "R5-4H",
					HostComputerIFVersionID = "1.0"
				},
				ProcessingID = "P",
				CLSIVersionNo = "LIS2-A"
			};

			var expectedOutput = "H|\\^&|||40009603A^CDRuby^R5-4H^1.0|||||||P|LIS2-A";

			// Act
			var result = headerRecord.ToString();

			// Assert
			Assert.Equal(expectedOutput, result);
		}
	}
}