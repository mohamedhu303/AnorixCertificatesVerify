// AnorixCertificates.Infrastructure/Services/CertificateService.cs

using AnorixCertificates.Core.Models;

namespace AnorixCertificates.Infrastructure.Services;

public class CertificateService
{
    private static readonly char[] _chars =
        "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();

    // ✅ توليد الـ ID الجديد: B-1-AB12CD
    public string GenerateCertificateId(string prefix)
    {
        // التحقق من الـ Prefix
        if (!CourseTypeConfig.CourseTypes.TryGetValue(
            prefix.ToUpper(), out var courseInfo))
        {
            throw new ArgumentException(
                $"Invalid prefix: {prefix}. Must be W, A, or B");
        }

        // توليد الـ 6 Characters العشوائية
        var random = new Random();
        var code = new char[6];
        for (int i = 0; i < 6; i++)
        {
            code[i] = _chars[random.Next(_chars.Length)];
        }

        // B-1-AB12CD
        return $"{prefix.ToUpper()}-{courseInfo.TypeNumber}-{new string(code)}";
    }

    // ✅ الحصول على معلومات الكورس
    public CourseTypeInfo GetCourseInfo(string prefix)
    {
        if (!CourseTypeConfig.CourseTypes.TryGetValue(
            prefix.ToUpper(), out var courseInfo))
        {
            throw new ArgumentException($"Invalid prefix: {prefix}");
        }
        return courseInfo;
    }

    // ✅ التحقق من صحة الـ ID Format
    // Format: B-1-AB12CD
    public bool ValidateCertificateIdFormat(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;

        // Pattern: [WAB]-[0-9]-[A-Z0-9]{6}
        var pattern = @"^[WAB]-[0-9]-[A-Z0-9]{6}$";
        return System.Text.RegularExpressions.Regex
            .IsMatch(id, pattern);
    }

    // ✅ فك تشفير الـ ID وإرجاع معلوماته
    public CertificateIdInfo DecodeCertificateId(string id)
    {
        if (!ValidateCertificateIdFormat(id))
            throw new ArgumentException($"Invalid ID format: {id}");

        var parts = id.Split('-');
        // parts[0] = B
        // parts[1] = 1
        // parts[2] = AB12CD

        var prefix = parts[0];
        var typeNumber = parts[1];
        var code = parts[2];

        var courseInfo = CourseTypeConfig.CourseTypes
            .GetValueOrDefault(prefix);

        return new CertificateIdInfo
        {
            FullId = id,
            Prefix = prefix,
            TypeNumber = typeNumber,
            RandomCode = code,
            CourseCategory = courseInfo?.CourseCategory ?? "Unknown",
            CourseFullName = courseInfo?.FullName ?? "Unknown",
            LevelDescription = courseInfo?.Description ?? "Unknown"
        };
    }
}

public class CertificateIdInfo
{
    public string FullId { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public string TypeNumber { get; set; } = string.Empty;
    public string RandomCode { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public string CourseFullName { get; set; } = string.Empty;
    public string LevelDescription { get; set; } = string.Empty;
}