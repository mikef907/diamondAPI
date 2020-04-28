using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Common.Lib.DataAccess
{
    public interface IGenericRepository<T> : IUoWRepository
        where T : class
    {
        int Count();
        void Reference(T entity, Expression<Func<T, object>> func);
        void Collection(T entity, Expression<Func<T, IEnumerable<object>>> func);
        T Create(T entity);
        void Delete(object id);
        T Find(object id);
        IEnumerable<T> Get();
        void Update(T entity);
        T SingleOrDefault(Func<T, bool> func);
        IEnumerable<T> Where(Func<T, bool> func);
        IEnumerable<T> Where(Expression<Func<T, object>> includes, Func<T, bool> predicate);
        IEnumerable<T> Where(IEnumerable<Expression<Func<T, object>>> includes, Func<T, bool> predicate);
    }
}