using System.ComponentModel.DataAnnotations;

namespace Razmx.App.Models;

public record RegisterRequest
{
    [Required][EmailAddress]
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;

    [Compare(nameof(Password))]
    public string Confirm { get; init; } = string.Empty;
}