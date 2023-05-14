﻿using ECommerce1.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce1.Services
{
    public class ResourceDbContext : DbContext
    {
        public ResourceDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<FavouriteItem> FavouriteItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductPhoto> ProductPhotos { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Seller> Sellers { get; set; }
        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewPhoto> ReviewPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<City>(e =>
            {
                e.Property(e => e.Name).HasColumnType("nvarchar(256)").HasMaxLength(256).IsRequired();

                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(c => c.Country)
                .WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(c => c.Addresses)
                .WithOne(a => a.City)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Country>(e =>
            {
                e.Property(e => e.Name)
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256).IsRequired();

                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);
                 
                e.HasMany(p => p.Cities)
                .WithOne(p => p.Country)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<Address>(e =>
            {
                e.Property(e => e.First)
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256).IsRequired();

                e.Property(e => e.Second)
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256);

                e.Property(e => e.Zip)
                .HasColumnType("nvarchar(8)")
                .HasMaxLength(8).IsRequired();

                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                .WithMany(e => e.Addresses)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.City)
                .WithMany(e => e.Addresses)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Category>(e =>
            {
                e.Property(e => e.Name)
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256)
                .IsRequired();

                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.AllowProducts)
                .HasDefaultValue(false).IsRequired();

                e.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .IsRequired();

                e.HasOne(c => c.ParentCategory)
                .WithMany(c => c.ChildCategories)
                .IsRequired(false);

                e.HasMany(c => c.ChildCategories)
                .WithOne(c => c.ParentCategory)
                .IsRequired(false);

                e.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<ProductPhoto>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.Url)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

                e.HasOne(pp => pp.Product)
                .WithMany(p => p.ProductPhotos)
                .IsRequired();
            });

            builder.Entity<Profile>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.AuthId)
                .IsRequired();

                e.Property(e => e.FirstName)
                .HasColumnType("nvarchar(64)")
                .HasMaxLength(64)
                .IsRequired();

                e.Property(e => e.MiddleName)
                .HasColumnType("nvarchar(64)")
                .HasMaxLength(64);

                e.Property(e => e.LastName)
                .HasColumnType("nvarchar(64)")
                .HasMaxLength(64)
                .IsRequired();

                e.Property(e => e.PhoneNumber)
                .HasColumnType("nvarchar(32)")
                .HasMaxLength(32)
                .IsRequired();

                e.HasIndex(e => e.PhoneNumber).IsUnique();

                e.Property(e => e.Email)
                .HasColumnType("nvarchar(320)")
                .HasMaxLength(320)
                .IsRequired();

                e.HasIndex(e => e.Email).IsUnique();

                e.HasMany(e => e.Addresses)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(e => e.CartItems)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasMany(e => e.Orders)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.SetNull);

                e.HasMany(e => e.Reviews)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<Staff>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.AuthId)
                .IsRequired();

                e.Property(e => e.DisplayName)
                .HasColumnType("nvarchar(32)")
                .HasMaxLength(32)
                .IsRequired();

                e.Property(e => e.PhoneNumber)
                .HasColumnType("nvarchar(32)")
                .HasMaxLength(32)
                .IsRequired();

                e.HasIndex(e => e.PhoneNumber).IsUnique();

                e.Property(e => e.Email)
                .HasColumnType("nvarchar(320)")
                .HasMaxLength(320)
                .IsRequired();

                e.HasIndex(e => e.Email).IsUnique();
            });

            builder.Entity<Seller>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.AuthId)
                .IsRequired();

                e.Property(e => e.CompanyName)
                .HasColumnType("nvarchar(64)")
                .HasMaxLength(64)
                .IsRequired();

                e.Property(e => e.PhoneNumber)
                .HasColumnType("nvarchar(32)")
                .HasMaxLength(32)
                .IsRequired();

                e.Property(e => e.ProfilePhotoUrl)
                .HasColumnType("nvarchar(max)")
                .IsRequired().HasDefaultValue("/images/default.png");

                e.Property(e => e.WebsiteUrl)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

                e.HasIndex(e => e.PhoneNumber).IsUnique();

                e.Property(e => e.Email)
                .HasColumnType("nvarchar(320)")
                .HasMaxLength(320)
                .IsRequired();

                e.HasIndex(e => e.Email).IsUnique();
            });

            builder.Entity<Product>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.CreationTime)
                .HasDefaultValueSql("getdate()");

                e.Property(e => e.Name)
                .HasColumnType("nvarchar(128)")
                .HasMaxLength(128)
                .IsRequired();

                e.Property(e => e.Description)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

                e.Property(e => e.Price)
                .HasColumnType("money")
                .IsRequired();

                e.Property(e => e.InStock)
                .HasDefaultValue(true);

                e.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .IsRequired();

                e.HasOne(p => p.Seller)
                .WithMany(u => u.Products)
                .IsRequired();

                e.HasMany(p => p.ProductPhotos)
                .WithOne(pp => pp.Product)
                .IsRequired();

                e.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .IsRequired();
            });

            builder.Entity<CartItem>(e =>
            {
                e.Property(e => e.Id)
               .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                .WithMany(e => e.CartItems)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.Product)
                .WithMany(e => e.CartItems)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<FavouriteItem>(e =>
            {
                e.Property(e => e.Id)
               .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                .WithMany(e => e.FavouriteItems)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.Product)
                .WithMany(e => e.FavouriteItems)
                .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Order>(e =>
            {
                e.Property(e => e.Id)
               .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                .WithMany(e => e.Orders)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.Product)
                .WithMany(e => e.Orders)
                .OnDelete(DeleteBehavior.Cascade);

                e.Property(e => e.AddressCopy)
                .HasColumnType("nvarchar(256)")
                .HasMaxLength(256)
                .IsRequired();

                e.Property(e => e.OrderTime)
                .HasDefaultValueSql("getdate()");

                e.Property(e => e.OrderStatus)
                .HasDefaultValue(0);
            });

            builder.Entity<Review>(e =>
            {
                e.Property(e => e.Id)
               .IsRequired();

                e.HasKey(e => e.Id);

                e.HasOne(e => e.User)
                .WithMany(e => e.Reviews)
                .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(e => e.Product)
                .WithMany(e => e.Reviews)
                .OnDelete(DeleteBehavior.Cascade);

                e.Property(e => e.ReviewText)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

                e.Property(e => e.Quality)
                .IsRequired();

                e.HasMany(p => p.Photos)
                .WithOne(pp => pp.Review).IsRequired();
            });

            builder.Entity<ReviewPhoto>(e =>
            {
                e.Property(e => e.Id)
                .IsRequired();

                e.HasKey(e => e.Id);

                e.Property(e => e.Url)
                .HasColumnType("nvarchar(max)")
                .IsRequired();

                e.HasOne(pp => pp.Review)
                .WithMany(p => p.Photos)
                .IsRequired();
            });
        }
    }
}
