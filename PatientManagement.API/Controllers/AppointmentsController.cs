using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Commands;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;

namespace PatientManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPost]
    public async Task<IActionResult> ScheduleAppointment([FromBody] ScheduleAppointmentCommand command)
    {
        try
        {
            var result = await _appointmentService.ScheduleAppointmentAsync(command);
            return CreatedAtAction(nameof(GetAppointment), new { id = result.Id }, result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointment(int id)
    {
        var result = await _appointmentService.GetAppointmentAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointmentsByDate([FromQuery] DateTime date)
    {
        var results = await _appointmentService.GetAppointmentsByDateAsync(date);
        return Ok(results);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateAppointmentStatus(int id, [FromBody] UpdateAppointmentStatusRequest request)
    {
        try
        {
            var result = await _appointmentService.UpdateAppointmentStatusAsync(id, request.Status);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}

public class UpdateAppointmentStatusRequest
{
    public string Status { get; set; } = null!;
}
