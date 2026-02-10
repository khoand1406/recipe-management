using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.Constant
{
    public record DishViewedEvent(
    int DishId,
    int UserId,
    DateTime ViewedAt
);
}
