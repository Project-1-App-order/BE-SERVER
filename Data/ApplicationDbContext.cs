﻿using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options) { }

        public DbSet<OtpStorage> OtpStorages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodImage> FoodImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderType> OrderTypes { get; set; }
        public DbSet<RevokedToken> RevokedTokens { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OtpStorage>(entity =>
            {
                entity.HasOne(o => o.ApplicationUser)
                    .WithMany()
                    .HasForeignKey(o => o.UserId)   
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(o => o.UserId);
            });

            modelBuilder.Entity<OrderDetail>()
              .HasKey(rd => new { rd.OrderId, rd.FoodId });

            modelBuilder.Entity<OrderDetail>()
                        .HasOne(rd => rd.Order)
                        .WithMany(r => r.OrderDetails)
                        .HasForeignKey(rd => rd.OrderId);
        }
    }
}
