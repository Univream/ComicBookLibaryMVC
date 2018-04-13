using ComicBookLibraryManager.Helpers;
using ComicBookShared.Models;
using ComicBookShared.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ComicBookLibraryManager
{
    class Program
    {
        private static Context context { get { return new Context(); } }

        private static ArtistRepository _artistRepository = null;
        private static SeriesRepository _seriesRepository = null;
        private static RoleRepository _roleRepository = null;
        private static ComicBooksRepository _comicBookRepository = null;
        private static ComicBookArtistRepository _comicBookArtistRepository = null;

        // These are the various commands that can be performed 
        // in the app. Each command must have a unique string value.
        const string CommandListComicBooks = "l";
        const string CommandListComicBook = "i";
        const string CommandListComicBookProperties = "p";
        const string CommandAddComicBook = "a";
        const string CommandUpdateComicBook = "u";
        const string CommandDeleteComicBook = "d";
        const string CommandSave = "s";
        const string CommandCancel = "c";
        const string CommandArtist = "artist"; 
        const string CommandQuit = "q";

        // A collection of the comic book editable properties.
        // This collection of property names needs to match the list 
        // of the properties listed on the Comic Book Detail screen.
        readonly static List<string> EditableProperties = new List<string>()
        {
            "SeriesId",
            "IssueNumber",
            "Description",
            "PublishedOn",
            "AverageRating",
            "Artists"
        };

        static void Main(string[] args)
        {
            _comicBookRepository = new ComicBooksRepository(context);
            string command = CommandListComicBooks;
            IList<int> comicBookIds = null;

            // If the current command doesn't equal the "Quit" command 
            // then evaluate and process the command.
            while (command != CommandQuit)
            {
                switch (command)
                {
                    case CommandListComicBooks:
                        comicBookIds = ListComicBooks();
                        break;
                    case CommandAddComicBook:
                        AddComicBook();
                        command = CommandListComicBooks;
                        continue;
                    case CommandArtist:
                        GetAllArtist();
                        break;
                    default:
                        if (AttemptDisplayComicBook(command, comicBookIds))
                        {
                            command = CommandListComicBooks;
                            continue;
                        }
                        else
                        {
                            ConsoleHelper.OutputLine("Sorry, but I didn't understand that command.");
                        }
                        break;
                }

                // List the available commands.
                ConsoleHelper.OutputBlankLine();
                ConsoleHelper.Output("Commands: ");
                int comicBookCount = _comicBookRepository.GetList().Count;
                if (comicBookCount > 0)
                {
                    ConsoleHelper.Output("Enter a Number 1-{0}, ", comicBookCount);
                }
                ConsoleHelper.OutputLine("A - Add, Q - Quit", false);

                // Get the next command from the user.
                command = ConsoleHelper.ReadInput("Enter a Command: ", true);
                context.Dispose();
            }
        }

        /// <summary>
        /// Attempts to parse the provided command as a line number
        /// and if successful, displays the Comic Book Detail screen
        /// for the referenced comic book.
        /// </summary>
        /// <param name="command">The line number command.</param>
        /// <param name="comicBookIds">The collection of comic book IDs for the displayed comic book list.</param>
        /// <returns>Returns "true" if the comic book was successfully displayed, otherwise "false".</returns>
        private static bool AttemptDisplayComicBook(
            string command, IList<int> comicBookIds)
        {
            var successful = false;
            int? comicBookId = null;

            // Only attempt to parse the command to a line number 
            // if we have a collection of comic book IDs available.
            if (comicBookIds != null)
            {
                // Attempt to parse the command to a line number.
                int lineNumber = 0;
                int.TryParse(command, out lineNumber);

                // If the number is within range then get that comic book ID.
                if (lineNumber > 0 && lineNumber <= comicBookIds.Count)
                {
                    comicBookId = comicBookIds[lineNumber - 1];
                    successful = true;
                }
            }

            // If we have a comic book ID, then display the comic book.
            if (comicBookId != null)
            {
                DisplayComicBook(comicBookId.Value);
            }

            return successful;
        }

        /// <summary>
        /// Prompts the user for the comic book values to add 
        /// and adds the comic book to the database.
        /// </summary>
        private static void AddComicBook()
        {
            _comicBookRepository = new ComicBooksRepository(context);
            ConsoleHelper.ClearOutput();
            ConsoleHelper.OutputLine("ADD COMIC BOOK");

            // Get the comic book values from the user.

            var comicBook = new ComicBook();
            comicBook.SeriesId = GetSeriesId();
            comicBook.IssueNumber = GetIssueNumber();
            comicBook.Description = GetDescription();
            comicBook.PublishedOn = GetPublishedOnDate();
            comicBook.AverageRating = GetAverageRating();

            var comicBookArist = new ComicBookArtist();
            comicBookArist.ArtistId = GetArtistId();
            comicBookArist.RoleId = GetRoleId();
            comicBook.Artists.Add(comicBookArist);

            // Add the comic book to the database.
            _comicBookRepository.Add(comicBook);
            context.Dispose();
        }

        /// <summary>
        /// Gets the series ID from the user.
        /// </summary>
        /// <returns>Returns an integer for the selected series ID.</returns>
        private static int GetSeriesId()
        {
            _seriesRepository = new SeriesRepository(context);
            int? seriesId = null;
            IList<Series> series = _seriesRepository.GetList();

            // While the series ID is null, prompt the user to select a series ID 
            // from the provided list.
            while (seriesId == null)
            {
                ConsoleHelper.OutputBlankLine();

                foreach (Series s in series)
                {
                    ConsoleHelper.OutputLine("{0}) {1}", series.IndexOf(s) + 1, s.Title);
                }

                // Get the line number for the selected series.
                string lineNumberInput = ConsoleHelper.ReadInput(
                    "Enter the line number of the series that you want to add a comic book to: ");

                // Attempt to parse the user's input to a line number.
                int lineNumber = 0;
                if (int.TryParse(lineNumberInput, out lineNumber))
                {
                    if (lineNumber > 0 && lineNumber <= series.Count)
                    {
                        seriesId = series[lineNumber - 1].Id;
                    }
                }

                // If we weren't able to parse the provided line number 
                // to a series ID then display an error message.
                if (seriesId == null)
                {
                    ConsoleHelper.OutputLine("Sorry, but that wasn't a valid line number.");
                }
            }

            context.Dispose();
            return seriesId.Value;
        }

        /// <summary>
        /// Gets the artist ID from the user.
        /// </summary>
        /// <returns>Returns an integer for the selected artist ID.</returns>
        private static int GetArtistId()
        {
            _artistRepository = new ArtistRepository(context);
            int? artistId = null;
            IList<Artist> artists = _artistRepository.GetList();

            // While the artist ID is null, prompt the user to select a artist ID 
            // from the provided list.
            while (artistId == null)
            {
                ConsoleHelper.OutputBlankLine();

                foreach (Artist a in artists)
                {
                    ConsoleHelper.OutputLine("{0}) {1}", artists.IndexOf(a) + 1, a.Name);
                }

                // Get the line number for the selected artist.
                string lineNumberInput = ConsoleHelper.ReadInput(
                    "Enter the line number of the artist that you want to add to this comic book: ");

                // Attempt to parse the user's input to a line number.
                int lineNumber = 0;
                if (int.TryParse(lineNumberInput, out lineNumber))
                {
                    if (lineNumber > 0 && lineNumber <= artists.Count)
                    {
                        artistId = artists[lineNumber - 1].Id;
                    }
                }

                // If we weren't able to parse the provided line number 
                // to an artist ID then display an error message.
                if (artistId == null)
                {
                    ConsoleHelper.OutputLine("Sorry, but that wasn't a valid line number.");
                }
            }

            context.Dispose();
            return artistId.Value;
        }

        /// <summary>
        /// Gets the role ID from the user.
        /// </summary>
        /// <returns>Returns an integer for the selected role ID.</returns>
        private static int GetRoleId()
        {
            _roleRepository = new RoleRepository(context);
            int? roleId = null;
            IList<Role> roles = _roleRepository.GetList();

            // While the role ID is null, prompt the user to select a role ID 
            // from the provided list.
            while (roleId == null)
            {
                ConsoleHelper.OutputBlankLine();

                foreach (Role r in roles)
                {
                    ConsoleHelper.OutputLine("{0}) {1}", roles.IndexOf(r) + 1, r.Name);
                }

                // Get the line number for the selected role.
                string lineNumberInput = ConsoleHelper.ReadInput(
                    "Enter the line number of the role that the artist had on this comic book: ");

                // Attempt to parse the user's input to a line number.
                int lineNumber = 0;
                if (int.TryParse(lineNumberInput, out lineNumber))
                {
                    if (lineNumber > 0 && lineNumber <= roles.Count)
                    {
                        roleId = roles[lineNumber - 1].Id;
                    }
                }

                // If we weren't able to parse the provided line number 
                // to an role ID then display an error message.
                if (roleId == null)
                {
                    ConsoleHelper.OutputLine("Sorry, but that wasn't a valid line number.");
                }
            }

            context.Dispose();
            return roleId.Value;
        }

        /// <summary>
        /// Gets the issue number from the user.
        /// </summary>
        /// <returns>Returns an integer for the provided issue number.</returns>
        private static int GetIssueNumber()
        {
            int issueNumber = 0;

            // While the issue number is less than or equal to "0", 
            // prompt the user to provide an issue number.
            while (issueNumber <= 0)
            {
                // Get the issue number from the user.
                string issueNumberInput = ConsoleHelper.ReadInput(
                    "Enter an issue number: ");

                // Attempt to parse the user's input to an integer.
                int.TryParse(issueNumberInput, out issueNumber);

                // If we weren't able to parse the provided issue number 
                // to an integer then display an error message.
                if (issueNumber <= 0)
                {
                    ConsoleHelper.OutputLine("Sorry, but that wasn't a valid issue number.");
                }
            }

            return issueNumber;
        }

        /// <summary>
        /// Gets the description from the user.
        /// </summary>
        /// <returns>Returns a string for the provided description.</returns>
        private static string GetDescription()
        {
            return ConsoleHelper.ReadInput(
                "Enter the description: ");
        }

        /// <summary>
        /// Gets the published on date from the user.
        /// </summary>
        /// <returns>Returns a date/time for the provided published on date.</returns>
        private static DateTime GetPublishedOnDate()
        {
            DateTime publishedOnDate = DateTime.MinValue;

            // While the published on date equals the minimum date/time value, 
            // prompt the user to provide a published on date.
            while (publishedOnDate == DateTime.MinValue)
            {
                // Get the published on date from the user.
                string publishedOnDateInput = ConsoleHelper.ReadInput(
                    "Enter the date this comic book was published on: ");

                // Attempt to parse the user's input to a date/time.
                DateTime.TryParse(publishedOnDateInput, out publishedOnDate);

                // If we weren't able to parse the provided published on date 
                // to a date/time then display an error message.
                if (publishedOnDate == DateTime.MinValue)
                {
                    ConsoleHelper.OutputLine("Sorry, but that wasn't a valid date.");
                }
            }

            return publishedOnDate;
        }

        /// <summary>
        /// Gets the average rating from the user.
        /// </summary>
        /// <returns>Returns a decimal for the provided average rating.</returns>
        private static decimal? GetAverageRating()
        {
            decimal? averageRating = null;
            var promptUser = true;

            // Continue to prompt the user for an average rating 
            // until we get a valid value or an empty value (the 
            // average rating is not a required value).
            while (promptUser)
            {
                // Get the average rating from the user.
                string averageRatingInput = ConsoleHelper.ReadInput(
                    "Enter the average rating for this comic book: ");

                // Did we get a value from the user?
                if (!string.IsNullOrWhiteSpace(averageRatingInput))
                {
                    // Attempt to parse the user's input to a decimal.
                    decimal averageRatingValue;
                    if (decimal.TryParse(averageRatingInput, out averageRatingValue))
                    {
                        averageRating = averageRatingValue;

                        // If we were able to parse the provided average rating 
                        // then set the prompt user flag to "false" so we 
                        // break out of the while loop.
                        promptUser = false;
                    }
                    else
                    {
                        // If we weren't able to parse the provided average rating 
                        // to a decimal then display an error message.
                        ConsoleHelper.OutputLine("Sorry, but that wasn't a valid number.");
                    }
                }
                else
                {
                    // If we didn't get a value from the user 
                    // then set the prompt user flag to "false" 
                    // so we break out of the while loop.
                    promptUser = false;
                }
            }

            return averageRating;
        }

        /// <summary>
        /// Retrieves the comic books from the database and lists
        /// them to the screen.
        /// </summary>
        /// <returns>A collection of the comic book IDs in the same order 
        /// as the comic books were listed to facilitate selecting a comic book 
        /// by its line number.</returns>
        private static IList<int> ListComicBooks()
        {
            _comicBookRepository = new ComicBooksRepository(context);
            var comicBookIds = new List<int>();
            IList<ComicBook> comicBooks = _comicBookRepository.GetList();

            ConsoleHelper.ClearOutput();
            ConsoleHelper.OutputLine("COMIC BOOKS");

            ConsoleHelper.OutputBlankLine();

            foreach (ComicBook comicBook in comicBooks)
            {
                comicBookIds.Add(comicBook.Id);

                ConsoleHelper.OutputLine("{0}) {1}",
                    comicBooks.IndexOf(comicBook) + 1,
                    comicBook.DisplayText);
            }

            context.Dispose();
            return comicBookIds;
        }

        /// <summary>
        /// Displays the comic book detail for the provided comic book ID.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to display detail for.</param>
        private static void DisplayComicBook(int comicBookId)
        {
            string command = CommandListComicBook;

            // If the current command doesn't equal the "Cancel" command 
            // then evaluate and process the command.
            while (command != CommandCancel)
            {
                switch (command)
                {
                    case CommandListComicBook:
                        ListComicBook(comicBookId);
                        break;
                    case CommandUpdateComicBook:
                        UpdateComicBook(comicBookId);
                        command = CommandListComicBook;
                        continue;
                    case CommandDeleteComicBook:
                        if (DeleteComicBook(comicBookId))
                        {
                            command = CommandCancel;
                        }
                        else
                        {
                            command = CommandListComicBook;
                        }
                        continue;
                    default:
                        ConsoleHelper.OutputLine("Sorry, but I didn't understand that command.");
                        break;
                }

                // List the available commands.
                ConsoleHelper.OutputBlankLine();
                ConsoleHelper.Output("Commands: ");
                ConsoleHelper.OutputLine("U - Update, D - Delete, C - Cancel", false);

                // Get the next command from the user.
                command = ConsoleHelper.ReadInput("Enter a Command: ", true);
            }
        }

        /// <summary>
        /// Confirms that the user wants to delete the comic book 
        /// for the provided comic book ID and if so, deletes the 
        /// comic book from the database.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to delete.</param>
        /// <returns>Returns "true" if the comic book was deleted, otherwise "false".</returns>
        private static bool DeleteComicBook(int comicBookId)
        {
            _comicBookRepository = new ComicBooksRepository(context);
            var successful = false;

            // Prompt the user if they want to continue with deleting this comic book.
            string input = ConsoleHelper.ReadInput(
                "Are you sure you want to delete this comic book (Y/N)? ", true);

            // If the user entered "y", then delete the comic book.
            if (input == "y")
            {
                _comicBookRepository.Delete(comicBookId);
                successful = true;
            }

            context.Dispose();
            return successful;
        }

        /// <summary>
        /// Lists the detail for the provided comic book ID.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to list detail for.</param>
        private static void ListComicBook(int comicBookId)
        {
            _comicBookRepository = new ComicBooksRepository(context);
            ComicBook comicBook = _comicBookRepository.Get(comicBookId);

            ConsoleHelper.ClearOutput();
            ConsoleHelper.OutputLine("COMIC BOOK DETAIL");

            ConsoleHelper.OutputLine(comicBook.DisplayText);

            if (!string.IsNullOrWhiteSpace(comicBook.Description))
            {
                ConsoleHelper.OutputLine(comicBook.Description);
            }

            ConsoleHelper.OutputBlankLine();
            ConsoleHelper.OutputLine("Published On: {0}", comicBook.PublishedOn.ToShortDateString());
            ConsoleHelper.OutputLine("Average Rating: {0}",
                comicBook.AverageRating != null ?
                comicBook.AverageRating.Value.ToString("n2") : "N/A");

            ConsoleHelper.OutputLine("Artists");

            foreach (ComicBookArtist artist in comicBook.Artists)
            {
                ConsoleHelper.OutputLine("{0} - {1}", artist.Artist.Name, artist.Role.Name);
            }
            context.Dispose();
        }

        /// <summary>
        /// Lists the editable properties for the provided comic book ID 
        /// and prompts the user to select a property to edit.
        /// </summary>
        /// <param name="comicBookId">The comic book ID to update.</param>
        private static void UpdateComicBook(int comicBookId)
        {
            _comicBookRepository = new ComicBooksRepository(context);
            ComicBook comicBook = _comicBookRepository.Get(comicBookId);

            string command = CommandListComicBookProperties;

            // If the current command doesn't equal the "Cancel" command 
            // then evaluate and process the command.
            while (command != CommandCancel)
            {
                switch (command)
                {
                    case CommandListComicBookProperties:
                        ListComicBookProperties(comicBook);
                        break;
                    case CommandSave:
                        _comicBookRepository.Update(comicBook);
                        command = CommandCancel;
                        continue;
                    default:
                        if (AttemptUpdateComicBookProperty(command, comicBook))
                        {
                            command = CommandListComicBookProperties;
                            continue;
                        }
                        else
                        {
                            ConsoleHelper.OutputLine("Sorry, but I didn't understand that command.");
                        }
                        break;
                }

                // List the available commands.
                ConsoleHelper.OutputBlankLine();
                ConsoleHelper.Output("Commands: ");
                if (EditableProperties.Count > 0)
                {
                    ConsoleHelper.Output("Enter a Number 1-{0}, ", EditableProperties.Count);
                }
                ConsoleHelper.OutputLine("S - Save, C - Cancel", false);

                // Get the next command from the user.
                command = ConsoleHelper.ReadInput("Enter a Command: ", true);
            }

            context.Dispose();
            ConsoleHelper.ClearOutput();
        }

        /// <summary>
        /// Attempts to parse the provided command as a line number 
        /// and if successful, calls the appropriate user input method 
        /// for the selected comic book property.
        /// </summary>
        /// <param name="command">The line number command.</param>
        /// <param name="comicBook">The comic book to update.</param>
        /// <returns>Returns "true" if the comic book property was successfully updated, otherwise "false".</returns>
        private static bool AttemptUpdateComicBookProperty(
            string command, ComicBook comicBook)
        {
            _seriesRepository = new SeriesRepository(context);
            var successful = false;

            // Attempt to parse the command to a line number.
            int lineNumber = 0;
            int.TryParse(command, out lineNumber);

            // If the number is within range then get that comic book ID.
            if (lineNumber > 0 && lineNumber <= EditableProperties.Count)
            {
                // Retrieve the property name for the provided line number.
                string propertyName = EditableProperties[lineNumber - 1];

                // Switch on the provided property name and call the 
                // associated user input method for that property name.
                switch (propertyName)
                {
                    case "SeriesId":
                        comicBook.SeriesId = GetSeriesId();
                        comicBook.Series = _seriesRepository.Get(comicBook.SeriesId);
                        break;
                    case "IssueNumber":
                        comicBook.IssueNumber = GetIssueNumber();
                        break;
                    case "Description":
                        comicBook.Description = GetDescription();
                        break;
                    case "PublishedOn":
                        comicBook.PublishedOn = GetPublishedOnDate();
                        break;
                    case "AverageRating":
                        comicBook.AverageRating = GetAverageRating();
                        break;
                    case "Artists":
                        ChooseAddArtist(comicBook);
                        break;
                    default:
                        break;
                }

                successful = true;
            }


            context.Dispose();
            return successful;
        }

        /// <summary>
        /// Lists the editable comic book properties to the screen.
        /// </summary>
        /// <param name="comicBook">The comic book property values to list.</param>
        private static void ListComicBookProperties(ComicBook comicBook)
        {
            ConsoleHelper.ClearOutput();
            ConsoleHelper.OutputLine("UPDATE COMIC BOOK");

            // NOTE: This list of comic book property values 
            // needs to match the collection of editable properties 
            // declared at the top of this file.
            ConsoleHelper.OutputBlankLine();
            ConsoleHelper.OutputLine("1) Series: {0}", comicBook.Series.Title);
            ConsoleHelper.OutputLine("2) Issue Number: {0}", comicBook.IssueNumber);
            ConsoleHelper.OutputLine("3) Description: {0}", comicBook.Description);
            ConsoleHelper.OutputLine("4) Published On: {0}", comicBook.PublishedOn.ToShortDateString());
            ConsoleHelper.OutputLine("5) Average Rating: {0}", comicBook.AverageRating);
            Console.WriteLine("6) Artists: ");
            foreach (var comicArtist in comicBook.Artists)
            {
                Console.WriteLine("\t" + comicArtist.Artist.Name);
            }
        }

        private static void GetAllArtist()
        {
            _artistRepository = new ArtistRepository(context);
            var artists = _artistRepository.GetList().OrderBy(a => a.Id);
            ConsoleHelper.ClearOutput();
            Console.WriteLine("Artists");

            ConsoleHelper.OutputBlankLine();
            foreach (var artist in artists)
            {
                ConsoleHelper.OutputLine("{0}) {1}", (artist.Id + 1), artist.Name);
            }

            ConsoleHelper.OutputBlankLine();
            context.Dispose();
        }

        private static void ChooseAddArtist(ComicBook comicBook)
        {
            _comicBookArtistRepository = new ComicBookArtistRepository(context);
            var comicArtists = _comicBookArtistRepository.GetList();

            ConsoleHelper.ClearOutput();
            ConsoleHelper.OutputBlankLine();
            Console.WriteLine("Artists");
            ConsoleHelper.OutputBlankLine();

            // List Artists out of ComicBookArtists
            foreach (var comicArtist in comicArtists)
            {
                ConsoleHelper.OutputLine("{0}) {1} - {2}",
                    comicArtists.IndexOf(comicArtist) + 1,
                    comicArtist.Artist.Name, comicArtist.Role.Name);
            }

            ConsoleHelper.OutputBlankLine();
            ConsoleHelper.Output("Choose an Artist [1-{0}] to Add, C - Cancel", comicArtists.Count);
            int result = 0;


            // choose Artist
            while(true)
            {
                var attemptChoice = ConsoleHelper.ReadInput("> ").ToLower();

                if (Int32.TryParse(attemptChoice, out result))
                {
                    // Add Artist
                    if (result < comicArtists.Count)
                    {
                        var artist = comicArtists[result - 1];
                        comicBook.AddArtist(artist.Artist, artist.Role);
                    }

                    break;
                }
                else if (attemptChoice == "c")
                {
                    // Cancel
                    break;
                }
            }

            context.Dispose();  
        }
    }
}
