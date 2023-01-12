using System;
using BlogApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}

