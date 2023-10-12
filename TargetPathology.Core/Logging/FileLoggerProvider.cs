using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TargetPathology.Core.Logging
{
	/// <summary>
	/// Provides an instance of <see cref="FileLogger"/> for logging purposes.
	/// </summary>
	public class FileLoggerProvider : ILoggerProvider
	{
		private readonly string _filePath;
		private readonly Action<string> _outputAction;

		/// <summary>
		/// Initializes a new instance of the <see cref="FileLoggerProvider"/> class using the specified options.
		/// </summary>
		/// <param name="options">The options for configuring the logger.</param>
		public FileLoggerProvider(IOptions<FileLoggerOptions> options, Action<string> outputAction)
		{
			_filePath = FileLoggerHelpers.GetCurrentLogPath(options);
			_outputAction = outputAction;
		}

		/// <inheritdoc />
		public ILogger CreateLogger(string categoryName)
		{
			return new FileLogger(_filePath, _outputAction);
		}

		/// <inheritdoc />
		public void Dispose() { }
	}
}
