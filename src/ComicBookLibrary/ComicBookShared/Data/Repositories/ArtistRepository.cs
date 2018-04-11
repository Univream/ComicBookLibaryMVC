using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicBookShared.Data
{
    public class ArtistRepository : BaseRepository<Artist>
    {
        public ArtistRepository(Context context) : base(context)
        {}

        public override Artist Get(int id, bool includeRelatedEntities = true)
        {
            var artist = Context.Artists.AsQueryable();
            artist = artist.Where(a => a.Id == id);

            if(includeRelatedEntities)
            {
                artist = artist
                    .Include(a => a.ComicBooks.Select(cb => cb.ComicBook.Series))
                    .Include(a => a.ComicBooks.Select(cb => cb.Role));
            }

            return artist.SingleOrDefault();
        }

        public override IList<Artist> GetList()
        {
            return Context.Artists.OrderBy(a => a.Name).ToList();
        }

        public bool UniqueArtist(Artist artist)
        {
            return Context.Artists.Any(a => a.Id != artist.Id && a.Name == artist.Name);
        }
    }
}
