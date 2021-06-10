using ITSecurityNewsMonitor.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITSecurityNewsMonitor.Data
{
    public class SecNewsDbContext : DbContext
    {
        public SecNewsDbContext(DbContextOptions<SecNewsDbContext> options) : base(options)
        {
        }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsGroup> NewsGroups { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<LinkViewed> LinksViewed { get; set; }
        public DbSet<Favorite> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().ToTable("Tags");
            modelBuilder.Entity<News>().ToTable("News");
            modelBuilder.Entity<NewsGroup>().ToTable("NewsGroup");
            modelBuilder.Entity<Source>().ToTable("Sources");
            modelBuilder.Entity<View>().ToTable("Views");
            modelBuilder.Entity<LinkViewed>().ToTable("LinksViewed");
            modelBuilder.Entity<Favorite>().ToTable("Favorites");
        }
    }
}
