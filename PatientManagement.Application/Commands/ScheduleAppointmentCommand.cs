namespace PatientManagement.Application.Commands;

public class ScheduleAppointmentCommand
{
    public int PatientId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string? Reason { get; set; }
}
