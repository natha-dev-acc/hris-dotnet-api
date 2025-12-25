using System.Collections.Generic;
using HRIS_API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRIS_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Role> Roles { get; set; }

    }
}
