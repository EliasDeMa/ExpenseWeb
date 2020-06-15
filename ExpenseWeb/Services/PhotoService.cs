using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public PhotoService(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        public void AddPhoto(string fileName, IFormFile photo)
        {
            var path = Path.Combine(_hostEnvironment.WebRootPath, "expense-pics", fileName);

            using var stream = new FileStream(path, FileMode.Create);
            photo.CopyTo(stream);
        }

        public void DeletePhoto(string fileName)
        {
            var prevPath = Path.Combine(_hostEnvironment.WebRootPath, "photos", fileName);
            System.IO.File.Delete(prevPath);
        }
    }
}
