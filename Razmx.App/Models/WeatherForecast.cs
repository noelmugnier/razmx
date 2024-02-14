namespace Razmx.App.Models;

public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary, string Id)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public string Id { get; set; } = Guid.NewGuid().ToString();
}