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
        
        public BudgetContext(DbContextOptions<BudgetContext> options) : base(options)
        {
            

        }

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
            });

        }
    }
}
