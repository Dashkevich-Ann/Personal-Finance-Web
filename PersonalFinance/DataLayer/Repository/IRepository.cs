using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        TEntity Create(TEntity entity);

        TEntity Update(TEntity entity);
        
        void Delete(TEntity entity);

        TEntity Find(int id);

        IQueryable<TEntity> Query();
    }
}
