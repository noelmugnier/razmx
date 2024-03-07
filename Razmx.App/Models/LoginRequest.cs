using System.ComponentModel.DataAnnotations;

namespace Razmx.App.Models;

public record LoginRequest
{
    public int Id { get; set; }
    [Required][EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}