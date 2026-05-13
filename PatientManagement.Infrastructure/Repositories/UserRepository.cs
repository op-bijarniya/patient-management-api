using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly PatientManagementDbContext _dbContext;

    public UserRepository(PatientManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return null;
        }

        var normalizedUsername = username.Trim().ToLowerInvariant();
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == normalizedUsername, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
