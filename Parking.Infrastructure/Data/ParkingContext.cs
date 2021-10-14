using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Parking.SharedKernel.Models;

namespace Parking.Infrastructure.Data
{
    public class ParkingContext : IdentityDbContext<User>
    {
        public ParkingContext(DbContextOptions<ParkingContext> options)
            : base(options)
        {
        }

        public DbSet<UserPlace> UserPlaces { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseLazyLoadingProxies();
        }
    }
}
