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
    public class BorrowRepository:BaseRepository<Borrow>,IBorrowRepository
    {
        private readonly ApplicationDBContext context;
        public BorrowRepository(ApplicationDBContext context) :base(context)
        {
            this.context = context;
        }
    }
}
