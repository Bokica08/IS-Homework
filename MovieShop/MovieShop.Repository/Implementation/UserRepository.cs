using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MovieShop.Domain.Identity;
using MovieShop.Repository.Interface;

namespace MovieShop.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<MovieApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<MovieApplicationUser>();
        }
        public IEnumerable<MovieApplicationUser> listAll()
        {
            return entities.AsEnumerable();
        }

        public MovieApplicationUser Get(string id)
        {
            return entities
                .Include(z => z.UserCart)
                .Include(z => z.UserCart.Tickets)
                .Include("UserCart.Tickets.Ticket")
                .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(MovieApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(MovieApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(MovieApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
