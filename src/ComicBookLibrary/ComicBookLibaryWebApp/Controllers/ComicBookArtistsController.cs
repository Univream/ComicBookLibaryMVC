﻿using ComicBookLibaryWebApp.ViewModels;
using ComicBookShared.Data;
using ComicBookShared.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ComicBookLibaryWebApp.Controllers
{
    /// <summary>
    /// Controller for adding/deleting comic book artists.
    /// </summary>
    public class ComicBookArtistsController : BaseController
    {

        public ActionResult Add(int comicBookId)
        {

            var comicBook = Context.ComicBooks
                .Where(cb => cb.Id == comicBookId)
                .Include(cb => cb.Series)
                .SingleOrDefault();

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBookArtistsAddViewModel()
            {
                ComicBook = comicBook
            };

            viewModel.Init(Context);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBookArtistsAddViewModel viewModel)
        {
            ValidateComicBookArtist(viewModel);

            if (ModelState.IsValid)
            {

                ComicBook comicBook = Context.ComicBooks.Find(viewModel.ComicBookId);
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);
                Context.SaveChanges();


                TempData["Message"] = "Your artist was successfully added!";

                return RedirectToAction("Detail", "ComicBooks", new { id = viewModel.ComicBookId });
            }

            var comicbook = Context.ComicBooks
                .Where(cb => cb.Id == viewModel.ComicBookId)
                .Include(cb => cb.Series)
                .SingleOrDefault();

            if (comicbook == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            viewModel.ComicBook = comicbook;
            viewModel.Init(Context);

            return View(viewModel);
        }

        public ActionResult Delete(int comicBookId, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ComicBookArtist comicBookArtist = Context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .Where(cba => cba.Id == (int)id)
                .SingleOrDefault();

            if (comicBookArtist == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(comicBookArtist);
        }

        [HttpPost]
        public ActionResult Delete(int comicBookId, int id)
        {

            ComicBookArtist comicBookArtist = Context.ComicBookArtist
                .Include(cba => cba.Artist)
                .Include(cba => cba.Role)
                .Include(cba => cba.ComicBook.Series)
                .Where(cba => cba.Id == id)
                .SingleOrDefault();

            if (comicBookArtist == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            Context.ComicBookArtist.Remove(comicBookArtist);
            Context.SaveChanges();

            TempData["Message"] = "Your artist was successfully deleted!";

            return RedirectToAction("Detail", "ComicBooks", new { id = comicBookId });
        }

        /// <summary>
        /// Validates a comic book artist on the server
        /// before adding a new record.
        /// </summary>
        /// <param name="viewModel">The view model containing the values to validate.</param>
        private void ValidateComicBookArtist(ComicBookArtistsAddViewModel viewModel)
        {
            // If there aren't any "ArtistId" and "RoleId" field validation errors...
            if (ModelState.IsValidField("ArtistId") &&
                ModelState.IsValidField("RoleId"))
            {
                // Then make sure that this artist and role combination 
                // doesn't already exist for this comic book.
                // TODO Call method to check if this artist and role combination
                // already exists for this comic book.
                if (Context.ComicBookArtist
                    .Any(cba => cba.ComicBookId == viewModel.ComicBookId &&
                        cba.ArtistId == viewModel.ArtistId &&
                        cba.RoleId == viewModel.RoleId
                    ))
                {
                    ModelState.AddModelError("ArtistId",
                        "This artist and role combination already exists for this comic book.");
                }
            }
        }
    }
}