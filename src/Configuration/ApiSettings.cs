using System.ComponentModel.DataAnnotations;

namespace Bold.Integration.Base.Configuration;

public class ApiSettings
{
    [Required]
    public required BoldApiSettings Bold { get; init; }
}
public class BoldApiSettings
{
    [Required]
    public string BaseUrl { get; init; } = string.Empty;
}