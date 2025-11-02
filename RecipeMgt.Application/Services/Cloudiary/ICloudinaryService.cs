using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace RecipeMgt.Application.Services.Cloudiary
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
        Task DeleteImageAsync(string publicId);

        Task<List<string>> UploadImagesAsync(List<IFormFile> files);
    }
}
