using AutoMapper;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Enums;
using PatientManagement.Domain.Interfaces;

namespace PatientManagement.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IMapper mapper)
    {
        _appointmentRepository = appointmentRepository;
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<AppointmentDto> ScheduleAppointmentAsync(ScheduleAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(command.PatientId, cancellationToken);
        if (patient == null)
        {
            throw new NotFoundException($"Patient with id {command.PatientId} was not found.");
        }

        var appointment = new Appointment(command.PatientId, command.ScheduledAt, command.Reason);
        await _appointmentRepository.AddAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<AppointmentDto?> GetAppointmentAsync(int id, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
    }

    public async Task<IReadOnlyCollection<AppointmentDto>> GetAppointmentsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        var appointments = await _appointmentRepository.GetByDateAsync(date.Date, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<AppointmentDto>>(appointments);
    }

    public async Task<AppointmentDto> UpdateAppointmentStatusAsync(int appointmentId, string status, CancellationToken cancellationToken = default)
    {
        var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            throw new NotFoundException($"Appointment with id {appointmentId} was not found.");
        }

        if (!Enum.TryParse<AppointmentStatus>(status, true, out var appointmentStatus))
        {
            throw new ArgumentException($"'{status}' is not a valid appointment status.", nameof(status));
        }

        appointment.UpdateStatus(appointmentStatus);
        await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await _appointmentRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<AppointmentDto>(appointment);
    }
}
