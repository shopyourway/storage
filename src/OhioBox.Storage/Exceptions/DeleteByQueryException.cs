using System;

namespace OhioBox.Storage.Exceptions
{
	public class DeleteByQueryException : Exception
	{
		public DeleteByQueryException(string message) : base($"Unable to delete by query: {message}")
		{
		}
	}
}