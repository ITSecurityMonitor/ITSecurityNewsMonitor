using ITSecurityNewsMonitor.Models;
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

            if (context.Sources.Any())
            {
                return;   // DB has been seeded
            }

            Source source1 = new Source()
            {
                Name = "Securityweek",
                Link = "http://feeds.feedburner.com/Securityweek?format=xml",
                Homepage = "https://www.securityweek.com/"
            };

            Source source2 = new Source()
            {
                Name = "Threatpost",
                Link = "https://threatpost.com/feed/",
                Homepage = "https://threatpost.com/"
            };


            Source source3 = new Source()
            {
                Name = "Bleepingcomputer",
                Link = "https://www.bleepingcomputer.com/feed/",
                Homepage = "https://www.bleepingcomputer.com/"
            };


            context.Sources.Add(source1);
            context.SaveChanges();
        }
    }
}
