using System;

namespace OhioBox.Storage.MySql
{
	public interface IPerfLogger<T>
	{
		void Error(string errorText, Exception exception);
		void Warn(string message, string newLine, string stackTrace);
	}
}