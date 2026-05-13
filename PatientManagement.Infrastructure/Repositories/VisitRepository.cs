using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class VisitRepository : IVisitRepository
{
    private readonly PatientManagementDbContext _dbContext;

    public VisitRepository(PatientManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Visit> AddAsync(Visit visit, CancellationToken cancellationToken = default)
    {
        await _dbContext.Visits.AddAsync(visit, cancellationToken);
        return visit;
    }

    public async Task<Visit?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Visits
            .Include(v => v.Medications)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Visit>> GetByPatientIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Visits
            .Where(v => v.PatientId == patientId)
            .Include(v => v.Medications)
            .OrderByDescending(v => v.VisitDate)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
