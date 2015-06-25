using DrumbleApp.Shared.Data.Schema;
using DrumbleApp.Shared.Interfaces;
using System;
using System.Data.Linq;

namespace DrumbleApp.Shared.Data.Repositories
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

        /*public virtual void Insert(TEntity entity)
        {
            dbSet.InsertOnSubmit(entity);
        }*/

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
