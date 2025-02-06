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
    public class BookGenreRepository:BaseRepository<BookGenre>,IBookGenreRepository
    {
        private readonly ApplicationDBContext context;
        public BookGenreRepository(ApplicationDBContext context) :base(context)
        {
            this.context = context;
        }
    }
}
