namespace PatientManagement.Application.DTOs;

public class MedicationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Dosage { get; set; } = null!;
    public string Frequency { get; set; } = null!;
    public string Duration { get; set; } = null!;
    public string? Instructions { get; set; }
}
