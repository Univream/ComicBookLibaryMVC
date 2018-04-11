using ComicBookShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibaryWebApp.Controllers
{
    /// <summary>
    /// Defines a controller with an opening and closing structure for an EF derived Context
    /// </summary>
    public class BaseController : Controller
    {
        protected RoleRepository RoleRepository = null;
        protected ArtistRepository ArtistRepository = null;
        protected SeriesRepository SeriesRepository = null;


        protected Context Context = null;
        private bool _disposed = false;

        public BaseController()
        {
            Context = new Context();
            ArtistRepository = new ArtistRepository(Context);
            SeriesRepository = new SeriesRepository(Context);
            RoleRepository = new RoleRepository(Context);
        }

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Context.Dispose();

            _disposed = true;

            base.Dispose(disposing);

        }

    }
}