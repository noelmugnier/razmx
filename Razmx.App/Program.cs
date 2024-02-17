using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razmx.App.Components;
using Razmx.App.Models;
using Razmx.App.Pages;
using Razmx.App.Pages.Auth;
using Razmx.App.Pages.Forecast;
using Tailwind;

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

app.MapGroup("")
    .MapHtmxPages(typeof(MainLayout))
    .MapNotFoundPage<Fallback>();

app.MapGet("/api/forecasts",
    () => new RazorComponentResult<Forecasts>(new Dictionary<string, object?>()
        { { "Value", ForecastsService.GetNextForecasts() } }));

app.MapPost("/api/forecasts",
    (HttpContext context) =>
        HtmxResults.RedirectToCreated<ForecastDetails>(context, new { Id = Guid.NewGuid().ToString() }));

app.MapPut("/api/forecasts/{id}",
    (HttpContext context, string id) => HtmxResults.FullPageRedirect<ForecastDetails>(context, new { Id = id }));

app.MapPost("/register",
    async ([FromForm] RegisterRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext,
        CancellationToken token, [FromQuery] string? returnUrl = null) =>
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(a => a.UserName == request.Email, token);
        if (user != null)
        {
            var state = ModelState.Init(request);
            state.Errors.Add("Email", "Email already in use");
            state.Errors.Add("Password", "Invalid password");

            return new RazorComponentResult<Register>(new Dictionary<string, object?>(){{"State", state}});
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

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            return HtmxResults.RedirectToUrl(httpContext, returnUrl);
        }

        return HtmxResults.Redirect<Home>(httpContext);
    });

app.MapPost("/login",
    async ([FromForm] LoginRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext,
        CancellationToken token, [FromQuery] string? returnUrl = null) =>
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(
            a => a.UserName == request.Email && a.PasswordHash == request.Password, token);
        if (user == null)
        {
            var state = ModelState.Init(request);
            state.Errors.Add("_", "Invalid username or password");
            state.Errors.Add("Email", "Invalid username");
            state.Errors.Add("Password", "Invalid password");

            return new RazorComponentResult<Login>(new Dictionary<string, object?>(){{"State", state}});
        }

        await SignInUser(httpContext, user);

        if (!string.IsNullOrWhiteSpace(returnUrl))
        {
            return HtmxResults.RedirectToUrl(httpContext, returnUrl);
        }

        return HtmxResults.Redirect<Home>(httpContext);
    });

app.MapPost("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return HtmxResults.FullPageRedirect<Home>(httpContext);
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

    public static IEnumerable<WeatherForecast> GetNextForecasts() => Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)],
                Guid.NewGuid().ToString()
            ))
        .ToArray();
}