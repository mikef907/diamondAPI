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
            if (!_repos.ContainsKey(typeof(T))) _repos.Add(typeof(T), new GenericRepository<T>(_context));
            return _repos[typeof(T)] as GenericRepository<T>;
        }

        public int Commit() => _context.SaveChanges();
    }
}
