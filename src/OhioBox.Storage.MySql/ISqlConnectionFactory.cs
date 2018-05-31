using System;
using System.Collections.Generic;
using System.Data;

namespace OhioBox.Storage.MySql
{
	public interface ISqlConnectionFactory
	{
		IDbConnection GetConnection<T>();
	}
}