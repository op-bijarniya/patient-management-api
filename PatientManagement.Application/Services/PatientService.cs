using AutoMapper;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;

namespace PatientManagement.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<PatientDto> CreatePatientAsync(CreatePatientCommand command, CancellationToken cancellationToken = default)
    {
        var patient = new Patient(command.FirstName, command.LastName, command.DateOfBirth, command.Gender, command.PhoneNumber, command.Email, command.Address);
        await _patientRepository.AddAsync(patient, cancellationToken);
        await _patientRepository.SaveChangesAsync(cancellationToken);
        return _mapper.Map<PatientDto>(patient);
    }

    public async Task<PatientDto> UpdatePatientAsync(UpdatePatientCommand command, CancellationToken cancellationToken = default)
    {
        var existing = await _patientRepository.GetByIdAsync(command.Id, cancellationToken);
        if (existing == null)
        {
            throw new NotFoundException($"Patient with id {command.Id} was not found.");
        }

        existing.UpdateDemographics(command.FirstName, command.LastName, command.DateOfBirth, command.Gender);
        existing.UpdateContact(command.PhoneNumber, command.Email, command.Address);

        await _patientRepository.UpdateAsync(existing, cancellationToken);
        await _patientRepository.SaveChangesAsync(cancellationToken);

        return _mapper.Map<PatientDto>(existing);
    }

    public async Task<PatientDto?> GetPatientAsync(int id, CancellationToken cancellationToken = default)
    {
        var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
        return patient == null ? null : _mapper.Map<PatientDto>(patient);
    }

    public async Task<IReadOnlyCollection<PatientDto>> SearchPatientsAsync(string? searchText, CancellationToken cancellationToken = default)
    {
        var patients = await _patientRepository.SearchAsync(searchText, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<PatientDto>>(patients);
    }

    public async Task<IReadOnlyCollection<PatientDto>> GetRecentPatientsAsync(int count, CancellationToken cancellationToken = default)
    {
        var patients = await _patientRepository.GetRecentPatientsAsync(count, cancellationToken);
        return _mapper.Map<IReadOnlyCollection<PatientDto>>(patients);
    }
}
