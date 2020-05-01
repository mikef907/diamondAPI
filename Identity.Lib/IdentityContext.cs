using Common.Lib.Models.EM;
using Microsoft.EntityFrameworkCore;
using System;

namespace Identity.Lib
{
    public class IdentityContext : DbContext
    {
        public IdentityContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
