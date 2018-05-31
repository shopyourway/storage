using System;
using System.Collections.Generic;
using System.Data;

namespace OhioBox.Storage.MySql
{
	public interface ISqlConnectionFactory
	{
		IDbConnection GetConnection<T>();
		ISqlConnectionProvider GetConnectionProvider<T>();
		IEnumerable<ISqlConnectionProvider> Providers { get; }
		IList<KeyValuePair<string, string>> GetTypeFullNameAndAonnectionString();

		void Add(string connectionString, IEnumerable<Type> types);
	}
}