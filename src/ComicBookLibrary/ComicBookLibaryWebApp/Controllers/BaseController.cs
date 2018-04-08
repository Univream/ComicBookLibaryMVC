﻿using ComicBookShared.Data;
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
        private Context Context = null;
        private bool _disposed = false;

        protected ComicBooksRepository _comicBooksRepository { get; private set; }
        protected Repository Repository { get; private set; }
        protected ComicBookArtistRepository _artistRepository { get; private set; }

        public BaseController()
        {
            Context = new Context();
            Repository = new Repository(Context);
            _comicBooksRepository = new ComicBooksRepository(Context);
            _artistRepository = new ComicBookArtistRepository(Context); 
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