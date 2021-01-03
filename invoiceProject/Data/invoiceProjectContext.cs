using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using invoiceProject.Models;

namespace invoiceProject.Data
{
    public class invoiceProjectContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<UserGiftCard>()
                .HasKey(po => new { po.UserID, po.GiftCardID });

            modelBuilder.Entity<UserGiftCard>()
                .HasOne(po => po.user)
                .WithMany(o => o.userGiftCard)
                .HasForeignKey(po => po.UserID);

            modelBuilder.Entity<UserGiftCard>()
                .HasOne(po => po.giftCard)
                .WithMany(o => o.userGiftCard)
                .HasForeignKey(po => po.GiftCardID);


        }

        public invoiceProjectContext (DbContextOptions<invoiceProjectContext> options)
            : base(options)
        {
        }

        public DbSet<invoiceProject.Models.User> User { get; set; }

        public DbSet<invoiceProject.Models.Credit> Credit { get; set; }

        public DbSet<invoiceProject.Models.Invoice> Invoice { get; set; }

        public DbSet<invoiceProject.Models.Category> Category { get; set; }

        public DbSet<invoiceProject.Models.GiftCard> GiftCard { get; set; }

        public DbSet<invoiceProject.Models.UserGiftCard> UserGiftCard { get; set; }
    }
}
