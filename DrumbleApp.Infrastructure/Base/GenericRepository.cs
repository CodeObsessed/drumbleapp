using DrumbleApp.Domain.Interfaces;
using DrumbleApp.Infrastructure.DataEntities;
using System;
using System.Data.Linq;

namespace DrumbleApp.Infrastructure.Base
{
    public abstract class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected GenericRepository(CacheContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            this.Context = context;
            this.DbSet = context.GetTable<TEntity>();
        }

        private Table<TEntity> dbSet;
        protected Table<TEntity> DbSet
        {
            get
            {
                return dbSet;
            }
            set
            {
                dbSet = value;
            }
        }

        private CacheContext context;
        protected CacheContext Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
            }
        }

        public virtual void Delete(TEntity entity)
        {
            dbSet.DeleteOnSubmit(entity);
        }

        public virtual void RemoveAll()
        {
            dbSet.DeleteAllOnSubmit(dbSet);
        }
    }
}
