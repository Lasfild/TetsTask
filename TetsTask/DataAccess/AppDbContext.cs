using Microsoft.EntityFrameworkCore;
using System;
using TetsTask.Models;

namespace TetsTask.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        public DbSet<Person> Person { get; set; }
    }
}
