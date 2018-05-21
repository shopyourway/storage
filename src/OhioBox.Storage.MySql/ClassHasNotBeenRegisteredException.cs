using System;

namespace OhioBox.Storage.MySql
{
	internal class ClassHasNotBeenRegisteredException : Exception
	{
		private const string MessageFormat = "No sql connection provider was found for type [{0}], class was not registered using Moranbernate.";

		internal ClassHasNotBeenRegisteredException(Type type)
			: base(string.Format(MessageFormat, type.Name))
		{
		}
	}
}