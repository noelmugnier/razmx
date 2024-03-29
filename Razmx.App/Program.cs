using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razmx.App.Pages.Auth;
using Razmx.App.Pages.Contacts;
using Razmx.App.Pages.Home;
using Tailwind;
using NotFound = Razmx.App.Pages.NotFound;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/forbidden";
        options.LogoutPath = "/logout";
    });

builder.Services.AddAuthorization(options => { });

builder.Services.AddAntiforgery();
builder.Services.AddRazorComponents();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("AppDbContext"), options => { },
    build =>
    {
        build.EnableSensitiveDataLogging();
        build.EnableDetailedErrors();
    });

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
app.UseAuthentication();
app.UseAuthorization();

app
    .MapHtmxPages(typeof(MainLayout))
    .MapNotFoundPage<NotFound>();

app.MapGet("/api/forecasts",
    () => new RazorComponentResult<Forecasts>(new Dictionary<string, object?>()
        { { "Model", ForecastsService.GetNextForecasts() } }));

app.MapPost("/api/register",
    async ([FromForm] RegisterRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext,
        CancellationToken token, [FromQuery] string? returnUrl = null) =>
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(a => a.UserName == request.Email, token);
        if (user != null)
        {
            return new HtmxFormComponentResult<Register, RegisterRequest>(request);
        }

        user = new IdentityUser(request.Email)
        {
            Id = Guid.NewGuid().ToString(),
            Email = request.Email,
            PasswordHash = request.Password
        };

        await dbContext.AddAsync(user, token);
        await dbContext.SaveChangesAsync(token);
        await SignInUser(httpContext, user);

        return !string.IsNullOrWhiteSpace(returnUrl)
            ? httpContext.NavigateToUrl(returnUrl)
            : httpContext.RedirectTo<Home>();
    });

app.MapPost("/api/login",
    async ([FromForm] LoginRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext,
        CancellationToken token, [FromQuery] string? returnUrl = null) =>
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            a => a.UserName == request.Email && a.PasswordHash == request.Password, token);
        if (user == null)
        {
            var errors = new Dictionary<string, string>
            {
                { "_", "Invalid username or password" },
                { nameof(request.Email), "Invalid username" },
                { nameof(request.Password), "Invalid password" }
            };

            return new HtmxFormComponentResult<Login, LoginRequest>(request, errors);
        }

        await SignInUser(httpContext, user);

        return !string.IsNullOrWhiteSpace(returnUrl)
            ? httpContext.NavigateToUrl(returnUrl)
            : httpContext.RedirectTo<Home>();
    });

app.MapPost("/api/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return httpContext.RedirectTo<Home>();
});

app.MapGet("/api/forecasts/json",
    () => new JsonResult(ForecastsService.GetNextForecasts()));

app.MapPost("/api/contacts", async (HttpContext httpContext, [FromForm] Contact contact, [FromServices] AppDbContext dbContext, CancellationToken token) =>
{
    await dbContext.AddAsync(contact, token);
    await dbContext.SaveChangesAsync(token);
    return httpContext.NavigateTo<List>();
});

await app.RunAsync();
return;

async Task SignInUser(HttpContext httpContext, IdentityUser identityUser)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, identityUser.UserName),
        new(ClaimTypes.NameIdentifier, identityUser.Id),
        new(ClaimTypes.Email, identityUser.Email)
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
}

public static class ForecastsService
{
    private static string[] summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static IEnumerable<WeatherForecast> GetNextForecasts() => Enumerable.Range(1, 10).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)],
                Guid.NewGuid().ToString()
            ))
        .ToArray();
}