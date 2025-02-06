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
    public class GenreRepository:BaseRepository<Genre>,IGenreRepository
    {
        private readonly ApplicationDBContext context;

        public GenreRepository(ApplicationDBContext context):base(context) 
        {
            this.context = context; 
        }
    }
}
