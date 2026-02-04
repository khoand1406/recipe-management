using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Domain.Enums
{
    public enum UserActivityType
    {
        Login = 1,
        Logout = 2,
        CreateRecipe = 10,
        UpdateRecipe = 11,
        DeleteRecipe = 12,

        View=19,
        Comment = 20,
        Rate = 21,
        Bookmark = 22,

        Follow = 30
    }
}
