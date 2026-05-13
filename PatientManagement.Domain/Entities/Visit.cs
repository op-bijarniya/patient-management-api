using System.Collections.Generic;

namespace PatientManagement.Domain.Entities;

public class Visit : BaseEntity
{
    private readonly List<Medication> _medications = new();

    public int PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;
    public int? AppointmentId { get; private set; }
    public Appointment? Appointment { get; private set; }
    public DateTime VisitDate { get; private set; }
    public decimal TemperatureC { get; private set; }
    public string BloodPressure { get; private set; } = null!;
    public int Pulse { get; private set; }
    public string Complaints { get; private set; } = null!;
    public string Diagnosis { get; private set; } = null!;
    public string? Notes { get; private set; }

    public IReadOnlyCollection<Medication> Medications => _medications.AsReadOnly();

    private Visit() { }

    public Visit(int patientId, DateTime visitDate, decimal temperatureC, string bloodPressure, int pulse, string complaints, string diagnosis, string? notes = null, int? appointmentId = null)
    {
        PatientId = patientId;
        AppointmentId = appointmentId;
        VisitDate = visitDate;
        TemperatureC = temperatureC;
        BloodPressure = bloodPressure?.Trim() ?? throw new ArgumentNullException(nameof(bloodPressure));
        Pulse = pulse;
        Complaints = complaints?.Trim() ?? throw new ArgumentNullException(nameof(complaints));
        Diagnosis = diagnosis?.Trim() ?? throw new ArgumentNullException(nameof(diagnosis));
        Notes = notes?.Trim();
    }

    public void AddMedication(Medication medication)
    {
        if (medication == null) throw new ArgumentNullException(nameof(medication));
        _medications.Add(medication);
    }
}
