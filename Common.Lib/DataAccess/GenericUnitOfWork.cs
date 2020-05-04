using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Common.Lib.DataAccess
{
    public class GenericUnitOfWork : IGenericUnitOfWork
    {
        private DbContext _context { get; set; }
        private IDictionary<Type, IUoWRepository> _repos { get; set; } = new Dictionary<Type, IUoWRepository>();

        public GenericUnitOfWork(DbContext context)
        {
            _context = context;
        }

        public IGenericRepository<T> Repo<T>() where T : class
        {
            var type = typeof(T);
            if (!_repos.ContainsKey(type)) _repos.Add(type, new GenericRepository<T>(_context));
            return _repos[type] as GenericRepository<T>;
        }

        public int Commit() => _context.SaveChanges();
    }
}
