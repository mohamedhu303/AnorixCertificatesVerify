using Microsoft.AspNetCore.Mvc;
using AnorixCertificates.Core.DTOs;
using AnorixCertificates.Infrastructure.Services;

namespace AnorixCertificates.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private readonly GoogleSheetsService _sheetsService;
    private readonly CertificateService _certService;

    public CertificatesController(
        GoogleSheetsService sheetsService,
        CertificateService certService)
    {
        _sheetsService = sheetsService;
        _certService = certService;
    }

    // ── Test ──────────────────────────────
    [HttpGet("test")]
    public async Task<ActionResult> TestConnection()
    {
        var result = await _sheetsService.TestConnection();
        return Ok(new { message = result });
    }

    // ── Verify ────────────────────────────
    [HttpGet("verify/{id}")]
    public async Task<ActionResult<VerifyCertificateResponse>> Verify(string id)
    {
        if (!_certService.ValidateCertificateIdFormat(id))
            return Ok(new VerifyCertificateResponse
            {
                IsValid = false,
                Message = $"Invalid format: {id}"
            });

        var row = await _sheetsService.GetCertificateById(id);

        if (row == null)
            return Ok(new VerifyCertificateResponse
            {
                IsValid = false,
                Message = "Certificate not found"
            });

        if (SafeGet(row, 16) == "Revoked")
            return Ok(new VerifyCertificateResponse
            {
                IsValid = false,
                Message = "Certificate revoked"
            });

        var idInfo = _certService.DecodeCertificateId(id);

        var certData = new CertificateData
        {
            CertificateId = SafeGet(row, 0),
            StudentName = SafeGet(row, 1),
            CourseCategory = idInfo.CourseCategory,
            CourseType = SafeGet(row, 2),
            LevelDescription = idInfo.LevelDescription,
            CourseName = SafeGet(row, 3),
            StartDate = SafeGet(row, 4),
            EndDate = SafeGet(row, 5),
            Duration = SafeGet(row, 6),
            Instructor = SafeGet(row, 7),
            CertificateType = SafeGet(row, 8),
            QRCodeUrl = SafeGet(row, 9),
            PdfUrl = SafeGet(row, 11),
            ImageUrl = SafeGet(row, 17)
        };

        return Ok(new VerifyCertificateResponse
        {
            IsValid = true,
            Message = "Certificate is authentic and valid",
            Data = certData,
            VerifiedAt = DateTime.UtcNow
        });
    }

    // ── Get Image ─────────────────────────
    [HttpGet("image/{id}")]
    public async Task<IActionResult> GetCertificateImage(string id)
    {
        try
        {
            if (!_certService.ValidateCertificateIdFormat(id))
                return BadRequest("Invalid certificate ID");

            var row = await _sheetsService.GetCertificateById(id);
            if (row == null)
                return NotFound("Certificate not found");

            var imageUrl = SafeGet(row, 17);
            if (string.IsNullOrEmpty(imageUrl))
                return NotFound("No image available");

            var fileId = ExtractGoogleDriveFileId(imageUrl);
            if (string.IsNullOrEmpty(fileId))
                return NotFound("Invalid image URL format");

            var urlsToTry = new[]
            {
                $"https://drive.google.com/uc?export=view&id={fileId}",
                $"https://drive.google.com/thumbnail?id={fileId}&sz=w2000",
                $"https://lh3.googleusercontent.com/d/{fileId}",
                $"https://drive.google.com/uc?export=download&id={fileId}"
            };

            using var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(30);
            httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
                "AppleWebKit/537.36 Chrome/120.0.0.0 Safari/537.36");

            foreach (var url in urlsToTry)
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode) continue;

                    var contentType = response.Content.Headers
                                        .ContentType?.MediaType ?? "image/jpeg";

                    if (contentType.Contains("text/html")) continue;

                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    if (imageBytes.Length < 1000) continue;

                    Response.Headers["Cache-Control"] = "public, max-age=3600";
                    Response.Headers["Access-Control-Allow-Origin"] = "*";

                    return File(imageBytes, contentType);
                }
                catch { continue; }
            }

            return NotFound("Could not fetch image from Google Drive");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    // ── Decode ────────────────────────────
    [HttpGet("decode/{id}")]
    public ActionResult DecodeCertificateId(string id)
    {
        if (!_certService.ValidateCertificateIdFormat(id))
            return BadRequest(new { error = "Invalid format", expected = "X-N-XXXXXX" });

        var info = _certService.DecodeCertificateId(id);

        return Ok(new
        {
            id = info.FullId,
            prefix = info.Prefix,
            typeNumber = info.TypeNumber,
            randomCode = info.RandomCode,
            courseCategory = info.CourseCategory,
            courseFullName = info.CourseFullName,
            level = info.LevelDescription
        });
    }

    // ── All ───────────────────────────────
    [HttpGet("all")]
    public async Task<ActionResult> GetAll()
    {
        var rows = await _sheetsService.GetAllCertificates();

        var list = rows.Select(row => new
        {
            CertificateId = SafeGet(row, 0),
            StudentName = SafeGet(row, 1),
            CourseType = SafeGet(row, 2),
            CourseName = SafeGet(row, 3),
            EndDate = SafeGet(row, 5),
            Status = SafeGet(row, 16)
        }).ToList();

        return Ok(new { count = list.Count, certificates = list });
    }

    // ── Stats ─────────────────────────────
    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        var rows = await _sheetsService.GetAllCertificates();

        return Ok(new
        {
            total = rows.Count,
            byType = new
            {
                basic = rows.Count(r => SafeGet(r, 0).StartsWith("B-")),
                advanced = rows.Count(r => SafeGet(r, 0).StartsWith("A-")),
                workshop = rows.Count(r => SafeGet(r, 0).StartsWith("W-"))
            },
            byStatus = new
            {
                active = rows.Count(r => SafeGet(r, 16) == "Active"),
                revoked = rows.Count(r => SafeGet(r, 16) == "Revoked")
            }
        });
    }

    // ── Helpers ───────────────────────────
    private string SafeGet(IList<object> row, int index)
    {
        if (row == null || index >= row.Count) return string.Empty;
        return row[index]?.ToString() ?? string.Empty;
    }

    private string ExtractGoogleDriveFileId(string url)
    {
        if (string.IsNullOrEmpty(url)) return string.Empty;

        // /file/d/{id}/view
        var match = System.Text.RegularExpressions.Regex.Match(
            url, @"/file/d/([a-zA-Z0-9_-]+)");
        if (match.Success) return match.Groups[1].Value;

        // ?id={id} or &id={id}
        match = System.Text.RegularExpressions.Regex.Match(
            url, @"[?&]id=([a-zA-Z0-9_-]+)");
        if (match.Success) return match.Groups[1].Value;

        // /d/{id}/
        match = System.Text.RegularExpressions.Regex.Match(
            url, @"/d/([a-zA-Z0-9_-]+)");
        if (match.Success) return match.Groups[1].Value;

        return string.Empty;
    }
}