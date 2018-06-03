using System;
using System.Linq;
using System.Reflection;
using OhioBox.Storage.MySql.Moranbernate;

namespace OhioBox.Storage.MySql.Bootstrap
{
	public static class MoranbernateBootstrap
	{
		public static Registration[] GetRegistrationCandidates<TMetricsReporter>(Type perfLoggerType = null) where TMetricsReporter : IMetricsReporter
		{
			perfLoggerType = perfLoggerType ?? typeof(DefaultPerfLogger<>);

			var perfLoggerInterfaceType = typeof(IPerfLogger<>);
			if (!perfLoggerType.GetTypeInfo().GetInterfaces()
				.Any(x => 
					x.GetTypeInfo().IsGenericType && 
					x.GetTypeInfo().GetGenericTypeDefinition() == perfLoggerInterfaceType))
				throw new StorageRegistrationException(perfLoggerInterfaceType, perfLoggerType);

			return new[]
			{
				new Registration(typeof(IAggregatedQueryRunner<>), typeof(AggregatedQueryRunner<>)),
				new Registration(perfLoggerInterfaceType, perfLoggerType),
				new Registration(typeof(IMetricsReporter), typeof(TMetricsReporter))
			};
		}
	}

	public class StorageRegistrationException : Exception
	{
		public StorageRegistrationException(Type @interface, Type type) : 
			base($"Unable to create registrations. {type.Name} does not implement {@interface.Name}")
		{}
	}
}