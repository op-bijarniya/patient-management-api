using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> ScheduleAppointmentAsync(ScheduleAppointmentCommand command, CancellationToken cancellationToken = default);
    Task<AppointmentDto?> GetAppointmentAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AppointmentDto>> GetAppointmentsByDateAsync(DateTime date, CancellationToken cancellationToken = default);
    Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, string status, CancellationToken cancellationToken = default);
}
