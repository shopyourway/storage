using System;

namespace OhioBox.Storage.MySql
{
	public interface IMetricsReporter
	{
		IDisposable Report(string key);
		void Report(string key, long runtimeInTicks);
	}
}