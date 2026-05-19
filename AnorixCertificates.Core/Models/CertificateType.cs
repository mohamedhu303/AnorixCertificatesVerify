// AnorixCertificates.Core/Models/CertificateType.cs

namespace AnorixCertificates.Core.Models;

public static class CourseTypeConfig
{
    public static readonly Dictionary<string, CourseTypeInfo> CourseTypes = new()
    {
        {
            "W", new CourseTypeInfo
            {
                Prefix = "W",
                TypeNumber = "1",
                CourseCategory = "UI UX",
                FullName = "Workshops UI UX",
                Description = "Workshop Level"
            }
        },
        {
            "A", new CourseTypeInfo
            {
                Prefix = "A",
                TypeNumber = "1",
                CourseCategory = "UI UX",
                FullName = "Advanced UI UX",
                Description = "Advanced Level"
            }
        },
        {
            "B", new CourseTypeInfo
            {
                Prefix = "B",
                TypeNumber = "1",
                CourseCategory = "UI UX",
                FullName = "Basic UI UX",
                Description = "Basic Level"
            }
        }
    };
}

public class CourseTypeInfo
{
    public string Prefix { get; set; } = string.Empty;
    public string TypeNumber { get; set; } = string.Empty;
    public string CourseCategory { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}