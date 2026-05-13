using System.Collections.Generic;

namespace PatientManagement.Domain.Entities;

public class Patient : BaseEntity
{
    private readonly List<Appointment> _appointments = new();
    private readonly List<Visit> _visits = new();

    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public string Gender { get; private set; } = null!;
    public string PhoneNumber { get; private set; } = null!;
    public string? Email { get; private set; }
    public string? Address { get; private set; }

    public IReadOnlyCollection<Appointment> Appointments => _appointments.AsReadOnly();
    public IReadOnlyCollection<Visit> Visits => _visits.AsReadOnly();

    private Patient() { }

    public Patient(string firstName, string lastName, DateTime dateOfBirth, string gender, string phoneNumber, string? email = null, string? address = null)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName?.Trim() ?? throw new ArgumentNullException(nameof(lastName));
        DateOfBirth = dateOfBirth;
        Gender = gender?.Trim() ?? throw new ArgumentNullException(nameof(gender));
        PhoneNumber = phoneNumber?.Trim() ?? throw new ArgumentNullException(nameof(phoneNumber));
        Email = email?.Trim();
        Address = address?.Trim();
    }

    public void UpdateContact(string phoneNumber, string? email, string? address)
    {
        PhoneNumber = phoneNumber?.Trim() ?? throw new ArgumentNullException(nameof(phoneNumber));
        Email = email?.Trim();
        Address = address?.Trim();
    }

    public void UpdateDemographics(string firstName, string lastName, DateTime dateOfBirth, string gender)
    {
        FirstName = firstName?.Trim() ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName?.Trim() ?? throw new ArgumentNullException(nameof(lastName));
        DateOfBirth = dateOfBirth;
        Gender = gender?.Trim() ?? throw new ArgumentNullException(nameof(gender));
    }

    public void AddAppointment(Appointment appointment)
    {
        if (appointment == null) throw new ArgumentNullException(nameof(appointment));
        _appointments.Add(appointment);
    }

    public void AddVisit(Visit visit)
    {
        if (visit == null) throw new ArgumentNullException(nameof(visit));
        _visits.Add(visit);
    }
}
