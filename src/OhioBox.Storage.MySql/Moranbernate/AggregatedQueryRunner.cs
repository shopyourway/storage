using System;
using System.Collections.Generic;
using System.Linq;
using OhioBox.Moranbernate.Querying;
using OhioBox.Storage.MySql.Bootstrap;


namespace OhioBox.Storage.MySql.Moranbernate
{
	public interface IAggregatedQueryRunner<T>
		where T : class, new()
	{
		IEnumerable<QueryResult<T>> QueryAggregated(Action<IQueryBuilder<T>> query = null);
	}

	public class AggregatedQueryRunner<T> : IAggregatedQueryRunner<T>
		where T : class, new()
	{
		private readonly ISqlConnectionFactory _connectionFactory;
		private readonly IPerfLogger<T> _perfLogger;
		private readonly IMetricsReporter _metricsReporter;

		private readonly string _space = $"Storage.Moranbernate.{typeof(T).Name}";

		public AggregatedQueryRunner(ISqlConnectionFactory connectionFactory,
			IPerfLogger<T> perfLogger = null,
			IMetricsReporter metricsReporter = null)
		{
			_connectionFactory = connectionFactory;
			_perfLogger = perfLogger ?? new DefaultPerfLogger<T>();
			_metricsReporter = metricsReporter ?? new DefaultMetricsReporter();
		}

		public AggregatedQueryRunner(string connectionString,
			IPerfLogger<T> perfLogger = null,
			IMetricsReporter metricsReporter = null)
		{
			_perfLogger = perfLogger ?? new DefaultPerfLogger<T>();
			_metricsReporter = metricsReporter ?? new DefaultMetricsReporter();
			_connectionFactory = new SqlConnectionFactory(connectionString, _metricsReporter);
		}

		public IEnumerable<QueryResult<T>> QueryAggregated(Action<IQueryBuilder<T>> query = null)
		{
			try
			{
				using (_metricsReporter.Report($"{_space}.Aggregate"))
				using (var connection = _connectionFactory.GetConnection<T>())
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