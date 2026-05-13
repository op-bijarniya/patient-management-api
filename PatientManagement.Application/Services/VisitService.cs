using AutoMapper;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;

namespace PatientManagement.Application.Services;

public class VisitService : IVisitService
{
    private readonly IVisitRepository _visitRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;

    public VisitService(IVisitRepository visitRepository, IPatientRepository patientRepository, IAppointmentRepository appointmentRepository, IMapper mapper)
    {
        _visitRepository = visitRepository;
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
    }

    public async Task<VisitDto> RecordVisitAsync(CreateVisitCommand command, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(command.PatientId, cancellationToken);
        if (patient == null)
        {
            throw new NotFoundException($"Patient with id {command.PatientId} was not found.");
        }

        if (command.AppointmentId.HasValue)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.AppointmentId.Value, cancellationToken);
            if (appointment == null)
            {
                throw new NotFoundException($"Appointment with id {command.AppointmentId.Value} was not found.");
            }
        }

        var visit = new Visit(command.PatientId, command.VisitDate, command.TemperatureC, command.BloodPressure, command.Pulse, command.Complaints, command.Diagnosis, command.Notes, command.AppointmentId);
        foreach (var medicationCommand in command.Medications)
        {
            var medication = new Medication(0, medicationCommand.Name, medicationCommand.Dosage, medicationCommand.Frequency, medicationCommand.Duration, medicationCommand.Instructions);
            visit.AddMedication(medication);
        }

        await _visitRepository.AddAsync(visit, cancellationToken);
        await _visitRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<VisitDto>(visit);
    }

    public async Task<VisitDto?> GetVisitAsync(int id, CancellationToken cancellationToken = default)
    {
        var visit = await _visitRepository.GetByIdAsync(id, cancellationToken);
        return visit == null ? null : _mapper.Map<VisitDto>(visit);
    }

    public async Task<IReadOnlyCollection<VisitDto>> GetVisitsByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var visits = await _visitRepository.GetByPatientIdAsync(patientId, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<VisitDto>>(visits);
    }
}
