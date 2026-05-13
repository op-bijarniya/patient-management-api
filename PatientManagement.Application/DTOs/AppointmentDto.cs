namespace PatientManagement.Application.DTOs;

public class AppointmentDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = null!;
    public string? Reason { get; set; }
}
