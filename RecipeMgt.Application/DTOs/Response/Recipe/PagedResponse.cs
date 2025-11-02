using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeMgt.Application.DTOs.Response.Recipe
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; }= new List<T>();

        public int Page {  get; set; }

        public int PageSize { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }
    }
}
