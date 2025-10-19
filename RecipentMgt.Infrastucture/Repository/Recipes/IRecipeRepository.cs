﻿using RecipeMgt.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipentMgt.Infrastucture.Repository.Recipes
{
    public interface IRecipeRepository
    {
        Task<IEnumerable<Recipe>> getAll();

        Task<IEnumerable<Recipe>> GetRecipes(int dishId);

        Task<IEnumerable<Recipe>> getRecipesByFilter();

        public Task<IEnumerable<Recipe>> GetRecipesByUser(int userId);

        public Task<IEnumerable<Recipe>> GetRelatedRecipes(int id);

        Task<Recipe?> getRecipeById(int id);

        Task<(bool Success, string Message, int Traceid)> createRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps);

        Task<(bool Success, string Message, int Traceid)> updateRecipes(Recipe request, List<Ingredient> ingredients, List<Step> steps);
        Task<bool> deleteRecipes(int id);
    }
}
