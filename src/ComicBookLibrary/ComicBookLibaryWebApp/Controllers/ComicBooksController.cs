﻿using ComicBookShared.Models;
using ComicBookShared.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ComicBookLibaryWebApp.ViewModels;
using System.Net;
using System.Data.Entity.Infrastructure;
using System;

namespace ComicBookLibaryWebApp.Controllers
{
    /// <summary>
    /// Controller for the "Comic Books" section of the website.
    /// </summary>
    public class ComicBooksController : BaseController
    {
        private ComicBooksRepository _comicBooksRepository = null;

        public ComicBooksController()
        {
            _comicBooksRepository = new ComicBooksRepository(Context);
        }
        

        public ActionResult Index()
        {
            var comicBooks = _comicBooksRepository.GetList();

            return View(comicBooks);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            // Sort the artists.
            comicBook.Artists = comicBook.Artists.OrderBy(a => a.Role.Name).ToList();

            return View(comicBook);
        }

        public ActionResult Add()
        {
            var viewModel = new ComicBooksAddViewModel();

            viewModel.Init(SeriesRepository, ArtistRepository, RoleRepository);

            return View(viewModel);
        }


        [HttpPost]
        public ActionResult Add(ComicBooksAddViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                var comicBook = viewModel.ComicBook;
                comicBook.AddArtist(viewModel.ArtistId, viewModel.RoleId);
                _comicBooksRepository.Add(comicBook);

                TempData["Message"] = "Your comic book was successfully added!";

                return RedirectToAction("Detail", new { id = comicBook.Id });
            }

            viewModel.Init(SeriesRepository, ArtistRepository, RoleRepository);

            return View(viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var comicBook = _comicBooksRepository.Get((int)id, false);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBooksEditViewModel()
            {
                ComicBook = comicBook
            };

            viewModel.Init(SeriesRepository, ArtistRepository, RoleRepository);
            if (TempData["EditErrors"] != null)
                ModelState.AddModelError(string.Empty , TempData["EditErrors"].ToString());

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(ComicBooksEditViewModel viewModel)
        {
            ValidateComicBook(viewModel.ComicBook);

            if (ModelState.IsValid)
            {
                try
                {
                    _comicBooksRepository.Update(viewModel.ComicBook);

                    TempData["Message"] = "Your comic book was successfully updated!";

                    return RedirectToAction("Detail", new { id = viewModel.ComicBook.Id });
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var entityProperty = ex.Entries.Single().GetDatabaseValues();
                    if (entityProperty != null)
                    {
                        TempData["EditErrors"] =  "Your loaded comic was changed by another user";
                        return RedirectToAction("Edit", new { id = ((ComicBook)entityProperty.ToObject()).Id });
                    }
                    else
                    {
                        TempData["Error"] = "Your changed comic was already delete by some else";
                        return RedirectToAction("Index");
                    }
                    
                }
                
            }

            viewModel.Init(SeriesRepository, ArtistRepository, RoleRepository);

            return View(viewModel);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            
            var comicBook = _comicBooksRepository.Get((int)id);

            if (comicBook == null)
            {
                return HttpNotFound();
            }

            var viewModel = new ComicBookDeleteViewModel()
            {
                ComicBook = comicBook
            };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Delete(ComicBookDeleteViewModel viewModel)
        {

            try
            {
                _comicBooksRepository.Delete(viewModel.Id, viewModel.ComicBook.RowVersion);

                TempData["Message"] = "Your comic book was successfully deleted!";

                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entityProperty = ex.Entries.Single().GetDatabaseValues();
                if (entityProperty != null)
                {
                    TempData["EditErrors"] = "Your loaded comic was changed by another user";
                    return RedirectToAction("Edit", new { id = ((ComicBook)entityProperty.ToObject()).Id });
                }
                else
                {
                    TempData["Error"] = "Your loaded entity was already delete by someone else";
                }

            }

            return View(viewModel);
        }

        /// <summary>
        /// Validates a comic book on the server
        /// before adding a new record or updating an existing record.
        /// </summary>
        /// <param name="comicBook">The comic book to validate.</param>
        private void ValidateComicBook(ComicBook comicBook)
        {
            // If there aren't any "SeriesId" and "IssueNumber" field validation errors...
            if (ModelState.IsValidField("ComicBook.SeriesId") &&
                ModelState.IsValidField("ComicBook.IssueNumber"))
            {
                // Then make sure that the provided issue number is unique for the provided series.
                if (_comicBooksRepository.IsUniqueCombo(comicBook))
                {
                    ModelState.AddModelError("ComicBook.IssueNumber",
                        "The provided Issue Number has already been entered for the selected Series.");
                }
            }
        }
    }
}   