using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IVisitService
{
    Task<VisitDto> RecordVisitAsync(CreateVisitCommand command, CancellationToken cancellationToken = default);
    Task<VisitDto?> GetVisitAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VisitDto>> GetVisitsByPatientAsync(int patientId, CancellationToken cancellationToken = default);
}
