using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PriceNegotiationAPI.Model;

namespace PriceNegotiationAPI.Database
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Negotiation> Negotiations { get; set; }
        public DbSet<NegotiationOffer> NegotiationOffers { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Negotiation>()
                .HasOne(n => n.Product)
                .WithMany(p => p.Negotiations)
                .HasForeignKey(n => n.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Negotiation>()
                .HasOne(n => n.CreatedBy)
                .WithMany(u => u.Negotiations)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NegotiationOffer>()
                .HasOne(no => no.Negotiation)
                .WithMany(n => n.NegotiationOffers)
                .HasForeignKey(no => no.NegotiationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NegotiationOffer>()
                .HasOne(no => no.Employee)
                .WithMany(u => u.NegotiationOffers)
                .HasForeignKey(no => no.EmployeeId);
        }
    }
}
