using System;
using System.Collections.Generic;
using System.Text;
using MovieShop.Domain.Domain;

namespace MovieShop.Repository.Interface
{
    public interface IRepository<T> where T : BaseEntity
    {
        IEnumerable<T> listAll();
        T Get(Guid? id);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
        IEnumerable<Ticket> GetAllTickets(DateTime? dateFilter);
    }
}
