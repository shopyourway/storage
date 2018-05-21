using System.Data;

namespace OhioBox.Storage.MySql
{
	public static class SqlConnectionProviderExtension
	{
		public static IDbConnection GetOpenConnection(this ISqlConnectionProvider provider)
		{
			var connection = provider.ProvideConnection();
			return connection;
		}
	}
}