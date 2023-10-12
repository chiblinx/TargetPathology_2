using System.ComponentModel;
using System.IO.Ports;

namespace TargetPathology.Core.Communications
{
	/// <summary>
	/// Delegate for handling serial data received events.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">The data received.</param>
	public delegate void SerialDataReceivedDelegate(object sender, SerialDataEventArgs e);

	/// <summary>
	/// Provides data for the DataReceived event.
	/// </summary>
	public class SerialDataEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the data that was received.
		/// </summary>
		public string? Data { get; set; }
	}

	/// <summary>
	/// Defines an interface for serial port communication.
	/// </summary>
	public interface ISerialPort : IDisposable, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets a value indicating whether the serial port is open.
		/// </summary>
		public bool IsOpen { get; }

		/// <summary>
		/// Gets or sets the name of the serial port.
		/// </summary>
		public string? PortName { get; set; }

		/// <summary>
		/// Gets the current status of the serial port.
		/// </summary>
		public SerialPortStatus Status { get; }

		/// <summary>
		/// Gets or sets the baud rate.
		/// </summary>
		public int BaudRate { get; set; }

		/// <summary>
		/// Gets or sets the parity-checking protocol.
		/// </summary>
		public Parity Parity { get; set; }

		/// <summary>
		/// Gets or sets the data bits length.
		/// </summary>
		public int DataBits { get; set; }

		/// <summary>
		/// Gets or sets the standard number of stop bits per byte.
		/// </summary>
		public StopBits StopBits { get; set; }

		/// <summary>
		/// Opens the serial port connection.
		/// </summary>
		public void Open();

		/// <summary>
		/// Closes the serial port connection.
		/// </summary>
		public void Close();

		/// <summary>
		/// Reads a sequence of bytes from the current stream.
		/// </summary>
		/// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read from the current stream.</param>
		/// <returns>The total number of bytes read into the buffer.</returns>
		public int Read(byte[] buffer, int offset, int count);

		/// <summary>
		/// Reads all bytes from the current port.
		/// </summary>
		/// <returns>The bytes read from the port.</returns>
		public byte[] ReadAllBytes();

		/// <summary>
		/// Writes a sequence of bytes to the current stream.
		/// </summary>
		/// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
		/// <param name="count">The number of bytes to be written to the current stream.</param>
		public void Write(byte[] buffer, int offset, int count);

		/// <summary>
		/// Event that gets triggered when data is received on the serial port.
		/// </summary>
		public event SerialDataReceivedDelegate? DataReceived;
	}
}
