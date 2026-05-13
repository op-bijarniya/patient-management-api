namespace PatientManagement.Application.DTOs;

public class PatientHistoryDto
{
    public PatientDto Patient { get; set; } = null!;
    public IReadOnlyCollection<VisitDto> Visits { get; set; } = Array.Empty<VisitDto>();
}
