namespace PatientManagement.Application.Interfaces;

public interface IHistoryExportService
{
    Task<byte[]> ExportPatientHistoryCsvAsync(int patientId, CancellationToken cancellationToken = default);
    Task<byte[]> ExportPatientHistoryPdfAsync(int patientId, CancellationToken cancellationToken = default);
}
