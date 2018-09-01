using AngularBooking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AngularBooking.Data
{
    public interface IRepository<T> where T : class, IModel
    {
        bool Create(T entity);
        T GetById(int id);
        IQueryable<T> Get();
        bool Update(T entity);
        bool Delete(T entity);
        bool Exists(T entity);
    }
}
