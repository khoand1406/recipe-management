using Microsoft.EntityFrameworkCore;
using RecipeMgt.Domain.Entities;
using RecipentMgt.Infrastucture.Persistence;

namespace RecipentMgt.Infrastucture.Repository.Steps
{
    public class StepRepository : IStepRepository
    {
        private readonly RecipeManagementContext _context;

        public StepRepository(RecipeManagementContext context)
        {
            _context = context;
        }

        public async Task<Step?> GetByIdAsync(int stepId)
        {
            return await _context.Steps.FindAsync(stepId);
        }

        public async Task<IEnumerable<Step>> GetByRecipeIdAsync(int recipeId)
        {
            return await _context.Steps
                .Where(s => s.RecipeId == recipeId)
                .OrderBy(s => s.StepNumber)
                .ToListAsync();
        }

        public async Task<Step> CreateAsync(Step step)
        {
            _context.Steps.Add(step);
            await _context.SaveChangesAsync();
            return step;
        }

        public async Task<Step?> UpdateAsync(Step step)
        {
            var existing = await _context.Steps.FindAsync(step.StepId);
            if (existing == null) return null;

            existing.StepNumber = step.StepNumber;
            existing.Instruction = step.Instruction;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int stepId)
        {
            var step = await _context.Steps.FindAsync(stepId);
            if (step == null) return false;

            _context.Steps.Remove(step);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
