using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OhioBox.Storage
{
	public interface IUpdateBuilder<T>
	{
		IUpdateBuilder<T> Set<TValue>(Expression<Func<T, TValue>> field, TValue value);
		IUpdateBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> field, TValue value);
		IUpdateBuilder<T> AddToSet<TValue>(Expression<Func<T, IEnumerable<TValue>>> field, params TValue[] values);
		IUpdateBuilder<T> RemoveFromSet<TValue>(Expression<Func<T, IEnumerable<TValue>>> field, params TValue[] values);
	}
}