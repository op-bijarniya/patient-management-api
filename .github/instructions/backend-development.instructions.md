---
name: backend-development
description: "Use when: Working on .NET Core backend code. Provides Clean Architecture guidelines, SOLID principles, and .NET best practices for the Patient Management API."
---

# Backend Development Instructions

## Project Structure (Clean Architecture)

```
PatientManagement/
├── PatientManagement.API/          # REST API Layer
│   ├── Controllers/                # HTTP endpoints (thin layer)
│   ├── Filters/                    # Cross-cutting concerns
│   ├── Program.cs                  # Application entry point
│   └── appsettings.json            # Configuration
│
├── PatientManagement.Application/  # Business Logic Layer
│   ├── Services/                   # Business services
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Service contracts
│   ├── MappingProfiles/            # AutoMapper configurations
│   ├── Exceptions/                 # Custom exceptions
│   └── Commands/Queries/           # CQRS patterns
│
├── PatientManagement.Domain/       # Domain Layer
│   ├── Entities/                   # Core business entities
│   ├── ValueObjects/               # Immutable value objects
│   ├── Aggregates/                 # Aggregate roots
│   ├── Interfaces/                 # Repository interfaces
│   ├── Events/                     # Domain events
│   └── Specifications/             # Query specifications
│
├── PatientManagement.Infrastructure/ # Infrastructure Layer
│   ├── Data/
│   │   ├── Configuration/          # Entity configurations
│   │   ├── Migrations/             # EF Core migrations
│   │   └── DbContext.cs            # Database context
│   ├── Repositories/               # Repository implementations
│   ├── Services/                   # External service implementations
│   └── Extensions/                 # Service registrations
│
└── PatientManagement.Tests/        # Testing Layer
    ├── Unit/                       # Unit tests
    ├── Integration/                # Integration tests
    └── TestData/                   # Test fixtures
```

## Architecture Principles

### 1. Clean Architecture Layers
- **API Layer**: Only HTTP concerns, delegates to Application layer
- **Application Layer**: Business logic, orchestration, DTOs
- **Domain Layer**: Core business rules, entities, no dependencies
- **Infrastructure Layer**: Data access, external services

### 2. SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes substitute base classes
- **Interface Segregation**: Clients depend on small interfaces
- **Dependency Inversion**: Depend on abstractions, not concretions

### 3. CQRS Pattern
Use Command Query Responsibility Segregation:
- **Commands**: Change state (CreatePatient, UpdateVitals)
- **Queries**: Read data (GetPatient, SearchPatients)
- **Handlers**: Process commands/queries

## Coding Standards

### Naming Conventions
```csharp
// Classes and Methods: PascalCase
public class PatientService { }
public void CreatePatient(CreatePatientCommand cmd) { }

// Properties and Fields: PascalCase
public string FirstName { get; set; }
private readonly IPatientRepository _patientRepository;

// Parameters: camelCase
public void UpdatePatient(int patientId, UpdatePatientDto dto) { }

// Interfaces: I + PascalCase
public interface IPatientRepository { }

// Private fields: _ + camelCase
private readonly IMapper _mapper;
```

### Code Style
- Line length: 120 characters maximum
- Use XML documentation for public APIs
- Use async/await for I/O operations
- Prefer LINQ over loops where appropriate
- Use guard clauses for parameter validation

### Example Controller
```csharp
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PatientsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient(CreatePatientCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetPatient), new { id = result.Id }, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var query = new GetPatientQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
```

### Example Service
```csharp
public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;

    public PatientService(IPatientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PatientDto> CreatePatient(CreatePatientCommand command)
    {
        var patient = _mapper.Map<Patient>(command);
        await _repository.AddAsync(patient);
        return _mapper.Map<PatientDto>(patient);
    }
}
```

### Example Entity
```csharp
public class Patient : BaseEntity
{
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public string PhoneNumber { get; private set; }

    private readonly List<Appointment> _appointments = new();
    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();

    private Patient() { } // EF Core constructor

    public Patient(string firstName, string lastName, DateTime dateOfBirth, string phoneNumber)
    {
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }

    public void UpdateContactInfo(string phoneNumber)
    {
        PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
    }
}
```

## Testing Guidelines

### Unit Testing (xUnit + Moq)
- Test one behavior per test
- Use descriptive test names: `[MethodName]_[Scenario]_[ExpectedBehavior]`
- Mock external dependencies
- Test both happy path and error cases

```csharp
[Fact]
public async Task CreatePatient_WithValidData_ReturnsPatientWithId()
{
    // Arrange
    var mockRepo = new Mock<IPatientRepository>();
    var service = new PatientService(mockRepo.Object, _mapper);
    var command = new CreatePatientCommand { FirstName = "John", LastName = "Doe" };

    // Act
    var result = await service.CreatePatient(command);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.Id > 0);
    mockRepo.Verify(r => r.AddAsync(It.IsAny<Patient>()), Times.Once);
}
```

### Integration Testing
- Test real database operations
- Test API endpoints end-to-end
- Use test database (in-memory or container)

### Test Coverage Target
- Minimum 80% code coverage
- Focus on business logic, not infrastructure
- Exclude generated code from coverage

## Database Design

### Entity Framework Core
- Use Code First approach
- Configure entities in separate configuration classes
- Use migrations for schema changes
- Enable sensitive data logging in development only

### Example Configuration
```csharp
public class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.LastName).HasMaxLength(100).IsRequired();
        builder.Property(p => p.PhoneNumber).HasMaxLength(20);

        builder.HasMany(p => p.Appointments)
               .WithOne(a => a.Patient)
               .HasForeignKey(a => a.PatientId);
    }
}
```

## Performance Considerations

### Database Optimization
- Use indexes on frequently queried columns
- Avoid N+1 query problems with Include/ThenInclude
- Use AsNoTracking for read-only queries
- Implement pagination for large result sets

### Caching Strategy
- Use in-memory cache for frequently accessed data
- Consider distributed cache for multi-instance deployments
- Cache expensive computations

### Asynchronous Programming
- Use async/await throughout the application
- Avoid blocking calls in async methods
- Use ConfigureAwait(false) in library code

## Security Guidelines

### Input Validation
- Validate all inputs on API boundaries
- Use Data Annotations or Fluent Validation
- Sanitize user inputs to prevent injection attacks

### Authentication & Authorization
- Use JWT tokens for API authentication
- Implement role-based authorization
- Validate tokens on each request

### Data Protection
- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Implement proper CORS policies

## Error Handling

### Global Exception Handling
- Use middleware for unhandled exceptions
- Return appropriate HTTP status codes
- Log errors with correlation IDs

### Custom Exceptions
```csharp
public class PatientNotFoundException : Exception
{
    public PatientNotFoundException(int patientId)
        : base($"Patient with ID {patientId} was not found.")
    {
        PatientId = patientId;
    }

    public int PatientId { get; }
}
```

## Logging

### Structured Logging
- Use Serilog or Microsoft.Extensions.Logging
- Log at appropriate levels (Debug, Info, Warning, Error)
- Include contextual information
- Never log sensitive data

### Example Logging
```csharp
public async Task<PatientDto> GetPatient(int id)
{
    _logger.LogInformation("Retrieving patient with ID {PatientId}", id);

    var patient = await _repository.GetByIdAsync(id);
    if (patient == null)
    {
        _logger.LogWarning("Patient with ID {PatientId} not found", id);
        throw new PatientNotFoundException(id);
    }

    return _mapper.Map<PatientDto>(patient);
}
```

## Development Workflow

### Creating a New Feature
1. Add domain entities in Domain layer
2. Define repository interfaces in Domain
3. Implement repository in Infrastructure
4. Create commands/queries in Application
5. Add service methods in Application
6. Create API endpoints in API layer
7. Write unit tests for all layers
8. Update database migrations if needed

### Code Review Checklist
- [ ] SOLID principles followed
- [ ] Clean Architecture layers respected
- [ ] Unit tests written and passing
- [ ] Code coverage maintained
- [ ] Naming conventions followed
- [ ] XML documentation added
- [ ] No sensitive data logged
- [ ] Error handling implemented
- [ ] Performance considerations addressed

## Tools & Dependencies

### Essential Packages
- `Microsoft.EntityFrameworkCore` - ORM
- `AutoMapper` - Object mapping
- `MediatR` - CQRS implementation
- `FluentValidation` - Input validation
- `Serilog` - Structured logging
- `xunit` - Testing framework
- `Moq` - Mocking library

### Development Tools
- Visual Studio 2022 or VS Code
- .NET CLI
- SQL Server or PostgreSQL
- Docker (for containerized development)

## Common Patterns

### Repository Pattern
```csharp
public interface IPatientRepository
{
    Task<Patient> GetByIdAsync(int id);
    Task<IEnumerable<Patient>> GetAllAsync();
    Task AddAsync(Patient patient);
    Task UpdateAsync(Patient patient);
    Task DeleteAsync(int id);
}

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Patient> GetByIdAsync(int id)
    {
        return await _context.Patients
            .Include(p => p.Appointments)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
```

### Unit of Work Pattern
```csharp
public interface IUnitOfWork
{
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### Service Registration
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(DependencyInjection).Assembly);
        services.AddAutoMapper(typeof(DependencyInjection).Assembly);

        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IAppointmentService, AppointmentService>();

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPatientRepository, PatientRepository>();

        return services;
    }
}
```

This instruction file provides comprehensive guidance for backend development in the Patient Management application, ensuring consistency and quality across all .NET Core development work.