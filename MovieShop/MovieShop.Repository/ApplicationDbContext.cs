using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MovieShop.Domain.Domain;
using MovieShop.Domain.Identity;
using Microsoft.EntityFrameworkCore;


namespace MovieShop.Repository
{
    public class ApplicationDbContext : IdentityDbContext<MovieApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<TicketInShoppingCart> TicketsinShoppingCarts { get; set; }
        public virtual DbSet<EmailMessage> EmailMessages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Ticket>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();

            builder.Entity<ShoppingCart>()
                .Property(z => z.Id)
                .ValueGeneratedOnAdd();


            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Ticket)
                .WithMany(z => z.Ticketsinshoppingcart)
                .HasForeignKey(z => z.ShoppingCartId);
            builder.Entity<TicketInShoppingCart>()
                .HasOne(z => z.Shoppingcart)
                .WithMany(z => z.Tickets)
                .HasForeignKey(z => z.TicketId);
            builder.Entity<ShoppingCart>()
                .HasOne<MovieApplicationUser>(z => z.Owner)
                .WithOne(z => z.UserCart)
                .HasForeignKey<ShoppingCart>(z => z.OwnerId);
            builder.Entity<OrderTicket>()
                .HasOne(z => z.SelectedTicket)
                .WithMany(z => z.Orders)
                .HasForeignKey(z => z.TicketId);
            builder.Entity<OrderTicket>()
                .HasOne(z => z.UserOrder)
                .WithMany(z => z.OrderTicket)
                .HasForeignKey(z => z.OrderId);
        }
    }
}
