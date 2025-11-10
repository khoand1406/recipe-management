using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Categories
{
    public interface ICategoryRepository
    {
        Task<ICollection<Category>> GetAll();

        
    }
}
