using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ComicBookArtistRepository : BaseRepository<ComicBookArtist>
    {
        public ComicBookArtistRepository(Context context) 
            : base(context)
        { }

        public override IList<ComicBookArtist> GetList()
        {
            return Context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .ToList();
        }
      

        public override ComicBookArtist Get(int id, bool includeRelatedEntities = true)
        {
            var comicBookArtist = Context.ComicBookArtist.AsQueryable();

            if(includeRelatedEntities)
            {
                comicBookArtist = comicBookArtist
                    .Include(cba => cba.Artist)
                    .Include(cba => cba.Role)
                    .Include(cba => cba.ComicBook.Series);
            }
            
            return comicBookArtist
                .Where(cba => cba.Id == (int)id)
                .SingleOrDefault();
        }
    }
}
