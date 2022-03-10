using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _table;
        private readonly DbContext _context;

        public Repository(DbContext dbContex)
        {
            _context = dbContex;
            _table = dbContex.Set<TEntity>();
        }

        public TEntity Create(TEntity entity)
        {
            return _context.Add(entity).Entity;
        }

        public TEntity Update(TEntity entity)
        {
           return _context.Update(entity).Entity;
        }

        public void Delete(TEntity entity)
        {
            _context.Remove(entity);
        }

        public TEntity Find(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        public IQueryable<TEntity> Query()
        {
            return _context.Set<TEntity>().AsQueryable();
        }
    }
}
