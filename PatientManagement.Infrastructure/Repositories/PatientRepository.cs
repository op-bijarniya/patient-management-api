using Microsoft.EntityFrameworkCore;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;

namespace PatientManagement.Infrastructure.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly PatientManagementDbContext _dbContext;

    public PatientRepository(PatientManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Patient> AddAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        await _dbContext.Patients.AddAsync(patient, cancellationToken);
        return patient;
    }

    public async Task<Patient?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .Include(p => p.Appointments)
            .Include(p => p.Visits)
                .ThenInclude(v => v.Medications)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Patient>> SearchAsync(string? searchText, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients.AsQueryable();
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var normalized = searchText.Trim().ToLower();
            query = query.Where(p => p.FirstName.ToLower().Contains(normalized)
                                     || p.LastName.ToLower().Contains(normalized)
                                     || p.PhoneNumber.ToLower().Contains(normalized));
        }

        return await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Take(50)
            .ToListAsync(cancellationToken);
    }

    public async Task<Patient> UpdateAsync(Patient patient, CancellationToken cancellationToken = default)
    {
        _dbContext.Patients.Update(patient);
        return patient;
    }

    public async Task<IReadOnlyCollection<Patient>> GetRecentPatientsAsync(int count, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Patients
            .OrderByDescending(p => p.Id)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
