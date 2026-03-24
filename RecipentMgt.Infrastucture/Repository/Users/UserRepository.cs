using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecipeMgt.Domain.Entities;
using RecipeMgt.Domain.Enums;
using RecipeMgt.Domain.RequestEntity;
using RecipentMgt.Infrastucture.Persistence;
using RecipentMgt.Infrastucture.Repository.Users;
using RecipentMgt.Infrastucture.Utils;

public class UserRepository : IUserRepository
{
    private readonly RecipeManagementContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(RecipeManagementContext context, ILogger<UserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetByIdAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.Recipes)
            .Include(u => u.Ratings)
            .Include(u => u.UserStatistic)
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Users.AnyAsync(x => x.Email == email);
    }

    public async Task<int> CountAsync()
    {
        return await _context.Users.CountAsync();
    }

    public async Task<PagedResponse<User>> GetUsersAsync(int page, int pageSize, string? searchQuery, int? userStatus)
    {
        var query = _context.Users
            .Include(u => u.Followers)
            .Include(u => u.Recipes)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(searchQuery))
        {
            query = query.Where(x => x.FullName.Contains(searchQuery));
        }

        if (userStatus != null)
        {
            query = userStatus switch
            {
                0 => query.Where(x => x.IsActived),
                1 => query.Where(x => !x.IsActived),
                2 => query.Where(x => x.IsBanned),
                3 => query.Where(x => x.DeleteAt != null),
                _ => throw new ArgumentException("INVALID_USER_STATUS"),
            };
        }

        return await PaginationHelper.ToPagedResponseAsync(query, page, pageSize);
    }

    public async Task<int> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return await _context.SaveChangesAsync();
    }

    public async Task CreateBatchAsync(List<User> users)
    {
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRangeAsync(List<User> users)
    {
        _context.Users.UpdateRange(users);
        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(User user)
    {
        user.DeleteAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }


    public async Task CreateUserActivityLogAsync(
        int? userId,
        string? sessionId,
        UserActivityType action,
        string target,
        int targetId,
        string? description)
    {
        await _context.UserActivityLogs.AddAsync(new UserActivityLog
        {
            UserId = userId,
            SessionId = sessionId,
            ActivityType = action,
            TargetType = target,
            TargetId = targetId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }


    public async Task<User> UpsertGoogleUserAsync(string providerId, string email, string username)
    {
        User? user = null;

        if (!string.IsNullOrEmpty(providerId))
        {
            user = await _context.Users
                .FirstOrDefaultAsync(x => x.Provider == "Google" && x.ProviderId == providerId);
        }

      
        user ??= await _context.Users
                .FirstOrDefaultAsync(x => x.Email == email);

        if (user != null)
        {
            user.FullName = username ?? user.FullName;
            user.IsActived = true;

            await _context.SaveChangesAsync();
            return user;
        }

        user = new User
        {
            Email = email,
            FullName = username,
            Provider = "Google",
            ProviderId = providerId,
            CreatedAt = DateTime.UtcNow,
            IsActived = true
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<User> UpsertAzureUserAsync(string email, string? name)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user != null) return user;

        user = new User
        {
            Email = email,
            FullName = name ?? "Default Name",
            Provider = "Azure",
            ProviderId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            IsActived = true
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }


    public async Task<List<User>> GetTopContributorsAsync()
    {
        var topContributor = await _context.Users
                .Include(u => u.UserStatistic)
                .OrderBy(u => u.UserStatistic.RecipeCount)
                .Take(5)
                .ToListAsync();
        return topContributor;
    }

    

    public Task DeleteAsync(User user)
    {
        throw new NotImplementedException();
    }

    public async Task<List<User>> GetUsersByIds(List<int> items)
    {
        var result= await _context.Users.Where(x=> items.Contains(x.UserId)).ToListAsync();
        return result;
    }

    public async Task<User?> GetByUserName(string username)
    {
        var result= await _context.Users.Where(x=> x.FullName.Equals(username)).FirstOrDefaultAsync();
        return result;
    }

    public async Task<Role?> GetRoleByName(string roleName)
    {
        var role=  await _context.Roles.FirstOrDefaultAsync(x=> x.RoleName.Equals(roleName));
        return role ?? null;
    }

    public async Task<IEnumerable<string>> GetExistingEmails(List<string> emails)
    {
        return await _context.Users
        .Where(x => emails.Contains(x.Email))
        .Select(x => x.Email)
        .ToListAsync();
    }

    public async Task<IEnumerable<string>> GetExistingName(List<string> names)
    {
        return await _context.Users.Where(x=> names.Contains(x.FullName))
                     .Select(x=> x.FullName)
                     .ToListAsync();
    }
}
