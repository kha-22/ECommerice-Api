using System.Reflection;
using ECommerice.Core.Entities;
using ECommerice.Core.Entities.OrderAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ECommerice.Infrastructure
{
    public class StoreContext : IdentityDbContext<AppUser>
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>()
               .HasOne<Category>(s => s.Category)
               .WithMany(g => g.Products)
               .HasForeignKey(s => s.CategoryId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Description).HasMaxLength(1000);

            modelBuilder.Entity<AppUser>()
             .HasMany(c => c.AddressList)
             .WithOne(e => e.AppUser)
             .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<AppUser>()
              .HasMany(c => c.ContactusList)
              .WithOne(e => e.AppUser)
              .HasForeignKey(f => f.UserId);

            modelBuilder.Entity<AppUser>()
             .HasMany(c => c.OrderList)
             .WithOne(e => e.AppUser)
             .HasForeignKey(f => f.UserId);

        }

        public DbSet<Product> Product {get; set;}
        public DbSet<Category> Category { get; set;}
        public DbSet<Order> Order { get; set;}
        public DbSet<OrderItem> OrderItem{ get; set;}
        public DbSet<DeliveryMethod> DeliveryMethod { get; set;}
        public DbSet<Contactus> Contactus { get; set; }
        


    }
}