using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GlogowskiBus.DAL.Abstract
{
    public interface IRepository<TEntity, TKey> where TEntity : class
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null);
        int Count(Expression<Func<TEntity, bool>> filter = null);
        TEntity GetById(TKey id);
        void Insert(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
    }
}
