using System.ComponentModel.DataAnnotations;

namespace Bold.Integration.Base.Configuration;

public class AzureSettings
{
    [Required]
    public required string TenantId { get; init; }
    [Required]
    public required string AppId { get; init; }
    [Required]
    public required string AppSecret { get; init; }
    [Required]
    public required string Scope { get; init; }
    public required AzureMonitorSettings? Monitor { get; init; }
}
public class AzureMonitorSettings
{
    public string ConnectionString { get; init; } = string.Empty;
}