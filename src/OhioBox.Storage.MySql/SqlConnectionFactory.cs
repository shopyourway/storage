using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace OhioBox.Storage.MySql
{
	internal class SqlConnectionFactory : ISqlConnectionFactory
	{
		private readonly IMetricsReporter _metricsReporter;
		private readonly Dictionary<Type, ISqlConnectionProvider> _typeToConnection = new Dictionary<Type, ISqlConnectionProvider>();
		private readonly List<ISqlConnectionProvider> _providers = new List<ISqlConnectionProvider>();		

		public IEnumerable<ISqlConnectionProvider> Providers => _providers;

		public SqlConnectionFactory(IMetricsReporter metricsReporter)
		{
			_metricsReporter = metricsReporter;
		}

		public IList<KeyValuePair<string, string>> GetTypeFullNameAndAonnectionString()
		{
			return _typeToConnection.Select(connectionProvider => new KeyValuePair<string, string>(connectionProvider.Key.FullName, connectionProvider.Value.ProvideConnection().ConnectionString)).ToList();
		}

		public void Add(string connectionString, IEnumerable<Type> types)
		{
			var provider = new SqlConnectionProvider(connectionString, _metricsReporter);

			_providers.Add(provider);

			foreach (var type in types)
			{
				_typeToConnection.Add(type, provider);
			}
		}

		public IDbConnection GetConnection<T>()
		{
			var provider = InnerGetConnectionProvider<T>();

			var connection = provider.ProvideConnection();
			
			return connection;
		}

		public ISqlConnectionProvider GetConnectionProvider<T>()
		{
			return InnerGetConnectionProvider<T>();
		}

		private ISqlConnectionProvider InnerGetConnectionProvider<T>()
		{
			var type = typeof(T);
			var provider = _typeToConnection.GetItemOrDefault(type);

			if (provider == null)
				throw new ClassHasNotBeenRegisteredException(type);

			return provider;
		}
	}
}
