using System.Collections.Generic;

namespace PatientManagement.Application.Commands;

public class CreateVisitCommand
{
    public int PatientId { get; set; }
    public int? AppointmentId { get; set; }
    public DateTime VisitDate { get; set; }
    public decimal TemperatureC { get; set; }
    public string BloodPressure { get; set; } = null!;
    public int Pulse { get; set; }
    public string Complaints { get; set; } = null!;
    public string Diagnosis { get; set; } = null!;
    public string? Notes { get; set; }
    public List<CreateMedicationCommand> Medications { get; set; } = new();
}
