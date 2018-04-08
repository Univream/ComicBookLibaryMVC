using ComicBookLibaryWebApp.ViewModels;
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
        private ComicBookArtistRepository _artistRepository = null;
        private ComicBooksRepository _comicBooksRepository = null; 

        public ActionResult Add(int comicBookId)
        {

            var comicBook = _comicBooksRepository.Get(comicBookId);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBookArtistsAddViewModel()
            {
                ComicBook = comicBook
            };

            viewModel.Init(Repository);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Add(ComicBookArtistsAddViewModel viewModel)
        {
            ValidateComicBookArtist(viewModel);

            if (ModelState.IsValid)
            {

                ComicBookArtist comicbookArtist = new ComicBookArtist()
                {
                    ComicBookId = viewModel.ComicBookId,
                    ArtistId = viewModel.ArtistId,
                    RoleId = viewModel.RoleId
                };

                _artistRepository.Add(comicbookArtist);


                TempData["Message"] = "Your artist was successfully added!";

                return RedirectToAction("Detail", "ComicBooks", new { id = viewModel.ComicBookId });
            }

            var comicbook = _comicBooksRepository.Get(viewModel.ComicBookId);

            if (comicbook == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            viewModel.ComicBook = comicbook;
            viewModel.Init(Repository);

            return View(viewModel);
        }

        public ActionResult Delete(int comicBookId, int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ComicBookArtist comicBookArtist = _artistRepository.Get((int)id);

            if (comicBookArtist == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return View(comicBookArtist);
        }

        [HttpPost]
        public ActionResult Delete(int comicBookId, int id)
        {

            if (_artistRepository.Delete(id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            
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
                
                if (_comicBooksRepository.IsUniqueComboArtist(viewModel.ComicBookId, viewModel.ArtistId, viewModel.RoleId))
                {
                    ModelState.AddModelError("ArtistId",
                        "This artist and role combination already exists for this comic book.");
                }
            }
        }
    }
}