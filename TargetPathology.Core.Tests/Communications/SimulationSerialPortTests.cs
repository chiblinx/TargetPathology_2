using System.Text;
using TargetPathology.Core.Communications;

namespace TargetPathology.Core.Tests.Communications
{
	public class SimulationSerialPortTests
	{
		[Fact]
		public void Test_OpenClose()
		{
			using var simulatedPort = new SimulationSerialPort();

			Assert.False(simulatedPort.IsOpen, "Port should be closed initially.");

			simulatedPort.Open();
			Assert.True(simulatedPort.IsOpen, "Port should be open after calling Open().");

			simulatedPort.Close();
			Assert.False(simulatedPort.IsOpen, "Port should be closed after calling Close().");
		}

		[Fact]
		public void Test_WriteRead()
		{
			using var simulatedPort = new SimulationSerialPort();

			var dataToSend = Encoding.ASCII.GetBytes("TestData");
			simulatedPort.Open();

			simulatedPort.Write(dataToSend, 0, dataToSend.Length);

			var receivedBuffer = new byte[dataToSend.Length];
			var bytesRead = simulatedPort.Read(receivedBuffer, 0, receivedBuffer.Length);

			Assert.Equal(dataToSend.Length, bytesRead);
			Assert.Equal(dataToSend, receivedBuffer);
		}

		[Fact]
		public void Test_SimulatedInputData()
		{
			using var simulatedPort = new SimulationSerialPort();
			var simulatedData = "Test";

			simulatedPort.SetSimulatedInputData(simulatedData);
			simulatedPort.Open();

			var receivedBuffer = new byte[simulatedData.Length];
			var bytesRead = 0;

			while (bytesRead < simulatedData.Length)
			{
				bytesRead += simulatedPort.Read(receivedBuffer, bytesRead, simulatedData.Length - bytesRead);
			}

			var receivedString = Encoding.ASCII.GetString(receivedBuffer);
			Assert.Equal(simulatedData, receivedString);
		}

		[Fact]
		public async Task Test_DataReceivedEvent()
		{
			using var simulatedPort = new SimulationSerialPort();

			var simulatedData = "EventTest";
			simulatedPort.SetSimulatedInputData(simulatedData);

			var receivedDataBuilder = new StringBuilder();

			simulatedPort.DataReceived += (sender, e) =>
			{
				receivedDataBuilder.Append(e.Data);
			};

			simulatedPort.Open();

			// the simulated port has a delay in its data reception simulation, so wait for a bit
			await Task.Delay(simulatedData.Length * 600);  // assuming 500ms delay for each character

			Assert.Equal(simulatedData, receivedDataBuilder.ToString());
		}
	}
}
