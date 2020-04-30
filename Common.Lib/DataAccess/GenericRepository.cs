using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Common.Lib.DataAccess
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected DbContext Entities { get; set; }
        protected DbSet<T> Set { get; set; }

        public GenericRepository(DbContext context)
        {
            Entities = context;
            Set = Entities.Set<T>();
        }
        public void Reference(T entity, Expression<Func<T, object>> func)
        {
            if (!Entities.Entry(entity).Reference(func).IsLoaded)
                Entities.Entry(entity).Reference(func).Load();
        }

        public void Collection(T entity, Expression<Func<T, IEnumerable<object>>> func)
        {
            if (!Entities.Entry(entity).Collection(func).IsLoaded)
                Entities.Entry(entity).Collection(func).Load();
        }
        
        public T Create(T entity) => Entities.Add(entity).Entity;

        public void Delete(object id) => Entities.Remove(Set.Find(id));
        
        public T Find(object id) => Entities.Find<T>(id);
        
        public IEnumerable<T> Get() => Set.AsEnumerable();
        
        public void Update(T entity)
        {
            Set.Attach(entity);
            Entities.Entry(entity).State = EntityState.Modified;
        }

        public IEnumerable<T> Get(Expression<Func<T, bool>> predicate, bool disableTracking = true)
            => Set.Compile(predicate, null, null, disableTracking);

        public IEnumerable<T> Get(List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includes = null,
                          Expression<Func<T, bool>> predicate = null,
                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                          bool disableTracking = true)
            => Set.Compile(predicate, orderBy, includes, disableTracking);

        public IEnumerable<T> Get(Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                          Expression<Func<T, bool>> predicate = null,
                          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                          bool disableTracking = true)
            => Set.Compile(predicate, orderBy, new List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> { include }, disableTracking);


        public int Count() => Set.Count();
    }

    static class GenericRepositoryExtensions
    {
        public static IQueryable<T> IncludeExt<T>(this IQueryable<T> set, List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includes) where T : class
        {
            includes.ForEach(include => set = include(set));
            return set;
        }

        public static IQueryable<T> IncludeExt<T>(this IQueryable<T> set, Func<IQueryable<T>, IIncludableQueryable<T, object>> includes) where T : class
        {
            set = includes(set);
            return set;
        }

        public static IQueryable<T> Compile<T>(this IQueryable<T> set,
                                  Expression<Func<T, bool>> predicate = null,
                                  Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                  List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includes = null,
                                  bool disableTracking = true) where T: class
        {
            if (disableTracking)
            {
                set = set.AsNoTracking();
            }

            if (includes != null)
            {
                set = set.IncludeExt(includes);
            }

            if (predicate != null)
            {
                set = set.Where(predicate);
            }

            if (orderBy != null)
            {
                set = orderBy(set);
            }

            return set;
        }
    }
}
