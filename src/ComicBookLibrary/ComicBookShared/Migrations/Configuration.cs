namespace ComicBookLibaryWebApp.Migrations
{
    using ComicBookShared.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ComicBookShared.Data.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "ComicBookShared.Data.Context";
        }

        protected override void Seed(ComicBookShared.Data.Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            context.Roles.AddOrUpdate(
                r => r.Name,
                new Role() { Name = "Script"},
                new Role() { Name = "Pencils"}
                );
        }
    }
}
