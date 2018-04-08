using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ComicBooksRepository
    {
        private Context _context = null;

        public ComicBooksRepository(Context context)
        {
            _context = context;
        }

        public IList<ComicBook> GetList()
        {
            return _context.ComicBooks
                    .Include(cb => cb.Series)
                    .OrderBy(cb => cb.Series.Title)
                    .ThenBy(cb => cb.IssueNumber)
                    .ToList();
        }

        public ComicBook Get(int id, bool includeRelatedEntities = true)
        {

            var comicbooks = _context.ComicBooks.AsQueryable();
            // TODO Chech first filter speed
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


        public void Add(ComicBook comic)
        {
            _context.ComicBooks.Add(comic);
            _context.SaveChanges();
        }

        public void Update(ComicBook comic)
        {
            _context.Entry(comic).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var comicBook = new ComicBook() { Id = id };
            _context.ComicBooks.Remove(comicBook);
            _context.SaveChanges();
        }

        public bool IsUniqueCombo(ComicBook comic)
        {
            return _context.ComicBooks.Any(cb => cb.Id != comic.Id &&
                                cb.SeriesId == comic.SeriesId &&
                                cb.IssueNumber == comic.IssueNumber);
        }


        public bool IsUniqueComboArtist(int ComicId, int ArtistId, int RoleId)
        {
            return _context.ComicBookArtist.Any(cba => cba.ComicBookId == ComicId &&
                        cba.ArtistId == ArtistId &&
                        cba.RoleId == RoleId);
        }
    }
}
