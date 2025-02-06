using LMS.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        //getAllByCondition
       public Task<IEnumerable<T>> getAllAsync(Expression<Func<T, bool>>? condition=null,
          string[] include = null,
           Expression<Func<T, object>>? orderBy=null, string orderByDirection = Order.Ascending,
           int pagenumber=1,int pagesize=0);
       
        //getbycondition
       public Task<T> getAsync(Expression<Func<T, bool>>? condition,
           bool tracking=true,
           string[] include = null
           );
        //add
       public Task<T> AddAsync(T entity);
        //update
       public  void  Update(T newentity);
        //delete
       public void Delete(T entity);
        //count
        public Task<int> CountAsync(Expression<Func<T, bool>>? condition = null);


    }
}
