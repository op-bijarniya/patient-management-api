using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;

namespace PatientManagement.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginCommand command, CancellationToken cancellationToken = default);
}
