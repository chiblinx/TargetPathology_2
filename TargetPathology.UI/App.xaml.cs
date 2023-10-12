using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Windows;
using TargetPathology.Core.DependencyInjection;
using TargetPathology.Core.Logging;
using TargetPathology.UI.Messaging;

namespace TargetPathology.UI
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private readonly IHost _host;

		public static IConfiguration? Configuration { get; private set; }

		public App()
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

			Configuration = builder.Build();

			_host = new HostBuilder()
				.ConfigureAppConfiguration((hostingContext, config) =>
				{
					config.AddConfiguration(Configuration);
				})
				.ConfigureServices((hostContext, services) =>
				{
					// register messaging services
					services.AddSingleton<SimpleMessenger>();

					// register logging services
					services.Configure<FileLoggerOptions>(Configuration.GetSection("Logging:File"));

					services.AddLogging(loggingBuilder =>
					{
						loggingBuilder.Services.AddSingleton<ILoggerProvider>(sp =>
						{
							var options = sp.GetRequiredService<IOptions<FileLoggerOptions>>();
							var messenger = sp.GetRequiredService<SimpleMessenger>();

							return new FileLoggerProvider(options, message =>
							{
								messenger.Send(message + Environment.NewLine);
							});
						});
					});

					// register serial port services
					services.AddSerialPortServices();

					// register main window UI as services
					services.AddTransient<MainViewModel>();
					services.AddTransient<MainWindow>();

					// register other services or configurations here if needed
				})
				.Build();
		}

		protected override async void OnStartup(StartupEventArgs e)
		{
			await _host.StartAsync();

			var mainWindow = _host.Services.GetService<MainWindow>() ?? throw new InvalidOperationException($"Could not create an instance of {nameof(MainWindow)}!");
			mainWindow.Show();

			base.OnStartup(e);
		}

		protected override async void OnExit(ExitEventArgs e)
		{
			using (_host)
			{
				await _host.StopAsync(TimeSpan.FromSeconds(5));
			}
			base.OnExit(e);
		}
	}
}
