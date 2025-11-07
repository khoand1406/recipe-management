using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeMgt.Domain.Entities;

public partial class Image
{
    public int ImageId { get; set; }

    public string EntityType { get; set; }

    public int EntityId { get; set; }

    public string ImageUrl { get; set; }

    public string Caption { get; set; }

    public DateTime? UploadedAt { get; set; }

}
