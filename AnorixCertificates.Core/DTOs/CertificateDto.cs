// AnorixCertificates.Core/DTOs/CertificateDto.cs

namespace AnorixCertificates.Core.DTOs;

public class VerifyCertificateResponse
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public CertificateData? Data { get; set; }
    public DateTime VerifiedAt { get; set; } = DateTime.UtcNow;
}

public class CertificateData
{
    public string CertificateId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public string CourseType { get; set; } = string.Empty;
    public string LevelDescription { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public string QRCodeUrl { get; set; } = string.Empty;

    public string PdfUrl { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}

public class CreateCertificateRequest
{
    public string StudentName { get; set; } = string.Empty;

    // W, A, or B
    public string CoursePrefix { get; set; } = string.Empty;

    public string CourseName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class CreateCertificateResponse
{
    public bool Success { get; set; }
    public string CertificateId { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
    public string QRCodeBase64 { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public string CourseType { get; set; } = string.Empty;
    public string LevelDescription { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class UpdateStatusRequest
{
    public string Status { get; set; } = string.Empty;
}