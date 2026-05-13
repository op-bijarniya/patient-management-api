using AutoMapper;
using Moq;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.MappingProfiles;
using PatientManagement.Application.Services;
using PatientManagement.Domain.Entities;
using PatientManagement.Domain.Interfaces;
using Xunit;

namespace PatientManagement.Tests.Services;

public class PatientServiceTests
{
    private readonly IMapper _mapper;

    public PatientServiceTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ApplicationMappingProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public async Task CreatePatientAsync_WithValidData_ReturnsPatientDto()
    {
        // Arrange
        var repositoryMock = new Mock<IPatientRepository>(MockBehavior.Strict);
        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Patient p, CancellationToken _) => p);
        repositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new PatientService(repositoryMock.Object, _mapper);
        var command = new CreatePatientCommand
        {
            FirstName = "Asha",
            LastName = "Rao",
            DateOfBirth = new DateTime(1990, 4, 15),
            Gender = "Female",
            PhoneNumber = "1234567890",
            Email = "asha@example.com",
            Address = "123 Clinic Road"
        };

        // Act
        var result = await service.CreatePatientAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(command.FirstName, result.FirstName);
        Assert.Equal(command.PhoneNumber, result.PhoneNumber);
        repositoryMock.Verify(x => x.AddAsync(It.IsAny<Patient>(), It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
