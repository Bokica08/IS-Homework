using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Identity;

namespace MovieShop.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<MovieApplicationUser> listAll();
        MovieApplicationUser Get(string id);
        void Insert(MovieApplicationUser entity);
        void Update(MovieApplicationUser entity);
        void Delete(MovieApplicationUser entity);
    }
}
