namespace PatientManagement.Domain.Entities;

public class Medication : BaseEntity
{
    public int VisitId { get; private set; }
    public Visit Visit { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Dosage { get; private set; } = null!;
    public string Frequency { get; private set; } = null!;
    public string Duration { get; private set; } = null!;
    public string? Instructions { get; private set; }

    private Medication() { }

    public Medication(int visitId, string name, string dosage, string frequency, string duration, string? instructions = null)
    {
        VisitId = visitId;
        Name = name?.Trim() ?? throw new ArgumentNullException(nameof(name));
        Dosage = dosage?.Trim() ?? throw new ArgumentNullException(nameof(dosage));
        Frequency = frequency?.Trim() ?? throw new ArgumentNullException(nameof(frequency));
        Duration = duration?.Trim() ?? throw new ArgumentNullException(nameof(duration));
        Instructions = instructions?.Trim();
    }
}
