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

        public DbSet<HighLevelTag> HighLevelTags { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<LowLevelTag> LowLevelTags { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsGroup> NewsGroups { get; set; }
        public DbSet<NewsLowLevelTag> NewsLowLevelTags { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<View> Views { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<VoteRequest> VoteRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HighLevelTag>().ToTable("HighLevelTags");
            modelBuilder.Entity<Keyword>().ToTable("Keywords");
            modelBuilder.Entity<LowLevelTag>().ToTable("LowLevelTags");
            modelBuilder.Entity<News>().ToTable("News");
            modelBuilder.Entity<NewsGroup>().ToTable("NewsGroup");
            modelBuilder.Entity<NewsLowLevelTag>().ToTable("NewsLowLevelTags");
            modelBuilder.Entity<Source>().ToTable("Sources");
            modelBuilder.Entity<View>().ToTable("Views");
            modelBuilder.Entity<Vote>().ToTable("Votes");
            modelBuilder.Entity<VoteRequest>().ToTable("VoteRequests");
        }
    }
}
