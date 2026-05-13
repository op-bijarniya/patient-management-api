using PatientManagement.Domain.Enums;

namespace PatientManagement.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; private set; }
    public Patient Patient { get; private set; } = null!;
    public DateTime ScheduledAt { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public string? Reason { get; private set; }

    private Appointment() { }

    public Appointment(int patientId, DateTime scheduledAt, string? reason = null)
    {
        PatientId = patientId;
        ScheduledAt = scheduledAt;
        Reason = reason?.Trim();
        Status = AppointmentStatus.Scheduled;
    }

    public void UpdateStatus(AppointmentStatus status)
    {
        Status = status;
    }

    public void UpdateReason(string? reason)
    {
        Reason = reason?.Trim();
    }
}
