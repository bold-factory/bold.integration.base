using System.ComponentModel.DataAnnotations;

namespace Bold.Integration.Base.Configuration;

public class SendGridSettings
{
    [Required]
    public required string ApiKey { get; init; }
    [Required]
    public required string Destination { get; init; }
}