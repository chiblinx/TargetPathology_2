using System.Windows;
using System.Windows.Controls;
using TargetPathology.Core.Services;

namespace TargetPathology.UI
{
	public partial class SelectSerialPortWindow : Window
	{
		private readonly ISerialPortManager _serialPortManager;

		public string? SelectedPort { get; private set; }

		public SelectSerialPortWindow(ISerialPortManager serialPortManager)
		{
			InitializeComponent();

			_serialPortManager = serialPortManager;

			if (_serialPortManager.ActivePort != null)
				SelectedPort = _serialPortManager.ActivePort.PortName;

			UpdatePortList();
		}

		private void UpdatePortList()
		{
			PortListBox.ItemsSource = _serialPortManager.GetAllPortNames();
		}

		private void PortListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (PortListBox.SelectedItem is string portName && _serialPortManager.TryGetPort(portName, out var serialPort))
				SerialPortConfigControl.SerialPort = serialPort;
		}

		private void RefreshButton_Click(object sender, RoutedEventArgs e)
		{
			UpdatePortList();
		}

		private void OnSelectClicked(object sender, RoutedEventArgs e)
		{
			if (SerialPortConfigControl.SerialPort != null)
			{
				// update the serial port configuration in the manager
				_serialPortManager.AddOrUpdatePort(SerialPortConfigControl.SerialPort);

				SelectedPort = SerialPortConfigControl.SerialPort.PortName;
			}

			DialogResult = true;
			this.Close();
		}
	}
}