using Microsoft.Extensions.Options;

namespace TargetPathology.Core.Logging
{
	/// <summary>
	/// Provides utility methods to handle operations related to the FileLogger.
	/// </summary>
	public static class FileLoggerHelpers
	{
		/// <summary>
		/// Retrieves the path of the current log based on the provided file path options.
		/// </summary>
		/// <param name="options">The options for configuring the logger.</param>
		/// <returns>The computed path of the current log.</returns>
		public static string GetCurrentLogPath(IOptions<FileLoggerOptions> options)
		{
			var filePath = string.IsNullOrEmpty(options.Value.FilePath) ? AppDomain.CurrentDomain.BaseDirectory + "application.log" : options.Value.FilePath;
			return GetCurrentLogPath(filePath);
		}

		/// <summary>
		/// Retrieves the path of the current log based on the provided file path or the default path.
		/// </summary>
		/// <param name="filePath">Optional path to the log file. If not provided, the default is used.</param>
		/// <returns>The computed path of the current log.</returns>
		public static string GetCurrentLogPath(string? filePath = null)
		{
			filePath ??= AppDomain.CurrentDomain.BaseDirectory + "application.log";

			// Handling relative paths
			if (Path.IsPathRooted(filePath) == false)
				filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath);

			// Ensure directory exists before attempting to write logs
			var directory = Path.GetDirectoryName(filePath);

			if (Directory.Exists(directory) == false)
				Directory.CreateDirectory(directory!);

			return Path.Combine(directory, $"{DateTime.Now:yy-MM-dd}-{Path.GetFileName(filePath)}");
		}

		/// <summary>
		/// Exports the log of the current day to a provided destination path.
		/// </summary>
		/// <param name="destinationPath">The path where the current log should be exported.</param>
		public static void ExportCurrentLog(string destinationPath)
		{
			// Ensure the destination directory exists
			var directory = Path.GetDirectoryName(destinationPath);

			if (Directory.Exists(directory) == false)
				Directory.CreateDirectory(directory!);

			// Copy the current log file to the destination
			File.Copy(GetCurrentLogPath(), destinationPath, overwrite: true);
		}
	}
}