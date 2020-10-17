using Models;

namespace Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Models.DatabaseContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Models.DatabaseContext databaseContext)
        {
            base.Seed(databaseContext);
            if (databaseContext.Roles.Count() != 0)
            {
                return;
            }

            try
            {
                DatabaseContextInitializer.Seed(databaseContext);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
