using System;

namespace OhioBox.Storage.MySql.Bootstrap
{
	public class Registration
	{
		public Type Interface { get; }
		public Type Implementor { get; }

		public Registration(Type interfaceType, Type implementor)
		{
			Interface = interfaceType;
			Implementor = implementor;
		}
	}
}