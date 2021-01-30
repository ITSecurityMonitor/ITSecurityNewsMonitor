using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Data
{
    public class DbInitializer
    {
        public static void Initialize(SecNewsDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            /*if (context.Students.Any())
            {
                return;   // DB has been seeded
            }*/

            context.SaveChanges();
        }
    }
}
