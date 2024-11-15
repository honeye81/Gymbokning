using Gymbokning.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gymbokning.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<GymClass> GymClasses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the composite key for the join table
            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasKey(aug => new { aug.ApplicationUserId, aug.GymClassId });

            // Configure the many-to-many relationship
            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.ApplicationUser)
                .WithMany(u => u.AttendingClasses)
                .HasForeignKey(aug => aug.ApplicationUserId);

            modelBuilder.Entity<ApplicationUserGymClass>()
                .HasOne(aug => aug.GymClass)
                .WithMany(g => g.AttendingMembers)
                .HasForeignKey(aug => aug.GymClassId);
        }
        public DbSet<Gymbokning.Models.GymClass> GymClass { get; set; } = default!;
    }
}
