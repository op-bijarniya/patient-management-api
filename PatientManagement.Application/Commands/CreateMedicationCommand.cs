namespace PatientManagement.Application.Commands;

public class CreateMedicationCommand
{
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string Duration { get; set; } = null!;
    public string? Instructions { get; set; }
}
