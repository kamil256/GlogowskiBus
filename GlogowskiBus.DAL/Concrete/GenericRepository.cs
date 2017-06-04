using GlogowskiBus.DAL.Abstract;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace GlogowskiBus.DAL.Concrete
{
    public class GenericRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
    {
        private readonly GlogowskiBusContext context;

        public GenericRepository(GlogowskiBusContext context)
        {
            this.context = context;
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter);
            return query;
        }

        public int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();
            if (filter != null)
                query = query.Where(filter);
            return query.Count();
        }

        public virtual TEntity GetById(TKey id)
        {
            return context.Set<TEntity>().Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            context.Set<TEntity>().Attach(entity);
            context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(int id)
        {
            Delete(context.Set<TEntity>().Find(id));
        }

        public virtual void Delete(TEntity entity)
        {
            if (context.Entry(entity).State == EntityState.Deleted)
                context.Set<TEntity>().Attach(entity);
            context.Set<TEntity>().Remove(entity);
        }
    }
}