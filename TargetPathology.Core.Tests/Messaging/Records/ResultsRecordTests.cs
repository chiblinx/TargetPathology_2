using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Tests.Messaging.Records
{
	public class ResultsRecordTests
	{
		[Fact]
		public void FromString_NumericalTestResult_ParsesCorrectly()
		{
			// Arrange
			var numericalTestResultInput = "R|1|^^^CBC^^^MCHC|33.21|||||F|||19960810082234||40009603A";

			// Act
			var result = ResultsRecord.FromString(numericalTestResultInput);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("R", result.RecordType);
			Assert.Equal(1, result.RRSeqNum);
			Assert.Equal("^^^CBC^^^MCHC", result.UniversalTestId.ToString());
			Assert.Equal("33.21", result.DataOrMeasurementValue);
			Assert.Equal('F', result.ResultStatus);
			Assert.Equal("19960810082234", result.DateTimeTestStarted);
			Assert.Equal("40009603A", result.InstrumentIdentification);
		}

		[Fact]
		public void FromString_FlagTestResult_ParsesCorrectly()
		{
			// Arrange
			var flagTestResultInput = "R|23|^^^CBC^^^RBC MORPH|FLAG||||||||19960810082234||40009603A";

			// Act
			var result = ResultsRecord.FromString(flagTestResultInput);

			// Assert
			Assert.NotNull(result);
			Assert.Equal("R", result.RecordType);
			Assert.Equal(23, result.RRSeqNum);
			Assert.Equal("^^^CBC^^^RBC MORPH", result.UniversalTestId.ToString());
			Assert.Equal("FLAG", result.DataOrMeasurementValue);
			Assert.Null(result.ResultStatus); // No ResultStatus in the provided input
			Assert.Equal("19960810082234", result.DateTimeTestStarted);
			Assert.Equal("40009603A", result.InstrumentIdentification);
		}

		[Fact]
		public void ToString_ValidEntity_FormatsCorrectly()
		{
			// Arrange
			var resultsRecord = new ResultsRecord
			{
				RecordType = "R",
				RRSeqNum = 1,
				UniversalTestId = UniversalTestId.FromString("^^^CBC^^^MCHC"),
				DataOrMeasurementValue = "33.21",
				ResultStatus = 'F',
				DateTimeTestStarted = "19960810082234",
				InstrumentIdentification = "40009603A"
			};
			var expectedOutput = "R|1|^^^CBC^^^MCHC|33.21|||||F|||19960810082234||40009603A";

			// Act
			var result = resultsRecord.ToString();

			// Assert
			Assert.Equal(expectedOutput, result);
		}
	}
}
