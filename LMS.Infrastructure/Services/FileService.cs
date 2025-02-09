using LMS.Infrastructure.Services.AuthService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using LMS.Core.Interfaces;

namespace LMS.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private Microsoft.AspNetCore.Hosting.IHostingEnvironment env;
        public FileService(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            this.env = env;
            //
        }

        public Tuple<int, string> SaveImage(IFormFile imageFile, string category)
        {
            try
            {
                var contentPath = env.ContentRootPath;
                var uploadsFolder = Path.Combine(contentPath, "Uploads");

                // Determine the target folder based on the category
                var targetFolder = Path.Combine(uploadsFolder, category);

                // Ensure the target directory exists
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // Validate allowed extensions
                var ext = Path.GetExtension(imageFile.FileName).ToLower();
                var allowedExtensions = new string[] { ".jpg", ".png", ".jpeg" };
                if (!allowedExtensions.Contains(ext))
                {
                    string msg = $"Only {string.Join(", ", allowedExtensions)} extensions are allowed";
                    return new Tuple<int, string>(0, msg);
                }

                // Generate unique filename
                string uniqueString = Guid.NewGuid().ToString();
                var newFileName = uniqueString + ext;
                var fileWithPath = Path.Combine(targetFolder, newFileName);

                // Save the file
                using (var stream = new FileStream(fileWithPath, FileMode.Create))
                {
                    imageFile.CopyTo(stream);
                }

                return new Tuple<int, string>(1, Path.Combine(category, newFileName)); // Return relative path
            }
            catch (Exception ex)
            {
                return new Tuple<int, string>(0, "An error occurred while saving the file: " + ex.Message);
            }
        }

    }
}
