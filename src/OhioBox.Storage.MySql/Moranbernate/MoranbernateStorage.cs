﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Utils;
using OhioBox.Storage.MySql.Bootstrap;

namespace OhioBox.Storage.MySql.Moranbernate
{
	public class MoranbernateStorage<T> : IStorage<T>
		where T : class, new()
	{
		private readonly ISqlConnectionFactory _connectionFactory;
		private readonly IPerfLogger<T> _perfLogger;
		private readonly IMetricsReporter _metricsReporter;

		private readonly string _space = $"Storage.Moranbernate.{typeof(T).Name}";
		private const int QueryRuntimeThreshold = 1000;

		public MoranbernateStorage(ISqlConnectionFactory connectionFactory, 
			IPerfLogger<T> perfLogger = null,
			IMetricsReporter metricsReporter = null)
		{
			_connectionFactory = connectionFactory;
			_perfLogger = perfLogger ?? new DefaultPerfLogger<T>();
			_metricsReporter = metricsReporter ?? new DefaultMetricsReporter();
		}

		public int Count(Action<IQueryBuilder<T>> queryManipulator)
		{
			using (_metricsReporter.Report($"{_space}.Count"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				var count = conn.Count<T>(restrictable =>
				{
					queryManipulator?.Invoke(new MoranbernateRestrictions<T>(restrictable));
				});
				return Convert.ToInt32(count);
			}
		}

		public void Delete(IList<T> ts)
		{
			if (ts.Count == 0)
				return;
			using (_metricsReporter.Report($"{_space}.Delete"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				foreach (var t in ts)
					conn.Delete(t);
			}
		}

		public T GetById(object t)
		{
			using (_metricsReporter.Report($"{_space}.GetByid"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				return conn.GetById<T>(t);
			}
		}

		public QueryResults<T> RunQuery()
		{
			var list = QueryInternal(null, "GetAll", null);
			return new QueryResults<T>(list);
		}

		public IList<T> Query(Action<IQueryBuilder<T>> queryManipulator)
		{
			return QueryInternal(q => queryManipulator(new MoranbernateQueryBuilder<T>(q)), "RunQuery", null);
		}

		public void Add(T t)
		{
			using (_metricsReporter.Report($"{_space}.Add"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.Insert(t);
			}
		}

		public void Update(T t)
		{
			using (_metricsReporter.Report($"{_space}.Update"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.Update(t);
			}
		}

		public void Delete(T t)
		{
			using (_metricsReporter.Report($"{_space}.Delete"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.Delete(t);
			}
		}

		public void Save(T t)
		{
			using (_metricsReporter.Report($"{_space}.Save"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.Upsert(t);
			}
		}

		public IList<T> GetByField<TKey>(Expression<Func<T, TKey>> selector, TKey key)
		{
			return QueryInternal(q => q.Where(w => w.Equal(selector, key)), "GetByField", () => " by field: " + ExpressionProcessor.FindMemberExpression(selector.Body) + " = " + key);
		}

		public IList<T> GetByField<TKey>(Expression<Func<T, TKey>> selector, ICollection<TKey> ids)
		{
			if (ids.Count == 0)
				return new List<T>();

			return QueryInternal(q => q.Where(w => w.In(selector, ids)), "GetByField", () => " by field: " + ExpressionProcessor.FindMemberExpression(selector.Body) + " with " + ids.Count + " parameters");
		}

		public int UpdateByQuery(Action<IQueryBuilder<T>> query, Action<IUpdateBuilder<T>> update)
		{
			return ExecuteQuery(conn =>
			{
				var rows = conn.UpdateByQuery<T>(
					builder => update(new MoranbernateUpdateBuilder<T>(builder)),
					restrictable => query(new MoranbernateRestrictions<T>(restrictable)));

				return (rows, rows);
			}, "UpdateByQuery");
		}

		public int DeleteByQuery(Action<IQueryBuilder<T>> query)
		{
			return ExecuteQuery(conn =>
				{
					var rows = conn.DeleteByQuery<T>(restrictable => query(new MoranbernateRestrictions<T>(restrictable)));
					return (rows, rows);
				}, 
				"DeleteByQuery");
		}

		private List<T> QueryInternal(Action<global::OhioBox.Moranbernate.Querying.IQueryBuilder<T>> action, string metricsKey, Func<string> logMessage)
		{
			return ExecuteQuery(conn =>
			{
				var list = conn.Query(action).ToList();
				return (list, list.Count);
			}, metricsKey, logMessage);
		}

		private TR ExecuteQuery<TR>(Func<IDbConnection, (TR result, int rows)> action, string metricsKey, Func<string> logMessage = null)
		{
			var sw = Stopwatch.StartNew();
			var rows = 0;
			try
			{
				using (var conn = _connectionFactory.GetConnection<T>())
				{
					var result = action(conn);
					rows = result.rows;
					return result.result;
				}
			}
			finally
			{
				sw.Stop();
				_metricsReporter.Report($"{_space}.{metricsKey}", sw.Elapsed.Ticks);
				if (sw.ElapsedMilliseconds > QueryRuntimeThreshold)
					LogSlowQuery(sw.ElapsedMilliseconds, rows, logMessage?.Invoke());
			}
		}

		private void LogSlowQuery(long elapsedMilliseconds, int rowsReturned, string message = null)
		{
			message = "Slow query: " + typeof(T).Name + " - " + elapsedMilliseconds + "ms in total, rows returned: " + rowsReturned + ", " + (message ?? "");
			_perfLogger.Warn(message, Environment.NewLine, Environment.StackTrace);
		}

		public void Add(IList<T> ts)
		{
			using (_metricsReporter.Report($"{_space}.BulkInsert"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.BulkInsert(ts);
			}
		}

		public void Save(IList<T> ts)
		{
			using (_metricsReporter.Report($"{_space}.BulkUpsert"))
			using (var conn = _connectionFactory.GetConnection<T>())
			{
				conn.BulkUpsert(ts);
			}
		}
	}
}