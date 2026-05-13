namespace PatientManagement.Application.DTOs;

public class VisitDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int? AppointmentId { get; set; }
    public DateTime VisitDate { get; set; }
    public decimal TemperatureC { get; set; }
    public string BloodPressure { get; set; } = null!;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = null!;
    public string Diagnosis { get; set; } = null!;
    public string? Notes { get; set; }
    public IReadOnlyCollection<MedicationDto> Medications { get; set; } = Array.Empty<MedicationDto>();
}
