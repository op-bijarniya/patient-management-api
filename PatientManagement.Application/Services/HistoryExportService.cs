using System.Globalization;
using System.Text;
using AutoMapper;
using CsvHelper;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PatientManagement.Application.DTOs;
using PatientManagement.Application.Exceptions;
using PatientManagement.Application.Interfaces;
using PatientManagement.Domain.Interfaces;

namespace PatientManagement.Application.Services;

public class HistoryExportService : IHistoryExportService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;

    public HistoryExportService(IPatientRepository patientRepository, IMapper mapper)
    {
        _patientRepository = patientRepository;
        _mapper = mapper;
    }

    public async Task<byte[]> ExportPatientHistoryCsvAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var history = await GetPatientHistoryAsync(patientId, cancellationToken);
        if (history == null)
        {
            throw new NotFoundException($"Patient with id {patientId} was not found.");
        }

        var builder = new StringBuilder();
        using (var writer = new StringWriter(builder))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteField("Patient Name");
            csv.WriteField("Date of Birth");
            csv.WriteField("Gender");
            csv.WriteField("Phone Number");
            csv.WriteField("Email");
            csv.WriteField("Address");
            csv.NextRecord();

            csv.WriteField(history.Patient.FirstName + " " + history.Patient.LastName);
            csv.WriteField(history.Patient.DateOfBirth.ToString("yyyy-MM-dd"));
            csv.WriteField(history.Patient.Gender);
            csv.WriteField(history.Patient.PhoneNumber);
            csv.WriteField(history.Patient.Email);
            csv.WriteField(history.Patient.Address);
            csv.NextRecord();
            csv.NextRecord();

            csv.WriteField("Visit Date");
            csv.WriteField("Temperature (C)");
            csv.WriteField("Blood Pressure");
            csv.WriteField("Pulse");
            csv.WriteField("Complaints");
            csv.WriteField("Diagnosis");
            csv.WriteField("Medication");
            csv.WriteField("Dosage");
            csv.WriteField("Frequency");
            csv.WriteField("Duration");
            csv.WriteField("Instructions");
            csv.NextRecord();

            foreach (var visit in history.Visits)
            {
                if (visit.Medications == null || visit.Medications.Count == 0)
                {
                    WriteVisitRow(csv, visit, null);
                    continue;
                }

                var isFirstMedication = true;
                foreach (var medication in visit.Medications)
                {
                    WriteVisitRow(csv, visit, medication, isFirstMedication);
                    isFirstMedication = false;
                }
            }
        }

        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    public async Task<byte[]> ExportPatientHistoryPdfAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var history = await GetPatientHistoryAsync(patientId, cancellationToken);
        if (history == null)
        {
            throw new NotFoundException($"Patient with id {patientId} was not found.");
        }

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(20);
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Patient Visit History").FontSize(20).SemiBold();
                        col.Item().Text($"{history.Patient.FirstName} {history.Patient.LastName}").FontSize(14);
                        col.Item().Text($"Phone: {history.Patient.PhoneNumber}");
                        col.Item().Text($"Email: {history.Patient.Email}");
                    });
                });

                page.Content().Column(column =>
                {
                    foreach (var visit in history.Visits)
                    {
                        column.Item().PaddingVertical(8).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(visitColumn =>
                        {
                            visitColumn.Item().Text($"Visit Date: {visit.VisitDate:yyyy-MM-dd}").Bold();
                            visitColumn.Item().Text($"Temperature: {visit.TemperatureC} °C");
                            visitColumn.Item().Text($"Blood Pressure: {visit.BloodPressure}");
                            visitColumn.Item().Text($"Pulse: {visit.Pulse}");
                            visitColumn.Item().Text($"Complaints: {visit.Complaints}");
                            visitColumn.Item().Text($"Diagnosis: {visit.Diagnosis}");
                            if (!string.IsNullOrWhiteSpace(visit.Notes))
                            {
                                visitColumn.Item().Text($"Notes: {visit.Notes}");
                            }
                            if (visit.Medications?.Count > 0)
                            {
                                visitColumn.Item().Text("Medications:").Bold();
                                visitColumn.Item().Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(80);
                                        columns.ConstantColumn(80);
                                        columns.ConstantColumn(80);
                                        columns.RelativeColumn();
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Name");
                                        header.Cell().Element(CellStyle).Text("Dosage");
                                        header.Cell().Element(CellStyle).Text("Frequency");
                                        header.Cell().Element(CellStyle).Text("Duration");
                                        header.Cell().Element(CellStyle).Text("Instructions");
                                    });

                                    foreach (var medication in visit.Medications)
                                    {
                                        table.Cell().Element(CellStyle).Text(medication.Name);
                                        table.Cell().Element(CellStyle).Text(medication.Dosage);
                                        table.Cell().Element(CellStyle).Text(medication.Frequency);
                                        table.Cell().Element(CellStyle).Text(medication.Duration);
                                        table.Cell().Element(CellStyle).Text(medication.Instructions);
                                    }
                                });
                            }
                        });
                    }
                });

                page.Footer().AlignCenter().Text("Generated by Patient Management System")
                    .FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
            });
        });

        return document.GeneratePdf();

        static IContainer CellStyle(IContainer container)
        {
            return container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
        }
    }

    private static void WriteVisitRow(CsvWriter csv, VisitDto visit, MedicationDto? medication, bool writeVisitDetails = true)
    {
        csv.WriteField(writeVisitDetails ? visit.VisitDate.ToString("yyyy-MM-dd") : string.Empty);
        csv.WriteField(writeVisitDetails ? visit.TemperatureC.ToString(CultureInfo.InvariantCulture) : string.Empty);
        csv.WriteField(writeVisitDetails ? visit.BloodPressure : string.Empty);
        csv.WriteField(writeVisitDetails ? visit.Pulse.ToString() : string.Empty);
        csv.WriteField(writeVisitDetails ? visit.Complaints : string.Empty);
        csv.WriteField(writeVisitDetails ? visit.Diagnosis : string.Empty);

        if (medication == null)
        {
            csv.WriteField(string.Empty);
            csv.WriteField(string.Empty);
            csv.WriteField(string.Empty);
            csv.WriteField(string.Empty);
            csv.WriteField(string.Empty);
        }
        else
        {
            csv.WriteField(medication.Name);
            csv.WriteField(medication.Dosage);
            csv.WriteField(medication.Frequency);
            csv.WriteField(medication.Duration);
            csv.WriteField(medication.Instructions);
        }

        csv.NextRecord();
    }

    private async Task<PatientHistoryDto?> GetPatientHistoryAsync(int patientId, CancellationToken cancellationToken)
    {
        var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        if (patient == null)
        {
            return null;
        }

        var patientDto = _mapper.Map<PatientDto>(patient);
        var visitDtos = _mapper.Map<IReadOnlyCollection<VisitDto>>(patient.Visits.OrderByDescending(v => v.VisitDate));
        return new PatientHistoryDto
        {
            Patient = patientDto,
            Visits = visitDtos
        };
    }
}
