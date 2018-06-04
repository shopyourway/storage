using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OhioBox.Storage
{
	public interface IQueryBuilder<T>
	{
		IQueryBuilder<T> Equal<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> NotEqual<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> In<TValue>(Expression<Func<T, TValue>> member, ICollection<TValue> value);

		IQueryBuilder<T> InAny<TValue>(Expression<Func<T, IEnumerable<TValue>>> member, IEnumerable<TValue> value);

		IQueryBuilder<T> NotIn<TValue>(Expression<Func<T, TValue>> member, ICollection<TValue> value);

		IQueryBuilder<T> NotInAny<TValue>(Expression<Func<T, IEnumerable<TValue>>> member, IEnumerable<TValue> value);

		IQueryBuilder<T> GreaterOrEqual<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> GreaterThan<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> LessOrEqual<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> LessThan<TValue>(Expression<Func<T, TValue>> member, TValue value);

		IQueryBuilder<T> IsNull<TValue>(Expression<Func<T, TValue>> member);

		IQueryBuilder<T> IsNotNull<TValue>(Expression<Func<T, TValue>> member);

		IQueryBuilder<T> Take(int limit);

		IQueryBuilder<T> Skip(int amount);

		IQueryBuilder<T> OrderBy(params Expression<Func<T, object>>[] fields);

		IQueryBuilder<T> OrderByDescending(params Expression<Func<T, object>>[] fields);

		IQueryBuilder<T> DistinctBy(params Expression<Func<T, object>>[] fields);

		IQueryBuilder<T> Select(params Expression<Func<T, object>>[] fields);

		IQueryBuilder<T> StartWith(Expression<Func<T, string>> member, string value);

		IQueryBuilder<T> Contains(Expression<Func<T, string>> member, string value);

		IQueryBuilder<T> RegexMatch(Expression<Func<T, string>> member, string pattern);

		IQueryBuilder<T> RegexMatch(Expression<Func<T, IEnumerable<string>>> member, string pattern);
			
		IQueryBuilder<T> FieldExists(Expression<Func<T, object>> member);

		IQueryBuilder<T> FieldDoesNotExist(Expression<Func<T, object>> member);
	}

	public static class QueryBuilderExt
	{
		public static IQueryBuilder<T> True<T>(this IQueryBuilder<T> target, Expression<Func<T, bool>> member)
		{
			return target.Equal(member, true);
		}

		public static IQueryBuilder<T> False<T>(this IQueryBuilder<T> target, Expression<Func<T, bool>> member)
		{
			return target.Equal(member, false);
		}

		public static IQueryBuilder<T> TakeIfNotNull<T>(this IQueryBuilder<T> target, int? take)
		{
			if (take.HasValue)
				return target.Take(take.Value);

			return target;
		}

		public static IQueryBuilder<T> If<T>(this IQueryBuilder<T> target, bool condition, Func<IQueryBuilder<T>, IQueryBuilder<T>> action)
		{
			return condition ? action(target) : target;
		}

		public static IQueryBuilder<T> If<T>(this IQueryBuilder<T> target, bool condition, Action<IQueryBuilder<T>> action)
		{
			if (condition)
				action(target);

			return target;
		}

		public static IQueryBuilder<T> GetAll<T>(this IQueryBuilder<T> target)
		{
			return target;
		}

		public static IQueryBuilder<T> Between<T, TValue>(this IQueryBuilder<T> target, Expression<Func<T, TValue>> member, TValue fromValue, TValue toValue)
		{
			return target.GreaterOrEqual(member, fromValue)
					.LessOrEqual(member, toValue);
		}
	}
}