using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IGenreDuplicateChecker
    {
        Task<bool> IsDuplicateGenre(string name);
    }
}
