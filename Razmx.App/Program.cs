using Razmx.App;
using Razmx.App.Pages.Shared;
using Tailwind;
using Index = Razmx.App.Pages.Index;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    _ = app.RunTailwind("tailwind:dev", "./");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();
app.UseAuthorization();

// app.MapHtmxRoutes();

app.MapRazorComponents<Index>();
app.Run();