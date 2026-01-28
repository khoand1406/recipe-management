using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.RequestEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Utils
{
    public static class PaginationHelper
    {
        public static async Task<PagedResponse<T>> ToPagedResponseAsync<T>(
        IQueryable<T> query,
        int page,
        int pageSize)
        {
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResponse<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
