using System;

namespace OhioBox.Storage.MySql
{
	public interface IMetricsReporter
	{
		IDisposable Report(string key);
		void Report(string key, MetricsReport metric);
	}

	public class MetricsReport
	{
		public long RuntimeInTicks { get; }
		public long? RowCount { get; }
		public long? RequestedItemsCount { get; }

		public MetricsReport(long runtimeInTicks, long? rowCount = null, long? requestedItemsCount = null)
		{
			RuntimeInTicks = runtimeInTicks;
			RowCount = rowCount;
			RequestedItemsCount = requestedItemsCount;
		}
	}
}