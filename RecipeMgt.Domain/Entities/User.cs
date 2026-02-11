using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecipeMgt.Domain.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string PasswordHash { get; set; }

    public int RoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string Provider { get; set; }= string.Empty;

    public string ProviderId { get; set; } = string.Empty;

    public bool IsActived { get; set; }


    public virtual ICollection<Rating> Ratings { get; set; } = [];

    public virtual ICollection<Dish> Dishes { get; set; } = [];
    public virtual ICollection<Recipe> Recipes { get; set; } = [];

    public virtual ICollection<Bookmark> Bookmarks { get; set; } = [];

    public virtual ICollection<Comment> Comments { get; set; } = [];

    public ICollection<Following> Followers { get; set; } = [];     
    public ICollection<Following> FollowingUsers { get; set; } = [];

    public ICollection<RefreshToken> RefreshTokens { get; set; }= [];
    public virtual Role Role { get; set; }

    public virtual UserStatistic UserStatistic { get; set; }

    public ICollection<UserActivityLog> ActivityLogs { get; set; }

}
