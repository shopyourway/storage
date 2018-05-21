using System;
using System.Data;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace OhioBox.Storage.MySql
{
	public interface ISqlConnectionProvider
	{
		IDbConnection ProvideConnection();
	}

	internal class SqlConnectionProvider : ISqlConnectionProvider
	{
		private readonly string _connectionString;
		private readonly IMetricsReporter _metricsReporter;
		private readonly string _schema;

		public SqlConnectionProvider(string connectionString, IMetricsReporter metricsReporter)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException(nameof(connectionString));
			
			_connectionString = connectionString;
			_metricsReporter = metricsReporter;
			_schema = ParseSchema(_connectionString);
		}

		public IDbConnection ProvideConnection()
		{
			using (_metricsReporter.Report($"Storage.OpenConnection.{_schema}"))
			{
				var sqlConnection = new MySqlConnection(_connectionString);
				sqlConnection.Open();
				return sqlConnection;
			}
		}

		private static string ParseSchema(string connectionString)
		{
			try
			{
				return Regex.Match(connectionString, "Database=(.+?);").Groups[1].Value;
			}
			catch
			{
				return "MaorWroteABadRegex";
			}
		}
	}
}