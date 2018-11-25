using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions;
using Microsoft.Extensions.Configuration;
using SimpleBillPay.Models;
using SimpleBillPay.Areas.Identity.Data;

namespace SimpleBillPay
{
    public class BudgetContext : DbContext 
    {
        public DbSet<User> AspNetUsers { get; set; }
        public DbSet<BillTemplate> BillTemplate { get; set; }
        public DbSet<BillInstance> BillInstance { get; set; }
        public DbSet<Payment> Payments { get; set; }
        
        public BudgetContext(DbContextOptions<BudgetContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BillTemplate>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.FrequencyInMonths).IsRequired();
            });

            modelBuilder.Entity<BillInstance>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.DueDate).IsRequired();
                entity.Property(e => e.BillTemplateID).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.HasMany(e => e.Payments).WithOne(e => e.BillInstance);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.ID) ;
                entity.Property(e => e.PaymentDate).IsRequired();
                entity.Property(e => e.BillInstanceID ).IsRequired();
                entity.Property(e => e.Amount).IsRequired();
                entity.HasOne(e => e.BillInstance).WithMany(e => e.Payments);
            });

            modelBuilder.Entity<BillPay>(entity =>    
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.BillPayDate).IsRequired();
                entity.Property(e => e.StartingAmount).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                entity.HasOne(e => e.User);
                entity.HasMany(e => e.Payments);
                entity.HasIndex(e=> new {e.UserId, e.BillPayDate}).IsUnique();
            });

        }

        public DbSet<SimpleBillPay.Models.BillPay> BillPay { get; set; }
    }
}
