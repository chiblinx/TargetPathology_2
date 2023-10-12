namespace TargetPathology.Core.Logging
{
	/// <summary>
	/// Represents configuration options for the <see cref="FileLogger"/>.
	/// </summary>
	public class FileLoggerOptions
	{
		/// <summary>
		/// Gets or sets the path of the log file.
		/// </summary>
		/// <value>The path of the log file.</value>
		public string? FilePath { get; set; }
	}
}
