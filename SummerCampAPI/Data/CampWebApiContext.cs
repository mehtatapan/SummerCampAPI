using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SummerCampAPI.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SummerCampAPI.Data
{
    public class CampWebApiContext : DbContext
    {
        //To give access to IHttpContextAccessor for Audit Data with IAuditable
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public CampWebApiContext(DbContextOptions<CampWebApiContext> options)
            : base(options)
        {
            UserName = "SeedData";
        }

        public CampWebApiContext(DbContextOptions<CampWebApiContext> options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
            //UserName = (UserName == null) ? "Unknown" : UserName;
            UserName = UserName ?? "Unknown";
        }

        public DbSet<Compound> Compounds { get; set; }
        public DbSet<Camper> Campers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("SC");

            //a unique index to the EMail Number
            modelBuilder.Entity<Camper>()
            .HasIndex(p => p.EMail)
            .IsUnique();

            modelBuilder.Entity<Compound>()
                .HasMany(p => p.Campers)
                .WithOne(d => d.Compound)
                .OnDelete(DeleteBehavior.Restrict);

        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
