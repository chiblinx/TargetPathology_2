using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;
using TargetPathology.Core.Communications;

namespace TargetPathology.Core.Services
{
	/// <summary>
	/// Provides a thread-safe manager for serial port devices, allowing for addition, retrieval, and removal operations.
	/// </summary>
	public class SerialPortManager : ISerialPortManager
	{
		private readonly ILogger<SerialPortManager> _logger;
		private readonly MessageProcessor _messageProcessor;
		private readonly ConcurrentDictionary<string, ISerialPort> _serialPorts = new();
		private ISerialPort? _activePort;

		private readonly StringBuilder _activeBuffer = new();

		/// <inheritdoc />
		public event ActivePortChangedDelegate? ActivePortChanged;

		/// <inheritdoc />
		public event SerialDataReceivedDelegate? ActivePortDataReceived;

		/// <inheritdoc />
		public event EventHandler<SerialPortStatusChangedEventArgs>? ActivePortStatusChanged;

		/// <inheritdoc />
		public ISerialPort? ActivePort
		{
			get => _activePort;
			set
			{
				if (_activePort != value)
				{
					// clear active buffer
					_activeBuffer.Clear();

					// unsubscribe from previous port's events
					if (_activePort != null)
					{
						_activePort.DataReceived -= (o, args) => ActivePort_DataReceived(o, args);
						_activePort.PropertyChanged -= ActivePort_PropertyChanged;
					}

					var prevPort = _activePort;
					_activePort = value;

					// subscribe to new active port's events
					if (_activePort != null)
					{
						_activePort.DataReceived += (o, args) => ActivePort_DataReceived(o, args);
						_activePort.PropertyChanged += ActivePort_PropertyChanged;
					}

					ActivePortChanged?.Invoke(this, new ActivePortChangedEventArgs
					{
						PreviousPort = prevPort,
						CurrentPort = _activePort
					});
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SerialPortManager"/> class with the specified logging provider.
		/// </summary>
		/// <param name="logger">The logger used for logging events and messages from the SerialPortManager.</param>
		/// <param name="messageProcessor"></param>
		/// <exception cref="ArgumentNullException">Thrown when the provided logger is null.</exception>
		public SerialPortManager(ILogger<SerialPortManager> logger, MessageProcessor messageProcessor)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_messageProcessor = messageProcessor;
		}

		/// <inheritdoc />
		public void AddOrUpdatePort(ISerialPort port)
		{
			if (string.IsNullOrEmpty(port.PortName))
			{
				_logger.LogError("Failed to add or update port. PortName is null or empty.");
				throw new ArgumentNullException(nameof(ISerialPort.PortName));
			}

			_serialPorts.AddOrUpdate(port.PortName, port, (key, oldValue) => port);
			_logger.LogInformation($"Serial port {port.PortName} has been added or updated.");
		}

		/// <inheritdoc />
		public bool TryGetPort(string portName, out ISerialPort port)
		{
			var success = _serialPorts.TryGetValue(portName, out port);

			if (success)
				_logger.LogInformation($"Successfully retrieved serial port {portName}.");
			else
				_logger.LogWarning($"Failed to retrieve serial port {portName}.");

			return success;
		}

		/// <inheritdoc />
		public void RemovePort(string portName)
		{
			var success = _serialPorts.TryRemove(portName, out _);

			if (success)
				_logger.LogInformation($"Serial port {portName} was removed.");
			else
				_logger.LogWarning($"Failed to remove serial port {portName}.");
		}

		/// <inheritdoc />
		public IEnumerable<string> GetAllPortNames()
		{
			return _serialPorts.Keys.ToList();
		}

		/// <inheritdoc />
		public void SetActivePort(string portName)
		{
			if (_serialPorts.TryGetValue(portName, out var port))
			{
				ActivePort = port;
			}
			else
			{
				throw new InvalidOperationException($"Port {portName} not found.");
			}
		}

		/// <inheritdoc />
		public SerialPortStatus GetActivePortStatus()
		{
			return _activePort != null ? _activePort.Status : SerialPortStatus.Offline;
		}

		/// <inheritdoc />
		public void OpenActivePort()
		{
			if (_activePort != null)
			{
				_activePort.Open();
				_logger.LogInformation($"Opened {_activePort.PortName}.");
			}
		}

		/// <inheritdoc />
		public void CloseActivePort()
		{
			if (_activePort != null)
			{
				_activePort.Close();
				_logger.LogInformation($"Closed {_activePort.PortName}.");
				_activePort = null;
			}
		}

		private async Task ActivePort_DataReceived(object sender, SerialDataEventArgs e)
		{
			// forward the DataReceived event of the active port
			ActivePortDataReceived?.Invoke(this, e);

			if (string.IsNullOrEmpty(e.Data) == false)
			{
				await ProcessBufferAsync(Encoding.UTF8.GetBytes(e.Data));
			}
		}

		private void ActivePort_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ISerialPort.Status))
			{
				var args = new SerialPortStatusChangedEventArgs
				{
					Status = _activePort?.Status ?? default
				};

				ActivePortStatusChanged?.Invoke(this, args);
			}
		}

		private async Task ProcessBufferAsync(byte[] receivedBytes)
		{
			// byte arrays
			var endOfTransmissionBytes = Encoding.UTF8.GetBytes("\u0004");
			var enquiryBytes = Encoding.UTF8.GetBytes("\u0005");
			var acknowledgeBytes = Encoding.UTF8.GetBytes("\u0006");

			try
			{
				// wait for start of transmission
				if (receivedBytes.SequenceEqual(enquiryBytes))
				{
					_logger.LogInformation("New transmission starting...");
					_activeBuffer.Clear();
				}
				else if (receivedBytes.SequenceEqual(endOfTransmissionBytes))
				{
					var buffer = _activeBuffer.ToString();
					_activeBuffer.Clear();

					if (string.IsNullOrEmpty(buffer))
					{
						_logger.LogInformation("Transmission ended, but no records detected.");
					}
					else
					{
						_logger.LogInformation("Transmission ended.");

						// cleanup UTF-8 characters
						buffer = buffer.Replace("\u0002", ""); // start of text
						buffer = buffer.Replace("\u0003", Environment.NewLine); // end of text
						buffer = buffer.Replace("\u000D", Environment.NewLine);

						_logger.LogInformation(buffer);
						_logger.LogInformation("Processing records...");

						await _messageProcessor.ProcessMessageAsync(buffer);
					}
				}
				else
				{
					_activeBuffer.Append(Encoding.UTF8.GetString(receivedBytes));
				}

				ActivePort?.Write(acknowledgeBytes, 0, acknowledgeBytes.Length);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to process received bytes and buffer.");
			}
		}
	}
}