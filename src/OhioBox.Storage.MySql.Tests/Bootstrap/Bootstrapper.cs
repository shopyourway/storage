using System.Configuration;
using System.Reflection;
using OhioBox.Storage.MySql.Bootstrap;
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
			
            var factory = new SqlConnectionFactory(new DefaultMetricsReporter());
            factory.Add(connectionString, MoranbernateInitializer.GetMappedTypes());

            _factory = factory;

            return _factory;
        }
    }
}