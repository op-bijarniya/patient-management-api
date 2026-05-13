using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using PatientManagement.Infrastructure.Data;
using PatientManagement.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSection = builder.Configuration.GetSection("Jwt");

if (!jwtSection.Exists())
{
    throw new Exception("JWT section is missing from configuration.");
}

var jwtSettings = jwtSection.Get<JwtSettings>();

if (jwtSettings == null)
{
    throw new Exception("JWT settings binding failed.");
}


// Add CORS policy to allow requests from Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCORS",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()!;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddPatientManagementInfrastructure(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PatientManagementDbContext>();
    dbContext.Database.Migrate();

    var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    if (await userRepo.GetByUsernameAsync("doctor") == null)
    {
        var defaultUser = new User("doctor", passwordHasher.HashPassword("Doctor@123"), "Doctor");
        await userRepo.AddAsync(defaultUser);
        await userRepo.SaveChangesAsync();
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowCORS");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
