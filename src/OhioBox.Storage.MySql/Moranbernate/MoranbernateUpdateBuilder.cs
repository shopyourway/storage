using System;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Utils;

namespace OhioBox.Storage.MySql.Moranbernate
{
	public class MoranbernateUpdateBuilder<T> : IUpdateBuilder<T>
	{
		private readonly IKeyValuePairBuilder<T> _builder;

		public MoranbernateUpdateBuilder(IKeyValuePairBuilder<T> builder)
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
			return this;
		}
	}
}