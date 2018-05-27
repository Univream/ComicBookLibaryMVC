using ComicBookShared.Models;
using System;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    /// <summary>
    /// Custom database initializer class used to populate
    /// the database with seed data.
    /// </summary>
    internal class DatabaseInitializer : DropCreateDatabaseIfModelChanges<Context>
    {
        protected override void Seed(Context context)
        {
            // This is our database's seed data...
            // 3 series, 6 artists, 2 roles, and 9 comic books.

            
        }
    }
}
