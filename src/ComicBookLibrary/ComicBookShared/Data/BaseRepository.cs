using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public abstract class BaseRepository<TEntity> 
        where TEntity : class
    {
        protected Context Context { get; private set; }

        public BaseRepository(Context context)
        {
            Context = Context;
        }


        public abstract TEntity Get(int id, bool includeRelatedEntities = false);
        public abstract IList<TEntity> GetList();

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public bool Delete(int id)
        {
            TEntity entity = Context.Set<TEntity>().Find(id);
            // no entity was found
            if (entity == null)
                return false;

            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();

            return true;
        }
    }
}
