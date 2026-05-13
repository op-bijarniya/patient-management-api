using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientManagement.Application.Commands;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PatientsController : ControllerBase
{
    private readonly IPatientService _patientService;
    private readonly IHistoryExportService _historyExportService;

    public PatientsController(IPatientService patientService, IHistoryExportService historyExportService)
    {
        _patientService = patientService;
        _historyExportService = historyExportService;
    }

    [HttpPost]
    public async Task<IActionResult> CreatePatient([FromBody] CreatePatientCommand command)
    {
        var result = await _patientService.CreatePatientAsync(command);
        return CreatedAtAction(nameof(GetPatient), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePatient(int id, [FromBody] UpdatePatientCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("Patient ID mismatch.");
        }

        try
        {
            var result = await _patientService.UpdatePatientAsync(command);
            return Ok(result);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatient(int id)
    {
        var result = await _patientService.GetPatientAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/history/csv")]
    public async Task<IActionResult> ExportPatientHistoryCsv(int id)
    {
        try
        {
            var content = await _historyExportService.ExportPatientHistoryCsvAsync(id);
            return File(content, "text/csv", $"patient_{id}_history.csv");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("{id}/history/pdf")]
    public async Task<IActionResult> ExportPatientHistoryPdf(int id)
    {
        try
        {
            var content = await _historyExportService.ExportPatientHistoryPdfAsync(id);
            return File(content, "application/pdf", $"patient_{id}_history.pdf");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchPatients([FromQuery] string? query)
    {
        var results = await _patientService.SearchPatientsAsync(query);
        return Ok(results);
    }

    [HttpGet("recent")]
    public async Task<IActionResult> GetRecentPatients([FromQuery] int count = 20)
    {
        var results = await _patientService.GetRecentPatientsAsync(count);
        return Ok(results);
    }
}
