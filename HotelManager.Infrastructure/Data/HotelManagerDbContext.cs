using HotelManager.Infrastructure.Data.Configuration;
using HotelManager.Infrastructure.Data.Еntities;
using HotelManager.Infrastructure.Data.Еntities.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Infrastructure.Data
{
    public class HotelManagerDbContext : IdentityDbContext<User>
    {
        private bool seedDb;

        public HotelManagerDbContext(DbContextOptions<HotelManagerDbContext> options, bool seed = true)
            : base(options)
        {
            this.seedDb = seed;
            if (this.Database.IsRelational())
            {
                this.Database.Migrate();
            }
            else
            {
                this.Database.EnsureCreated();
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (this.seedDb)
            {
                modelBuilder.ApplyConfiguration(new RoomTypeConfiguration());
                modelBuilder.ApplyConfiguration(new UserConfiguration());
            }

            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<Client>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Client>()
                .HasMany(c => c.Reservations)
                .WithMany(r => r.Clients);

            modelBuilder.Entity<Room>()
                .HasKey(r => r.Number);

            modelBuilder.Entity<Room>()
                .HasOne(r => r.RoomType);

            modelBuilder.Entity<Reservation>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User);

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Room);

            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Clients)
                .WithMany(c => c.Reservations);

            base.OnModelCreating(modelBuilder);
        }
    }
}