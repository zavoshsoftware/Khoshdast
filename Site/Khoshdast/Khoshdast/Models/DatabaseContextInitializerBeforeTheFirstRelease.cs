using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    internal class DatabaseContextInitializerBeforeTheFirstRelease :
        DropCreateDatabaseIfModelChanges<DatabaseContext>
    {
        protected override void Seed(DatabaseContext databaseContext)
        {
            base.Seed(databaseContext);

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
