using System;
using System.ComponentModel;
using System.IO.Ports;

namespace TargetPathology.Core.Communications
{
	/// <summary>
	/// Represents the standard implementation of the <see cref="ISerialPort"/> interface using <see cref="SerialPort"/>.
	/// </summary>
	public class StandardSerialPort : ISerialPort
	{
		private readonly SerialPort _port;

		/// <summary>
		/// Initializes a new instance of the <see cref="StandardSerialPort"/> class with the specified port name.
		/// </summary>
		/// <param name="portName">The name of the serial port to be accessed (e.g., "COM3").</param>
		public StandardSerialPort(string portName)
		{
			_port = new SerialPort(portName);
			_port.DataReceived += OnDataReceived;
		}

		/// <inheritdoc/>
		public bool IsOpen => _port.IsOpen;

		/// <inheritdoc/>
		public string? PortName
		{
			get => _port.PortName;
			set => _port.PortName = string.IsNullOrEmpty(value) ? "" : value;
		}

		private SerialPortStatus _status = SerialPortStatus.Offline;

		/// <inheritdoc/>
		public SerialPortStatus Status
		{
			get => _status;
			private set
			{
				if (_status != value)
				{
					_status = value;
					OnPropertyChanged(nameof(Status)); // Raise the event when Status changes
				}
			}
		}

		/// <inheritdoc/>
		public int BaudRate
		{
			get => _port.BaudRate;
			set => _port.BaudRate = value;
		}

		/// <inheritdoc/>
		public Parity Parity
		{
			get => _port.Parity;
			set => _port.Parity = value;
		}

		/// <inheritdoc/>
		public int DataBits
		{
			get => _port.DataBits;
			set => _port.DataBits = value;
		}

		/// <inheritdoc/>
		public StopBits StopBits
		{
			get => _port.StopBits;
			set => _port.StopBits = value;
		}

		/// <inheritdoc/>
		public void Open()
		{
			_port.Open();
			Status = SerialPortStatus.Connected;
		}

		/// <inheritdoc/>
		public void Close()
		{
			_port.Close();
			Status = SerialPortStatus.Offline;
		}

		/// <inheritdoc/>
		public int Read(byte[] buffer, int offset, int count)
		{
			Status = SerialPortStatus.ReadingData;
			return _port.Read(buffer, offset, count);
		}

		/// <inheritdoc/>
		public byte[] ReadAllBytes()
		{
			var buffer = new byte[] {};

			Status = SerialPortStatus.ReadingData;

			if (_port.BytesToRead > 0)
			{
				_port.Read(buffer, 0, _port.BytesToRead);
			}

			return buffer;
		}

		/// <inheritdoc/>
		public void Write(byte[] buffer, int offset, int count)
		{
			_port.Write(buffer, offset, count);
			Status = SerialPortStatus.WritingData;
		}

		/// <inheritdoc/>
		public event SerialDataReceivedDelegate? DataReceived;

		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Property changed.
		/// </summary>
		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		/// <inheritdoc/>
		public void Dispose()
		{
			_port.DataReceived -= OnDataReceived;
			_port.Dispose();
		}

		private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			var data = _port.ReadExisting();
			DataReceived?.Invoke(this, new SerialDataEventArgs { Data = data });
		}
	}
}