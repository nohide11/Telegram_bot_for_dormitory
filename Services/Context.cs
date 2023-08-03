using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace bot_misis.Services
{
    class Context : DbContext
    {
        public Context()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(@"host=localhost;port=5432;database=tg_misis;username=postgres;password=rootroot");
        }

        public DbSet<Entities.Users> Users { get; set; }
        public DbSet<Entities.Violations> Violations { get; set; }
    }
}
