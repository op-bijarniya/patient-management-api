using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.MappingProfiles;
using PatientManagement.Application.Services;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;
using PatientManagement.Infrastructure.Repositories;

namespace PatientManagement.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPatientManagementInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PatientManagementDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=patientmanagement.db");
            options.EnableSensitiveDataLogging();
        });

        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        services.AddScoped<IVisitRepository, VisitRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddAutoMapper(typeof(ApplicationMappingProfile));
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IHistoryExportService, HistoryExportService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IVisitService, VisitService>();

        return services;
    }
}
