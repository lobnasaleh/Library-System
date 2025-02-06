using LMS.Core.Constants;
using LMS.Core.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories
{
   public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDBContext context;
       // private readonly DbSet<T> dbset;

        public BaseRepository(ApplicationDBContext context) { 
        
          this.context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
          await context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? condition = null)
        {
            IQueryable<T> query = context.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }
            return await query.CountAsync();
           
        }

        public void Delete(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public async Task<IEnumerable<T>> getAllAsync(Expression<Func<T, bool>>? condition=null, string[] include = null, Expression<Func<T, object>>? orderBy = null, string orderByDirection = Order.Ascending, int pagenumber = 1, int pagesize = 0)
        {
            IQueryable<T> query = context.Set<T>();
          
            if (condition != null)
            {
                query = query.Where(condition);
            }
            if (include != null)
            {
                foreach (var inc in include)
                {
                    query = query.Include(inc);

                }
            }
            if (orderBy != null) {

                if (orderByDirection == Order.Descending) {
                    query.OrderByDescending(orderBy);
                }
                else if (orderByDirection == Order.Ascending)
                {
                    query.OrderBy(orderBy);
                }
            }
            if (pagesize > 0)
            {
                if (pagesize > 100)
                {
                    pagesize = 100;
                }
                query= query.Skip(pagesize*(pagenumber-1)).Take(pagesize);
            }
           
            return await query.ToListAsync();
        }

        public async Task<T> getAsync(Expression<Func<T, bool>>? condition, bool tracking = true, string[] include = null)
        {
            IQueryable<T> query = context.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }
            if (include != null)
            {
                foreach (var inc in include)
                {
                   query = query.Include(inc);  
                    //in controller        new []{"Author","Borrow"}

                }
            }
           
            if (!tracking)
            {
               
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public void Update( T newentity)
        {
            context.Set<T>().Update(newentity);
        }
    }
}
