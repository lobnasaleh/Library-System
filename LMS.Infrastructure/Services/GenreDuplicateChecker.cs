using LMS.Core.Interfaces;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class GenreDuplicateChecker : IGenreDuplicateChecker
    {
        readonly ApplicationDBContext _dbContext;
        public GenreDuplicateChecker(ApplicationDBContext _dbContext)
        {
            this._dbContext = _dbContext;
        }
        public async Task<bool> IsDuplicateGenre(string name)
        {
          return await _dbContext.Genres.AnyAsync(g => g.Name == name);
        }
    }
}
