using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseWeb.Services
{
    public interface IPhotoService
    {
        string AddPhoto(IFormFile file);
        void DeletePhoto(string fileName);
    }
}
