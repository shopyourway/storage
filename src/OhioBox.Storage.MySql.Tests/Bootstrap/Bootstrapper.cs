using System.Configuration;
using System.Reflection;
using OhioBox.Storage.MySql.Moranbernate;

namespace OhioBox.Storage.MySql.Tests.Bootstrap
{
	public static class Bootstrapper
	{
		private static ISqlConnectionFactory _factory;

		public static ISqlConnectionFactory Init()
		{
			if (_factory != null) return _factory;

			MoranbernateInitializer.Initialize(typeof(Bootstrapper).GetTypeInfo().Assembly);

			var connectionString = ConfigurationManager.ConnectionStrings["MySql"].ConnectionString;

			_factory = new SqlConnectionFactory(connectionString);

			return _factory;
		}
	}
}