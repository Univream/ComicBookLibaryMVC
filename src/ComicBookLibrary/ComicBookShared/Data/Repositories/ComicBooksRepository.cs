using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ComicBooksRepository : BaseRepository<ComicBook>
    {

        public ComicBooksRepository(Context context) 
            : base(context)
        {
        }

        public override IList<ComicBook> GetList()
        {
            return Context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public override ComicBook Get(int id, bool includeRelatedEntities = true)
        {

            var comicbooks = Context.ComicBooks.AsQueryable();
            // TODO Check first filter performance
            // first filter
            comicbooks = comicbooks
                    .Where(cb => cb.Id == id);

            if (includeRelatedEntities)
            {
                // optional includes
                comicbooks = comicbooks
                    .Include(cb => cb.Series)
                    .Include(cb => cb.Artists.Select(a => a.Artist))
                    .Include(cb => cb.Artists.Select(r => r.Role));
            }
            return comicbooks
                    .SingleOrDefault();
        }


        public bool IsUniqueCombo(ComicBook comic)
        {
            return Context.ComicBooks.Any(cb => cb.Id != comic.Id &&
                                cb.SeriesId == comic.SeriesId &&
                                cb.IssueNumber == comic.IssueNumber);
        }

        public bool IsUniqueComboArtist(int ComicId, int ArtistId, int RoleId)
        {
            return Context.ComicBookArtist.Any(cba => cba.ComicBookId == ComicId &&
                        cba.ArtistId == ArtistId &&
                        cba.RoleId == RoleId);
        }

        public void Delete(int id, byte[] rowVersion)
        {
            var comicBook = new ComicBook()
            {
                Id = id,
                RowVersion = rowVersion
            };
            Context.Entry(comicBook).State = EntityState.Deleted;
            Context.SaveChanges();
        }
    }
}
