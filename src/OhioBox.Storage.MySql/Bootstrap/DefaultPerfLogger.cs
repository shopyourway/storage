using System;
using OhioBox.Storage.MySql.Moranbernate;

namespace OhioBox.Storage.MySql.Bootstrap
{
	internal class DefaultPerfLogger<T> : IPerfLogger<T>
	{
		public void Error(string errorText, Exception exception)
		{
			
		}

		public void Warn(string message, string newLine, string stackTrace)
		{
			
		}
	}
}