using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Microsoft.Extensions.Configuration;

namespace AnorixCertificates.Infrastructure.Services;

public class GoogleSheetsService
{
    private readonly SheetsService _sheetsService;
    private readonly string _spreadsheetId;
    private readonly string _sheetName;

    public GoogleSheetsService(IConfiguration configuration)
    {
        _spreadsheetId = configuration["GoogleSheets:SpreadsheetId"]
                         ?? throw new ArgumentNullException("SpreadsheetId not configured");

        _sheetName = configuration["GoogleSheets:SheetName"] ?? "Certificates";

        var credentialsPath = configuration["GoogleSheets:CredentialsPath"]
                              ?? "credentials/google-service-account.json";

        GoogleCredential credential;

        using (var stream = new FileStream(
            credentialsPath, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential
                .FromStream(stream)
                .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
        }

        _sheetsService = new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = "AnorixCertificates"
        });
    }

    public async Task<string> TestConnection()
    {
        try
        {
            var request = _sheetsService.Spreadsheets.Values.Get(
                _spreadsheetId, $"{_sheetName}!A1:A1");
            var response = await request.ExecuteAsync();
            return "Connection successful!";
        }
        catch (Exception ex)
        {
            return $"Connection failed: {ex.Message}";
        }
    }

    public async Task<IList<object>?> GetCertificateById(string id)
    {
        try
        {
            var request = _sheetsService.Spreadsheets.Values.Get(
                _spreadsheetId, $"{_sheetName}!A:R");
            var response = await request.ExecuteAsync();
            var rows = response.Values;

            if (rows == null || rows.Count == 0) return null;

            return rows.FirstOrDefault(row =>
                row.Count > 0 &&
                row[0]?.ToString()?.Equals(
                    id, StringComparison.OrdinalIgnoreCase) == true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting certificate: {ex.Message}");
            return null;
        }
    }

    public async Task<IList<IList<object>>> GetAllCertificates()
    {
        try
        {
            var request = _sheetsService.Spreadsheets.Values.Get(
                _spreadsheetId, $"{_sheetName}!A:R");
            var response = await request.ExecuteAsync();
            var rows = response.Values;

            if (rows == null || rows.Count == 0)
                return new List<IList<object>>();

            return rows.Skip(1).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting all certificates: {ex.Message}");
            return new List<IList<object>>();
        }
    }
}