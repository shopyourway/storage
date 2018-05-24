using System;
using System.Linq.Expressions;

namespace OhioBox.Storage
{
	public interface IUpdateBuilder<T>
	{
		IUpdateBuilder<T> Set<TValue>(Expression<Func<T, TValue>> field, TValue value);
		IUpdateBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> field, TValue value);
	}
}