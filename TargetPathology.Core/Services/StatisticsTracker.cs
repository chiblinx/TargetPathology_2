using System.ComponentModel;
using TargetPathology.Core.Messaging.Records;

namespace TargetPathology.Core.Services
{
	/// <summary>
	/// Provides a concurrent tracking service for various statistics.
	/// </summary>
	public class StatisticsTracker : INotifyPropertyChanged
	{
		/// <inheritdoc />
		public event PropertyChangedEventHandler? PropertyChanged;

		/// <summary>
		/// Gets the number of records read.
		/// </summary>
		public int RecordsRead => _recordsRead;
		private int _recordsRead = 0;

		/// <summary>
		/// Gets the number of records processed.
		/// </summary>
		public int RecordsProcessed => _recordsProcessed;
		private int _recordsProcessed = 0;

		/// <summary>
		/// Gets the number of <see cref="TestOrderRecord"/> records received.
		/// </summary>
		public int TestOrderRecordsReceived => _testOrderRecordsReceived;
		private int _testOrderRecordsReceived = 0;

		/// <summary>
		/// Gets the number of <see cref="ResultsRecord"/> records received.
		/// </summary>
		public int ResultsRecordsReceived => _resultsRecordsReceived;
		private int _resultsRecordsReceived = 0;

		/// <summary>
		/// Gets the number of records written.
		/// </summary>
		public int RecordsWritten => _recordsWritten;
		private int _recordsWritten = 0;

		/// <summary>
		/// Gets the number of database errors occurred.
		/// </summary>
		public int DatabaseErrors => _databaseErrors;
		private int _databaseErrors = 0;

		/// <summary>
		/// Increments the records read count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementRecordsRead(int value = 1)
		{
			Interlocked.Add(ref _recordsRead, value);
			NotifyPropertyChanged(nameof(RecordsRead));
		}
		
		/// <summary>
		/// Increments the records processed count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementRecordsProcessed(int value = 1)
		{
			Interlocked.Add(ref _recordsProcessed, value);
			NotifyPropertyChanged(nameof(RecordsProcessed));
		}

		/// <summary>
		/// Increments the Test Order records received count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementTestOrderRecordsReceived(int value = 1)
		{
			Interlocked.Add(ref _testOrderRecordsReceived, value);
			NotifyPropertyChanged(nameof(TestOrderRecordsReceived));
		}

		/// <summary>
		/// Increments the Results records received count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementResultsRecordsReceived(int value = 1)
		{
			Interlocked.Add(ref _resultsRecordsReceived, value);
			NotifyPropertyChanged(nameof(ResultsRecordsReceived));
		}

		/// <summary>
		/// Increments the records written count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementRecordsWritten(int value = 1)
		{
			Interlocked.Add(ref _recordsWritten, value);
			NotifyPropertyChanged(nameof(RecordsWritten));
		}

		/// <summary>
		/// Increments the database errors count by a specified value.
		/// </summary>
		/// <param name="value">The value to increment by. Default is 1.</param>
		public void IncrementDatabaseErrors(int value = 1)
		{
			Interlocked.Add(ref _databaseErrors, value);
			NotifyPropertyChanged(nameof(DatabaseErrors));
		}

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}