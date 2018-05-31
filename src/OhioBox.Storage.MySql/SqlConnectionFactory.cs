using System.Data;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using OhioBox.Storage.MySql.Bootstrap;

namespace OhioBox.Storage.MySql
{
	public class SqlConnectionFactory : ISqlConnectionFactory
	{
		private readonly string _connectionString;
		private readonly IMetricsReporter _metricsReporter;
		private readonly string _schema;

		public SqlConnectionFactory(string connectionString, IMetricsReporter metricsReporter = null)
		{
			_connectionString = connectionString;
			_schema = ParseSchema(connectionString);
			_metricsReporter = metricsReporter ?? new DefaultMetricsReporter();
		}

		public IDbConnection GetConnection<T>()
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
