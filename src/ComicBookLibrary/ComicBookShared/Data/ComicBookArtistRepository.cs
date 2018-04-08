using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    public class ComicBookArtistRepository
    {
        private Context _context = null;

        public ComicBookArtistRepository(Context context)
        {
            _context = context;
        }


        public ComicBookArtist Get(int id)
        {
            return _context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .Where(cba => cba.Id == (int)id)
                .SingleOrDefault();
        }

        public IList<ComicBookArtist> GetList()
        {
            return _context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .ToList();
        }

        public void Add(ComicBookArtist cba)
        {
            _context.ComicBookArtist.Add(cba);
            _context.SaveChanges();
        }

        public bool Delete(int id)
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
