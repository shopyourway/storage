using System;

namespace OhioBox.Storage.MySql.Bootstrap
{
	internal class DefaultMetricsReporter : IMetricsReporter
	{
		private class EmptyDisposable : IDisposable
		{
			public void Dispose()
			{
			}
		}

		public IDisposable Report(string key)
		{
			return new EmptyDisposable();
		}

		public void Report(string key, long runtimeInTicks)
		{}
	}
}