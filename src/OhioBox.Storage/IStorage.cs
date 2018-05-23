using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OhioBox.Storage
{
	public interface IStorage<T>
		where T : class
	{
		QueryResults<T> RunQuery();

		IList<T> Query(Action<IQueryBuilder<T>> queryManipulator);

		int Count(Action<IQueryBuilder<T>> queryManipulator = null);

		T GetById(object t);
		void Add(T t);
		void Save(T t);
		void Update(T t);
		void Delete(T t);
		void Delete(IList<T> ts);
		void Add(IList<T> ts);
		void Save(IList<T> ts);

		IList<T> GetByField<TKey>(Expression<Func<T, TKey>> selector, TKey key);
		IList<T> GetByField<TKey>(Expression<Func<T, TKey>> selector, ICollection<TKey> ids);

		int UpdateByQuery(Action<IQueryBuilder<T>> query, Action<IUpdateBuilder<T>> update);
	}
}
