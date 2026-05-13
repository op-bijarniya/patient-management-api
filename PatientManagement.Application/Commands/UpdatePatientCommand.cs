namespace PatientManagement.Application.Commands;

public class UpdatePatientCommand
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string? Email { get; set; }
    public string? Address { get; set; }
}
