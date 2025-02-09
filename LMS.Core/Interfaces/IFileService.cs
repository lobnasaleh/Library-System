using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IFileService
    {
        public Tuple<int, string> SaveImage(IFormFile imageFile,string category);

    }
}
