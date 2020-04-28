using Microsoft.EntityFrameworkCore;
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
        public T SingleOrDefault(Func<T, bool> func) => Set.SingleOrDefault(func);
        public IEnumerable<T> Where(Func<T, bool> func) => Set.Where(func);

        public IEnumerable<T> Where(Expression<Func<T, object>> includes, Func<T, bool> predicate) => Set.Include(includes).Where(predicate);

        public IEnumerable<T> Where(IEnumerable<Expression<Func<T, object>>> includes, Func<T, bool> predicate)
        {
            IQueryable<T> q = Set.AsQueryable();
            includes.ToList().ForEach(f => q = q.Include(f));
            return q.Where(predicate);
        }


        public int Count() => Set.Count();
    }
}
