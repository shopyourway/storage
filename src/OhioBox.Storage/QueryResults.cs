using System.Collections.Generic;

namespace OhioBox.Storage
{
	public class QueryResults<T>
	{
		public QueryResults(IList<T> results)
		{
			Results = results;
		}

		public IList<T> Results { get; private set; }
	}
}