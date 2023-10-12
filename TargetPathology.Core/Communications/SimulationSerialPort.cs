using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Text;

namespace TargetPathology.Core.Communications
{
	/// <summary>
	/// Simulates a serial port for testing purposes. The class behaves similarly to a real serial port
	/// but without any actual hardware communication.
	/// </summary>
	public class SimulationSerialPort : ISerialPort
	{
		private volatile bool _isOpen = false;
		private readonly ConcurrentQueue<byte> _simulatedInputData = new();

		/// <inheritdoc/>
		public bool IsOpen => _isOpen;

		/// <inheritdoc/>
		public string? PortName { get; set; }

		/// <inheritdoc/>
		public SerialPortStatus Status { get; private set; } = SerialPortStatus.Offline;

		/// <inheritdoc/>
		public int BaudRate { get; set; } = 9600;

		/// <inheritdoc/>
		public Parity Parity { get; set; } = Parity.None;

		/// <inheritdoc/>
		public int DataBits { get; set; } = 8;

		/// <inheritdoc/>
		public StopBits StopBits { get; set; } = StopBits.One;

		/// <inheritdoc/>
		public event SerialDataReceivedDelegate? DataReceived;

		/// <inheritdoc/>
		public void Open()
		{
			if (_isOpen)
				throw new InvalidOperationException("Port is already open");

			_isOpen = true;
		}

		/// <inheritdoc/>
		public void Close()
		{
			_isOpen = false;
		}

		/// <inheritdoc/>
		public int Read(byte[] buffer, int offset, int count)
		{
			if (_isOpen == false)
				throw new InvalidOperationException("Port is not open");

			if (buffer.Length < offset + count)
				throw new ArgumentException("Offset and count exceed buffer bounds.");

			var bytesRead = 0;

			while (bytesRead < count && _simulatedInputData.TryDequeue(out var dataByte))
			{
				buffer[offset + bytesRead] = dataByte;
				bytesRead++;
			}

			return bytesRead;
		}

		/// <inheritdoc/>
		public byte[] ReadAllBytes()
		{
			var buffer = _simulatedInputData.ToArray();
			_simulatedInputData.Clear();
			
			return buffer;
		}

		/// <inheritdoc/>
		public void Write(byte[] buffer, int offset, int count)
		{
			
		}

		/// <summary>
		/// Sets the data that the simulated port will "receive" from the "external device". 
		/// This can be used to specify a sequence of characters for the simulation.
		/// </summary>
		/// <param name="simulatedData">The data string to simulate as received data.</param>
		public void SetSimulatedInputData(string simulatedData)
		{
			if (DataReceived != null)
			{
				DataReceived.Invoke(this, new SerialDataEventArgs { Data = simulatedData });
			}
			else
			{
				var dataBytes = Encoding.UTF8.GetBytes(simulatedData);

				foreach (var byteData in dataBytes)
				{
					_simulatedInputData.Enqueue(byteData);
				}
			}
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			Close();
		}

		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// 
		/// </summary>
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
