using LMS.Core.Entities;
using LMS.Core.Interfaces;
using LMS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repositories
{
   public class BookRepository:BaseRepository<Book>,IBookRepository
    {
        private readonly ApplicationDBContext context;
        public BookRepository(ApplicationDBContext context):base(context) 
        {
            this.context = context;
        }
    }
}
