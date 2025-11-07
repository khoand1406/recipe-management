using Microsoft.AspNetCore.Http;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Images
{
    public interface IImageService
    {
        Task<List<Image>> UploadEntityImagesAsync(IEnumerable<IFormFile> files, string entityType);
    }
}
