using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public abstract class BaseRepository<TEntity> 
        where TEntity : class, IEntity, new()
    {
        protected Context Context { get; private set; }

        public BaseRepository(Context context)
        {
            Context = context;
        }


        public abstract TEntity Get(int id, bool includeRelatedEntities = true);


        /// <summary>
        /// Used to Get back all Entities of the corresponding Entity Type
        /// </summary>
        /// <returns></returns>
        public virtual IList<TEntity> GetList()
        {
            return Context.Set<TEntity>().ToList();
        }

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
            IEntity entity = new TEntity()
            {
                Id = id
            };
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();

            return true;
        }
    }
}
