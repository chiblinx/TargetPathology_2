using Microsoft.Extensions.Logging;

namespace TargetPathology.Core.Logging
{
	/// <summary>
	/// Provides logging functionality, writing logs to a specified file.
	/// </summary>
	public class FileLogger : ILogger
	{
		private readonly string _filePath;
		private readonly Action<string>? _outputAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileLogger"/> class.
		/// </summary>
		/// <param name="filePath">The path to the file where logs will be written.</param>
		public FileLogger(string filePath, Action<string>? outputAction = null)
		{
			_filePath = filePath;
			_outputAction = outputAction;
		}

		/// <inheritdoc/>
		public IDisposable BeginScope<TState>(TState state)
		{
			return null; // No scope is being used here.
		}

		/// <inheritdoc/>
		public bool IsEnabled(LogLevel logLevel)
		{
			return true; // For simplicity, we're logging all levels. Adjust as needed.
		}

		/// <inheritdoc/>
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (IsEnabled(logLevel) == false)
				return;

			var logRecord = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";

			if (exception != null)
				logRecord += Environment.NewLine + exception;

			File.AppendAllText(_filePath, logRecord + Environment.NewLine);

			// Send to UI
			_outputAction?.Invoke(logRecord);
		}
	}
}
