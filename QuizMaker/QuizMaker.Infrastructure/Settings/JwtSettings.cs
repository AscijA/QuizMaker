using System.ComponentModel.DataAnnotations;

namespace QuizMaker.Infrastructure.Settings;

public class JwtSettings {
    public const string SectionName = "JwtSettings";

    [Required] public string Secret { get; set; } = string.Empty;
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; }
}