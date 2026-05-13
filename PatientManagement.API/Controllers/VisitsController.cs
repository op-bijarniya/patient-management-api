using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Commands;
using PatientManagement.Application.Interfaces;
using PatientManagement.Application.Exceptions;

namespace PatientManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class VisitsController : ControllerBase
{
    private readonly IVisitService _visitService;

    public VisitsController(IVisitService visitService)
    {
        _visitService = visitService;
    }

    [HttpPost]
    public async Task<IActionResult> RecordVisit([FromBody] CreateVisitCommand command)
    {
        try
        {
            var result = await _visitService.RecordVisitAsync(command);
            return CreatedAtAction(nameof(GetVisit), new { id = result.Id }, result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetVisit(int id)
    {
        var result = await _visitService.GetVisitAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("patient/{patientId}")]
    public async Task<IActionResult> GetVisitsByPatient(int patientId)
    {
        var results = await _visitService.GetVisitsByPatientAsync(patientId);
        return Ok(results);
    }
}
