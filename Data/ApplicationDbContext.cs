using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkshopManager.Models;

namespace WorkshopManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<UsedPart> UsedParts { get; set; } = default!;
        public DbSet<JobActivity> JobActivities { get; set; } = default!;
        public DbSet<ServiceOrderComment> ServiceOrderComments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JobActivity>()
                .Property(j => j.LaborCost)
                .HasPrecision(18, 2);

            builder.Entity<Part>()
                .Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            builder.Entity<IdentityUser>().ToTable("AspNetUsers");
            builder.Entity<IdentityRole>().ToTable("AspNetRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("AspNetUserClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("AspNetUserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("AspNetUserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("AspNetRoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("AspNetUserTokens");

            builder.Entity<IdentityUser>().Property(u => u.Id).HasColumnType("nvarchar(450)");
            builder.Entity<IdentityRole>().Property(r => r.Id).HasColumnType("nvarchar(450)");
            builder.Entity<IdentityUserClaim<string>>().Property(c => c.Id).HasColumnType("int");
            builder.Entity<IdentityUserRole<string>>().Property(r => r.UserId).HasColumnType("nvarchar(450)");
            builder.Entity<IdentityUserLogin<string>>().Property(l => l.UserId).HasColumnType("nvarchar(450)");
            builder.Entity<IdentityRoleClaim<string>>().Property(c => c.Id).HasColumnType("int");
            builder.Entity<IdentityUserToken<string>>().Property(t => t.UserId).HasColumnType("nvarchar(450)");
        }
    }
}