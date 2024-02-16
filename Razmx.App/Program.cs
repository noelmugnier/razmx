using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razmx.App.Pages.Auth;
using Razmx.App.Pages.Forecast;
using Tailwind;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/forbidden";
        options.LogoutPath = "/logout";
    });

builder.Services.AddAuthorization(options =>
{
});

builder.Services.AddAntiforgery();

builder.Services.AddSqlite<AppDbContext>(builder.Configuration.GetConnectionString("AppDbContext"));

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
app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("")
    .MapHtmxRoutes(typeof(Main))
    .WithRootComponent<ListForecasts>();

app.MapPost("/api/forecasts",
    (HttpContext context) => HtmxResults.Location<ForecastDetails>(context, new { Id = Guid.NewGuid().ToString() }));

app.MapPut("/api/forecasts/{id}",
    (HttpContext context, string id) => HtmxResults.Redirect<ForecastDetails>(context, new { Id = id }));

app.MapPost("/register", async ([FromForm] RegisterRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext, CancellationToken token) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(a => a.UserName== request.Email, token);
    if (user != null)
    {
        return new RazorComponentResult<Register>(new { Form = request, Error = "Email already in use" });
    }

    user = new IdentityUser(request.Email)
    {
        Id = Guid.NewGuid().ToString(),
        Email = request.Email,
        PasswordHash = request.Password
    };

    await dbContext.AddAsync(user, token);
    await dbContext.SaveChangesAsync(token);

    await SignInUser(user, httpContext);
    return HtmxResults.Redirect<ListForecasts>(httpContext);
});

app.MapPost("/login", async ([FromForm] LoginRequest request, [FromServices] AppDbContext dbContext, HttpContext httpContext, CancellationToken token) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(a => a.UserName== request.Email && a.PasswordHash == request.Password, token);
    if (user == null)
    {
        return new RazorComponentResult<Login>(new { Form = request, Error = "Invalid username or password" });
    }

    await SignInUser(user, httpContext);
    return HtmxResults.Redirect<ListForecasts>(httpContext);
});

app.MapPost("/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return HtmxResults.Redirect<Login>(httpContext);
});

await app.RunAsync();

async Task SignInUser(IdentityUser identityUser, HttpContext httpContext1)
{
    var claims = new List<Claim>
    {
        new (ClaimTypes.Name, identityUser.UserName),
        new (ClaimTypes.NameIdentifier, identityUser.Id),
        new (ClaimTypes.Email, identityUser.Email)
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await httpContext1.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
}

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<IdentityUser> Users { get; set; }
}