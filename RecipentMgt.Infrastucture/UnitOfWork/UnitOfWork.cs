using Microsoft.EntityFrameworkCore.Storage;
using RecipentMgt.Infrastucture.Persistence;
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Ingredients;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Steps;
using RecipentMgt.Infrastucture.Repository.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly RecipeManagementContext _context;
        private IDbContextTransaction? _transaction;

        public IRecipeRepository Recipes { get; }
        public IIngredientRepository Ingredients { get; }
        public IStepRepository Steps { get; }

        public IDishRepository Dishes { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(
            RecipeManagementContext context,
            IRecipeRepository recipes,
            IIngredientRepository ingredients,
            IStepRepository steps,

            IDishRepository dishes,
            IUserRepository users)
        {
            _context = context;
            Recipes = recipes;
            Ingredients = ingredients;
            Steps = steps;

            Dishes = dishes;
            Users = users;
        }

        public async Task BeginTransactionAsync()
            => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            await _transaction!.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
                await _transaction.RollbackAsync();
        }

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
