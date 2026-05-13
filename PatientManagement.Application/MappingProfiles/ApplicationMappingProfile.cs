using AutoMapper;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Domain.Entities;

namespace PatientManagement.Application.MappingProfiles;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CreatePatientCommand, Patient>();
        CreateMap<UpdatePatientCommand, Patient>();
        CreateMap<Patient, PatientDto>();

        CreateMap<ScheduleAppointmentCommand, Appointment>();
        CreateMap<Appointment, AppointmentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<CreateMedicationCommand, Medication>();
        CreateMap<Medication, MedicationDto>();

        CreateMap<Visit, VisitDto>();
    }
}
