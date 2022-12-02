using FarmshareAdmin.Data;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using mdl = FarmshareAdmin.Models;
using data = FarmshareAdmin.Data;
using utl = FarmshareAdmin.Utilities;

var builder = WebApplication.CreateBuilder(args);

/* Get the path for the domain of the web app on the server.  Server.MapPath replacement:
    string path = Path.GetDirectoryName(builder.Environment.WebRootPath);
 */
/*
 * Here's how to read configuration variables in program.cs:
 */
//var Environment = builder.Configuration.GetValue<string>("Environment");

// Add Active Directory authentication service to the container.
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
   .AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    // By default, all incoming requests will be authorized according to the default policy.
    options.FallbackPolicy = options.DefaultPolicy;
});


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddScoped<utl.Logging, utl.Logging>();
builder.Services.AddScoped<data.UnitOfWork, data.UnitOfWork>();
builder.Services.AddScoped<data.IFarmAllocationService, data.FarmAllocationService>();
builder.Services.AddScoped<data.SendMail, data.SendMail>();

builder.Services.AddDbContext<mdl.ACF_FarmshareContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Conn"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
