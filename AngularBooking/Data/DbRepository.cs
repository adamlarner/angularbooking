using AngularBooking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AngularBooking.Data
{
    /*
     * All methods have basic catch-all supressions, where catch blocks can be used for internal logging and graceful failure (return false)
     */

    public class DbRepository<T> : IRepository<T> where T : class, IModel
    {
        private ApplicationDbContext _context;

        public DbRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Create(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
                int added = _context.SaveChanges();
                return added > 0;
            }
            catch(Exception e)
            {
                // additional logging should be added here
                return false;
            }
            
        }

        public bool Delete(T entity)
        {
            // use entity id to get tracked entity from context
            try
            {
                var trackedEntity = _context.Set<T>().SingleOrDefault(f => f.Id == entity.Id);

                if (trackedEntity != null)
                {
                    _context.Remove<T>(trackedEntity);
                    int deleted = _context.SaveChanges();
                    return deleted > 0;
                }

                return false;
            }
            catch(Exception e)
            {
                return false;
            }

        }

        public bool Update(T entity)
        {
            try
            {
                // use entity id to get tracked entity from context
                var trackedEntity = _context.Set<T>().SingleOrDefault(f => f.Id == entity.Id);
                if (trackedEntity != null)
                {
                    // detach existing entity, and attached newly modified entity as modified
                    _context.Entry(trackedEntity).State = EntityState.Detached;
                    _context.Entry(entity).State = EntityState.Modified;

                    int updated = _context.SaveChanges();
                    return updated > 0;
                }

                return false;
            }
            catch(Exception e)
            {
                return false;
            }
            
        }

        public bool Exists(T entity)
        {
            try
            {
                T found = _context.Set<T>().Find(entity.Id);
                if (found != null)
                    return true;

                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public IQueryable<T> Get()
        {
            try
            {
                return _context.Set<T>();
            }
            catch(Exception e)
            {
                // return empty list (add logging and alert system in live scenario!)
                return new List<T>().AsQueryable();
            }
            
        }

        public T GetById(int id)
        {
            try
            {
                // return null object (same as above, add monitoring to ensure problems aren't left without remedy)
                return _context.Set<T>().SingleOrDefault(f => f.Id == id);
            }
            catch(Exception e)
            {
                return null;
            }
            
        }

    }
}
