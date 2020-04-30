using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
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
        void Update(T entity);
        IEnumerable<T> Get();
        IEnumerable<T> Get(Expression<Func<T, bool>> predicate, bool disableTracking = true);
        IEnumerable<T> Get(List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includes = null,
                          Expression<Func<T, bool>> predicate = null,
                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                          bool disableTracking = true);
        IEnumerable<T> Get(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                          Expression<Func<T, bool>> predicate = null,
                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                          bool disableTracking = true);
    }
}