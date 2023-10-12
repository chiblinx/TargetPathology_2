using Microsoft.Extensions.Options;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using TargetPathology.Core.Communications;
using TargetPathology.Core.Logging;
using TargetPathology.Core.Services;
using TargetPathology.UI.Messaging;

namespace TargetPathology.UI
{
	public sealed class MainViewModel : INotifyPropertyChanged
	{
		public readonly ISerialPortManager SerialPortManager;

		private readonly IOptions<FileLoggerOptions> _fileLoggerOptions;
		private readonly StatisticsTracker _statisticsTracker;

		// selected serial port
		private string? _currentSerialPortName;
		public string? CurrentSerialPort
		{
			get => _currentSerialPortName;
			set
			{
				if (_currentSerialPortName != value)
				{
					// Close and detach from the previously active port
					CloseActivePort();

					_currentSerialPortName = value;

					if (string.IsNullOrEmpty(_currentSerialPortName) == false)
					{
						SerialPortManager.SetActivePort(_currentSerialPortName);
						SerialPortManager.OpenActivePort();
					}

					OnPropertyChanged(nameof(CurrentSerialPort));
					OnPropertyChanged(nameof(IsOnline));
					OnPropertyChanged(nameof(StatusText));
					OnPropertyChanged(nameof(StatusColor));
				}
			}
		}

		public int RecordsRead => _statisticsTracker.RecordsRead;
		public int RecordsProcessed => _statisticsTracker.RecordsProcessed;
		public int TestOrderRecordsReceived => _statisticsTracker.TestOrderRecordsReceived;
		public int ResultsRecordsReceived => _statisticsTracker.ResultsRecordsReceived;
		public int RecordsWritten => _statisticsTracker.RecordsWritten;
		public int DatabaseErrors => _statisticsTracker.DatabaseErrors;

		// application version
		public static string ApplicationVersion
		{
			get
			{
				var version = Assembly.GetExecutingAssembly().GetName().Version;
				return version != null ? version.ToString() : "v1.0.0";
			}
		}

		// serial port status
		public bool IsOnline
		{
			get
			{
				if (SerialPortManager.ActivePort == null)
					return false;

				return SerialPortManager.ActivePort.IsOpen;
			}
		}

		// status text
		public string StatusText => IsOnline ? "Online" : "Offline";

		// status color
		public SolidColorBrush StatusColor => IsOnline ? Brushes.Green : Brushes.Red;

		// event for properties changed
		public event PropertyChangedEventHandler? PropertyChanged;

		public MainViewModel(ISerialPortManager serialPortManager, IOptions<FileLoggerOptions> fileLoggerOptions, 
			StatisticsTracker statisticsTracker)
		{
			SerialPortManager = serialPortManager;
			_fileLoggerOptions = fileLoggerOptions;
			_statisticsTracker = statisticsTracker;

			// Subscribe to the SerialPortManager's events
			SerialPortManager.ActivePortStatusChanged += SerialPortManager_ActivePortStatusChanged;
			SerialPortManager.ActivePortDataReceived += SerialPortManager_SerialDataReceived;

			// Subscribe to statistic tracker property changed events
			_statisticsTracker.PropertyChanged += delegate(object? sender, PropertyChangedEventArgs args)
			{
				if (args.PropertyName != null) 
					OnPropertyChanged(args.PropertyName);
			};
		}

		private void SerialPortManager_SerialDataReceived(object sender, SerialDataEventArgs e)
		{
			var responseBytes = Encoding.ASCII.GetBytes(e.Data);

			// <ACK> message received
			if (responseBytes == new byte[] { 0x06 })
			{
				// Send an <EOT> message
				var eotMessage = new byte[] { 0x04 }; // ASCII for <EOT>
				SerialPortManager.ActivePort.Write(eotMessage, 0, eotMessage.Length);
			}
			else
			{
				// Handle unexpected responses or timeouts
				Console.WriteLine("Unexpected response or timeout.");
			}
		}

		private void SerialPortManager_ActivePortStatusChanged(object sender, SerialPortStatusChangedEventArgs e)
		{
			OnPropertyChanged(nameof(SerialPortStatus));
			OnPropertyChanged(nameof(IsOnline));
			OnPropertyChanged(nameof(StatusText));
			OnPropertyChanged(nameof(StatusColor));
		}

		private void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public void CloseActivePort()
		{
			SerialPortManager.CloseActivePort();

			_currentSerialPortName = null;

			OnPropertyChanged(nameof(CurrentSerialPort));
			OnPropertyChanged(nameof(IsOnline));
			OnPropertyChanged(nameof(SerialPortStatus));
			OnPropertyChanged(nameof(StatusText));
			OnPropertyChanged(nameof(StatusColor));
		}

		public string GetCurrentLogFileName()
		{
			return Path.GetFileName(FileLoggerHelpers.GetCurrentLogPath(_fileLoggerOptions));
		}

		public void ExportLog(string destinationPath)
		{
			var logFilePath = FileLoggerHelpers.GetCurrentLogPath(_fileLoggerOptions);

			if (string.IsNullOrEmpty(Path.GetFileName(destinationPath)))
				destinationPath = Path.Combine(destinationPath, Path.GetFileName(logFilePath));

			File.Copy(logFilePath, destinationPath, true);
		}

		public void Test()
		{
			if (SerialPortManager.ActivePort == null)
			{
				var portNames = SerialPortManager.GetAllPortNames();
				SerialPortManager.SetActivePort(portNames.First());

				SerialPortManager.OpenActivePort();

				OnPropertyChanged(nameof(CurrentSerialPort));
				OnPropertyChanged(nameof(IsOnline));
				OnPropertyChanged(nameof(StatusText));
				OnPropertyChanged(nameof(StatusColor));
			}

			if (SerialPortManager.ActivePort!.IsOpen == false)
				SerialPortManager.OpenActivePort();

			// send an <ENQ> message
			var enqMessage = new byte[] { 0x05 }; // ASCII for <ENQ>
			SerialPortManager.ActivePort.Write(enqMessage, 0, enqMessage.Length);
		}
	}

	public class SerialPortStatusStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is SerialPortStatus serialPortStatusValue)
			{
				return serialPortStatusValue.ToString();
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string stringValue && Enum.TryParse(typeof(SerialPortStatus), stringValue, out var parsedValue))
			{
				return (SerialPortStatus)parsedValue;
			}
			return null;
		}
	}
}
