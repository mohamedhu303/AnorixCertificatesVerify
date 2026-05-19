// AnorixCertificates.Core/Models/Certificate.cs
namespace AnorixCertificates.Core.Models;

public class Certificate
{
    public string CertificateId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string CourseType { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string StartDate { get; set; } = string.Empty;
    public string EndDate { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Instructor { get; set; } = string.Empty;
    public string CertificateType { get; set; } = string.Empty;
    public string QRCodeUrl { get; set; } = string.Empty;
    public string CertificateUrl { get; set; } = string.Empty;
    public string PdfLink { get; set; } = string.Empty;
    public string SlideLink { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string EmailSent { get; set; } = "NO";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Active";
}

public enum CoursePrefix
{
    W, // Workshops UI UX
    A, // Advanced UI UX
    B  // Basic UI UX
}