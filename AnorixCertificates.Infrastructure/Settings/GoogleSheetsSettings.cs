namespace AnorixCertificates.Infrastructure.Settings;

public class GoogleSheetsSettings
{
    public string SpreadsheetId { get; set; } = string.Empty;
    public string SheetName { get; set; } = string.Empty;
    public string CredentialsPath { get; set; } = string.Empty;
}