using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class RoleRepository : BaseRepository<Role>
    {
        public RoleRepository(Context context) : base(context)
        { }

        public override Role Get(int id, bool includeRelatedEntities = true)
        {
            var role = Context.Roles.AsQueryable();
            return role.Where(r => r.Id == id).SingleOrDefault();
        }
    }
}
