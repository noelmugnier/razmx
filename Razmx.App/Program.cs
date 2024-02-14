using Razmx.App;
using Razmx.App.Pages.Forecast;
using Tailwind;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.RunTailwind("tailwind:dev");
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();

app.MapHtmxRoutes()
    .MapRootComponent<ListForecasts>();

app.MapPost("/api/forecasts",
    (HttpContext context) => Results.HtmxLocation<ForecastDetails>(context, new { Id = Guid.NewGuid().ToString() }));

app.MapPut("/api/forecasts/{id}",
    (HttpContext context, string id) => Results.HtmxRedirect<ForecastDetails>(context, new { Id = id }));

app.Run();