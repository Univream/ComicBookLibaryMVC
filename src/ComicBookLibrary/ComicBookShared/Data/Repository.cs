using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class Repository
    {
        private Context _context = null;

        public Repository(Context context)
        {
            _context = context;
        }

        public IList<Series> GetSeriesList()
        {
            return _context.Series.OrderBy(s => s.Title).ToList();
        }

        public IList<Artist> GetArtistList()
        {
            return _context.Artists.OrderBy(a => a.Name).ToList();
        }

        public IList<Role> GetRolesList()
        {
            return _context.Roles.OrderBy(R => R.Name).ToList();
        }


        public void AddComicBookArtist(ComicBookArtist cba)
        {
            _context.ComicBookArtist.Add(cba);
            _context.SaveChanges();
        }

        public ComicBookArtist GetComicBookArtist(int id)
        {
            return _context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .Where(cba => cba.Id == (int)id)
                .SingleOrDefault();
        }

        public IList<ComicBookArtist> GetComicBookArtists()
        {
            return _context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .ToList();
        }

        public bool DeleteComicBookArtist(int id)
        {
            var comicBookArtist = _context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .Where(cba => cba.Id == id)
                .SingleOrDefault();

            // wrong id
            if (comicBookArtist == null)
                return true;

            _context.ComicBookArtist.Remove(comicBookArtist);
            _context.SaveChanges();
            return false;
        }
    }
}
