using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Storage.MySql.Moranbernate
{
	public class MoranbernateUpdateBuilder<T> : IUpdateBuilder<T>
	{
		private readonly IUpdateStatementBuilder<T> _builder;

		public MoranbernateUpdateBuilder(IUpdateStatementBuilder<T> builder)
		{
			_builder = builder;
		}

		public IUpdateBuilder<T> Set<TValue>(Expression<Func<T, TValue>> field, TValue value)
		{
			_builder.Set(field, value);
			return this;
		}

		public IUpdateBuilder<T> Increment<TValue>(Expression<Func<T, TValue>> field, TValue value)
		{
			_builder.Increment(field, value);
			return this;
		}

		public IUpdateBuilder<T> AddToSet<TValue>(Expression<Func<T, IEnumerable<TValue>>> field, params TValue[] values)
		{
			// Not implemented since there is no collection support in moranbernate
			return this;
		}

		public IUpdateBuilder<T> RemoveFromSet<TValue>(Expression<Func<T, IEnumerable<TValue>>> field, params TValue[] values)
		{
			// Not implemented since there is no collection support in moranbernate
			return this;
		}
	}
}