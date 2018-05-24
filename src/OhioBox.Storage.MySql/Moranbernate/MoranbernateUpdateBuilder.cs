﻿using System;
using System.Collections.Generic;
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
			_builder.Increment(field, value);
			return this;
		}

		public IUpdateBuilder<T> Decrement<TValue>(Expression<Func<T, TValue>> field, TValue value)
		{
			_builder.Decrement(field, value);
			return this;
		}

		public IUpdateBuilder<T> AddToSet<TValue>(Expression<Func<T, IList<TValue>>> field, params TValue[] values)
		{
			// Not implemented since there is no collection support in moranbernate
			return this;
		}
	}
}