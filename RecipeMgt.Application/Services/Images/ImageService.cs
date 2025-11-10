using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using RecipeMgt.Application.Services.Cloudiary;
using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Services.Images
{
    public class ImageService : IImageService
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILogger<ImageService> _logger;
        public ImageService(ICloudinaryService cloudinaryService, ILogger<ImageService> logger)
        {
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<List<Image>> UploadEntityImagesAsync(IEnumerable<IFormFile> files, string entityType)
        {
            var images = new List<Image>();

            if (files == null || !files.Any())
                return images;

            foreach (var file in files)
            {
                try
                {
                    var uploadUrl = await _cloudinaryService.UploadImageAsync(file);

                    images.Add(new Image
                    {
                        EntityType = entityType,
                        ImageUrl = uploadUrl,
                        Caption = Path.GetFileNameWithoutExtension(file.FileName),
                        UploadedAt = DateTime.Now
                    });

                    _logger.LogInformation($"Uploaded image for {entityType}: {file.FileName}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to upload image {file.FileName} for {entityType}: {ex.Message}");
                }
            }

            return images;
        }
    }
}
