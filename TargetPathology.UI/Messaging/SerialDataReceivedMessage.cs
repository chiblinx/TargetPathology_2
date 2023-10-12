namespace TargetPathology.UI.Messaging
{
	public class SerialDataReceivedMessage
	{
		public string Data { get; }

		public SerialDataReceivedMessage(string data)
		{
			Data = data;
		}
	}

}
