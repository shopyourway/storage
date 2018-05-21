using System;
using System.Reflection;
using Moranbernate.Mapping;

namespace OhioBox.Storage.MySql.Moranbernate
{
	public static class MoranbernateInitializer
	{
		public static void Initialize(params Assembly[] assemblies)
		{
			MappingRepoDictionary.InitializeAssemblies(assemblies);
		}

		public static Type[] GetMappedTypes()
		{
			return MappingRepoDictionary.GetMappedTypes();
		}
	}
}