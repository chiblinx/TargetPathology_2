using TargetPathology.Core.Communications;

namespace TargetPathology.Core.Services
{
	/// <summary>
	/// Represents a method that will handle the event when the active serial port changes.
	/// </summary>
	/// <param name="sender">The source of the event.</param>
	/// <param name="e">A <see cref="ActivePortChangedEventArgs"/> that contains the event data.</param>
	public delegate void ActivePortChangedDelegate(object sender, ActivePortChangedEventArgs e);

	/// <summary>
	/// Provides data for the <see cref="ISerialPortManager.ActivePortChanged"/> event.
	/// </summary>
	public class ActivePortChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the serial port that was previously set as active.
		/// </summary>
		public ISerialPort? PreviousPort { get; set; }

		/// <summary>
		/// Gets or sets the serial port that is currently set as active.
		/// </summary>
		public ISerialPort? CurrentPort { get; set; }
	}

	/// <summary>
	/// Provides data for the ActivePortStatusChanged event.
	/// </summary>
	public class SerialPortStatusChangedEventArgs : EventArgs
	{
		/// <summary>
		/// Gets or sets the current status of the active serial port.
		/// </summary>
		public SerialPortStatus Status { get; set; }
	}

	/// <summary>
	/// Provides an interface for managing and accessing serial port devices.
	/// </summary>
	public interface ISerialPortManager
	{
		/// <summary>
		/// Adds or updates the provided serial port in the manager.
		/// </summary>
		/// <param name="port">The serial port to be added or updated.</param>
		public void AddOrUpdatePort(ISerialPort port);

		/// <summary>
		/// Attempts to get a serial port by its port name.
		/// </summary>
		/// <param name="portName">The name of the port, e.g., "COM3".</param>
		/// <param name="port">When this method returns, contains the <see cref="ISerialPort"/> instance if the port is found, or null if the port is not found.</param>
		/// <returns><c>true</c> if the port is found; otherwise, <c>false</c>.</returns>
		public bool TryGetPort(string portName, out ISerialPort port);

		/// <summary>
		/// Removes the specified serial port from the manager.
		/// </summary>
		/// <param name="portName">The name of the port to be removed, e.g., "COM3".</param>
		public void RemovePort(string portName);

		/// <summary>
		/// Gets a collection of all available port names managed by this service.
		/// </summary>
		/// <returns>A collection of port names.</returns>
		public IEnumerable<string> GetAllPortNames();

		/// <summary>
		/// Gets the currently set active serial port.
		/// </summary>
		public ISerialPort? ActivePort { get; }

		/// <summary>
		/// Sets a serial port as the active port using its name.
		/// </summary>
		/// <param name="portName">The name of the port to be set as active, e.g., "COM3".</param>
		/// <exception cref="InvalidOperationException">Thrown when the port with the given name does not exist.</exception>
		public void SetActivePort(string portName);

		/// <summary>
		/// Retrieves the status of the currently active serial port.
		/// </summary>
		/// <returns>
		/// The status of the active serial port. If no port is currently active, it will return SerialPortStatus.Offline.
		/// </returns>
		public SerialPortStatus GetActivePortStatus();

		/// <summary>
		/// Opens the currently active serial port if there's any.
		/// </summary>
		public void OpenActivePort();

		/// <summary>
		/// Closes the currently active serial port if there's any.
		/// </summary>
		public void CloseActivePort();

		/// <summary>
		/// Occurs when the active port changes.
		/// </summary>
		public event ActivePortChangedDelegate? ActivePortChanged;

		/// <summary>
		/// Occurs when data is received on the active port.
		/// </summary>
		public event SerialDataReceivedDelegate? ActivePortDataReceived;

		/// <summary>
		/// Event that gets triggered when the status of the active serial port changes.
		/// </summary>
		public event EventHandler<SerialPortStatusChangedEventArgs>? ActivePortStatusChanged;
	}
}