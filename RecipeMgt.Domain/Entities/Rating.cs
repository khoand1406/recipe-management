using System;
using System.Collections.Generic;

namespace RecipeMgt.Domain.Entities;

public partial class Rating
{
    public int RatingId { get; set; }

    public int RecipeId { get; set; }

    public int UserId { get; set; }

    public int? Score { get; set; }

    public string Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Recipe Recipe { get; set; }

    public virtual User User { get; set; }
}
