using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Data;
using TargetPathology.Core.Communications;

namespace TargetPathology.UI
{
	/// <summary>
	/// Interaction logic for SerialPortConfigControl.xaml
	/// </summary>
	public partial class SerialPortConfigControl : UserControl, INotifyPropertyChanged
	{
		private ISerialPort _serialPort;

		public ISerialPort SerialPort
		{
			get => _serialPort;
			set
			{
				_serialPort = value;
				OnPropertyChanged(nameof(SerialPort));
			}
		}

		public ObservableCollection<Parity> Parities { get; } = new ObservableCollection<Parity>
			{ Parity.None, Parity.Odd, Parity.Even, Parity.Mark, Parity.Space };

		public ObservableCollection<StopBits> StopBitsOptions { get; } = new ObservableCollection<StopBits>
			{ StopBits.None, StopBits.One, StopBits.Two, StopBits.OnePointFive };

		public event PropertyChangedEventHandler? PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public SerialPortConfigControl()
		{
			InitializeComponent();
			this.DataContext = this;
		}
	}

	public class ParityStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Parity parityValue)
			{
				return parityValue.ToString();
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string stringValue && Enum.TryParse(typeof(Parity), stringValue, out var parsedValue))
			{
				return (Parity)parsedValue;
			}
			return null;
		}
	}

	public class StopBitsStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is StopBits stopBitsValue)
			{
				return stopBitsValue.ToString();
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is string stringValue && Enum.TryParse(typeof(StopBits), stringValue, out var parsedValue))
			{
				return (StopBits)parsedValue;
			}
			return null;
		}
	}
}