using System;

namespace OhioBox.Storage.Exceptions
{
	public class UpdateByQueryException : Exception
	{
		public UpdateByQueryException(string message) : base($"Unable to update by query: {message}")
		{
		}
	}
}