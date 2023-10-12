using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using TargetPathology.UI.Messaging;

namespace TargetPathology.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly MainViewModel _viewModel;

		public MainWindow(MainViewModel viewModel, SimpleMessenger messenger)
		{
			InitializeComponent();
			_viewModel = viewModel;
			DataContext = _viewModel;

			messenger.Subscribe<string>(HandleSerialDataReceived);

			// subscribe to property changes from the ViewModel
			_viewModel.PropertyChanged += ViewModel_PropertyChanged;
		}

		private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(MainViewModel.IsOnline) or nameof(MainViewModel.StatusText) or nameof(MainViewModel.StatusColor))
			{
				UpdateOnlineStatus();
			}
		}

		private void UpdateOnlineStatus()
		{
			if (DataContext is MainViewModel viewModel)
			{
				StatusIndicator.Background = viewModel.StatusColor;
				StatusText.Text = viewModel.StatusText;
			}
		}

		public void AppendToConsole(string message)
		{
			ConsoleBox.AppendText(message);
		}

		private void HandleSerialDataReceived(string message)
		{
			Dispatcher.Invoke(() =>
			{
				AppendToConsole(message);
			});
		}

		private void OnConnectClicked(object sender, RoutedEventArgs e)
		{
			_viewModel.Test();
		}

		private void OnSettingsClicked(object sender, RoutedEventArgs e)
		{
			if (DataContext is MainViewModel { SerialPortManager: { } manager } viewModel)
			{
				var selectPortWindow = new SelectSerialPortWindow(manager);

				if (selectPortWindow.ShowDialog() == true)
				{
					viewModel.CurrentSerialPort = selectPortWindow.SelectedPort;
				}
			}
		}

		private void OnStopClicked(object sender, RoutedEventArgs e)
		{
			if (DataContext is MainViewModel { SerialPortManager: { } manager } viewModel)
			{
				viewModel.CloseActivePort();
			}
		}

		private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
		{
			using (SaveFileDialog dialog = new SaveFileDialog())
			{
				// Providing current log name as default file name
				dialog.FileName = _viewModel.GetCurrentLogFileName();

				var result = dialog.ShowDialog();

				if (result == System.Windows.Forms.DialogResult.OK)
				{
					var destinationPath = dialog.FileName;
					_viewModel.ExportLog(destinationPath);
				}
			}
		}
	}
}
