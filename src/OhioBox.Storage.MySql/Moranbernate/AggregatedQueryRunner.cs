using System;
using System.Collections.Generic;
using System.Linq;
using Moranbernate.Querying;


namespace OhioBox.Storage.MySql.Moranbernate
{
	public interface IAggregatedQueryRunner<T>
		where T : class, new()
	{
		IEnumerable<QueryResult<T>> QueryAggregated(Action<IQueryBuilder<T>> query = null);
	}

	internal class AggregatedQueryRunner<T> : IAggregatedQueryRunner<T>
		where T : class, new()
	{
		private readonly ISqlConnectionProvider _connectionProvider;
		private readonly IPerfLogger<T> _perfLogger;
		private readonly IMetricsReporter _metricsReporter;

		private readonly string _space = $"Storage.Moranbernate.{typeof(T).Name}";

		public AggregatedQueryRunner(ISqlConnectionFactory connectionFactory,
			IPerfLogger<T> perfLogger,
			IMetricsReporter metricsReporter)
		{
			_perfLogger = perfLogger;
			_metricsReporter = metricsReporter;
			_connectionProvider = connectionFactory.GetConnectionProvider<T>();
		}

		public IEnumerable<QueryResult<T>> QueryAggregated(Action<IQueryBuilder<T>> query = null)
		{
			try
			{
				using (_metricsReporter.Report($"{_space}.Aggregate"))
				using (var connection = _connectionProvider.GetOpenConnection())
				{
					return connection
						.QueryAggregated<T>(q =>
						{
							if (query != null)
								query(new MoranbernateQueryBuilder<T>(q));
						})
						.Select(x => new QueryResult<T> { Item = x.Item, RowCount = x.RowCount })
						.ToList();
				}
			}
			catch (Exception ex)
			{
				_perfLogger.Error("Error running aggregated query", ex);
				throw;
			}
		}
	}

	public class QueryResult<T>
	{
		public T Item { get; set; }
		public int RowCount { get; set; }
	}
}