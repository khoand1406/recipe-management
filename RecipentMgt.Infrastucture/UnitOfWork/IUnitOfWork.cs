
using RecipentMgt.Infrastucture.Repository.Dishes;
using RecipentMgt.Infrastucture.Repository.Ingredients;
using RecipentMgt.Infrastucture.Repository.Recipes;
using RecipentMgt.Infrastucture.Repository.Steps;
using RecipentMgt.Infrastucture.Repository.Users;


    public interface IUnitOfWork
    {
        IRecipeRepository Recipes { get; }
        IIngredientRepository Ingredients { get; }
        IStepRepository Steps { get; }
        IDishRepository Dishes { get; }
        IUserRepository Users { get; }

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        Task SaveChangesAsync();
    }


