namespace TargetPathology.Core.Communications
{
	/// <summary>
	/// Represents the current status of a serial port.
	/// </summary>
	public enum SerialPortStatus
	{
		/// <summary>
		/// Indicates the serial port is connected and ready for operations.
		/// </summary>
		Connected,

		/// <summary>
		/// Indicates the serial port is currently reading data.
		/// </summary>
		ReadingData,

		/// <summary>
		/// Indicates the serial port is currently writing data.
		/// </summary>
		WritingData,

		/// <summary>
		/// Indicates the serial port is offline or not operational.
		/// </summary>
		Offline
	}
}
