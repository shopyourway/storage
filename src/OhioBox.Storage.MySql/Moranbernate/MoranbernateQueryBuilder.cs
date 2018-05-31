using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OhioBox.Moranbernate.Mapping;
using OhioBox.Moranbernate.Querying;
using OhioBox.Moranbernate.Querying.Restrictions;
using Querying = OhioBox.Moranbernate.Querying;

namespace OhioBox.Storage.MySql.Moranbernate
{
	internal abstract class MoranbernateQueryBuilderBase<T> : IQueryBuilder<T>
	{
		protected abstract void AddPredicate(Action<IRestrictable<T>> where);

		public IQueryBuilder<T> Equal<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			if (value == null)
				return IsNull(member);

			AddPredicate(w => w.Equal(member, value));
			return this;
		}

		public IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> member, ICollection<TValue> value)
		{
			if (value == null || value.Count == 0)
			{
				AddPredicate(w => w.AddRestriction(StaticRestriction.False));
				return this;
			}

			AddPredicate(w => w.In(member, value));
			return this;
		}

		public IQueryBuilder<T> InAny<TValue>(Expression<Func<T, IList<TValue>>> member, ICollection<TValue> value)
		{
			// No support for collections in mornabernate as of now
			return this;
		}

		public IQueryBuilder<T> NotEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			if (value == null)
				return IsNotNull(member);

			AddPredicate(w => w.NotEqual(member, value));
			return this;
		}

		public IQueryBuilder<T> NotIn<TValue>(Expression<Func<T, TValue>> member, ICollection<TValue> value)
		{
			// Not in a set of empty values means the predicate does not filter anything
			if (value == null || value.Count == 0)
				return this;

			AddPredicate(w => w.NotIn(member, value));
			return this;
		}

		public IQueryBuilder<T> NotInAny<TValue>(Expression<Func<T, IList<TValue>>> member, ICollection<TValue> value)
		{
			// No support for collections in mornabernate as of now
			return this;
		}

		public IQueryBuilder<T> GreaterOrEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			AddPredicate(w => w.GreaterOrEqual(member, value));
			return this;
		}

		public IQueryBuilder<T> GreaterThan<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			AddPredicate(w => w.GreaterThan(member, value));
			return this;
		}

		public IQueryBuilder<T> LessOrEqual<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			AddPredicate(w => w.LessOrEqual(member, value));
			return this;
		}

		public IQueryBuilder<T> LessThan<TValue>(Expression<Func<T, TValue>> member, TValue value)
		{
			AddPredicate(w => w.LessThan(member, value));
			return this;
		}

		public IQueryBuilder<T> IsNull<TValue>(Expression<Func<T, TValue>> member)
		{
			AddPredicate(w => w.IsNull(member));
			return this;
		}

		public IQueryBuilder<T> IsNotNull<TValue>(Expression<Func<T, TValue>> member)
		{
			AddPredicate(w => w.IsNotNull(member));
			return this;
		}

		public IQueryBuilder<T> StartWith(Expression<Func<T, string>> member, string value)
		{
			AddPredicate(w => w.StartWith(member, value));
			return this;
		}

		public IQueryBuilder<T> Contains(Expression<Func<T, string>> member, string value)
		{
			AddPredicate(w => w.Contains(member, value));
			return this;
		}

		public IQueryBuilder<T> FieldExists(Expression<Func<T, object>> member)
		{
			return IsNotNull(member);
		}
		public abstract IQueryBuilder<T> Take(int limit);

		public abstract IQueryBuilder<T> Skip(int amount);

		public abstract IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] fields);

		public abstract IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] fields);

		public abstract IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields);

		public abstract IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields);
	}

	internal class MoranbernateRestrictions<T> : MoranbernateQueryBuilderBase<T>
	{
		private readonly IRestrictable<T> _restrictable;

		public MoranbernateRestrictions(IRestrictable<T> restrictable)
		{
			_restrictable = restrictable;
		}

		protected override void AddPredicate(Action<IRestrictable<T>> @where)
		{
			@where(_restrictable);
		}

		public override IQueryBuilder<T> Take(int limit)
		{
			throw new NotSupportedException();
		}

		public override IQueryBuilder<T> Skip(int amount)
		{
			throw new NotSupportedException();
		}

		public override IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] fields)
		{
			throw new NotSupportedException();
		}

		public override IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] fields)
		{
			throw new NotSupportedException();
		}

		public override IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields)
		{
			throw new NotSupportedException();
		}

		public override IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields)
		{
			throw new NotImplementedException();
		}
	}

	internal class MoranbernateQueryBuilder<T> : MoranbernateQueryBuilderBase<T>
	{
		private readonly Querying.IQueryBuilder<T> _queryBuilder;

		public MoranbernateQueryBuilder(Querying.IQueryBuilder<T> queryBuilder)
		{
			_queryBuilder = queryBuilder;
		}

		protected override void AddPredicate(Action<IRestrictable<T>> where)
		{
			_queryBuilder.Where(where);
		}

		public override IQueryBuilder<T> Take(int limit)
		{
			_queryBuilder.Take(limit);
			return this;
		}

		public override IQueryBuilder<T> Skip(int amount)
		{
			_queryBuilder.Skip(amount);
			return this;
		}

		public override IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] fields)
		{
			_queryBuilder.OrderBy(fields);
			return this;
		}

		public override IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] fields)
		{
			_queryBuilder.OrderByDescending(fields);
			return this;
		}

		public override IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields)
		{
			_queryBuilder.DistinctBy(fields);
			return this;
		}

		public override IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields)
		{
			_queryBuilder.Select(fields);
			return this;
		}
	}

	internal class StaticRestriction : IRestriction
	{
		private readonly string _sql;

		private StaticRestriction(string sql)
		{
			_sql = sql;
		}

		private static readonly StaticRestriction _false = new StaticRestriction("1 = 0");

		internal static StaticRestriction False { get { return _false; } }

		public string Apply(List<object> parameters, IDialect dialect)
		{
			return _sql;
		}
	}
}