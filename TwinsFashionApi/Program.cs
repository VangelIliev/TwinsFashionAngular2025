using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.IO;
using Serilog;
using TwinsFashion.Domain.Extensions;
using TwinsFashionApi.Models;
using TwinsFashionApi.Models.Mappings;
using TwinsFashionApi.Options;
using TwinsFashionApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day);
});

const string SpaCorsPolicy = "SpaCors";

builder.Services.AddCors(options =>
{
    options.AddPolicy(SpaCorsPolicy, policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "http://localhost:50473",
                "https://localhost:4200",
                "https://localhost:50473")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.Configure<CloudinaryOptions>(builder.Configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(provider =>
{
    var options = provider.GetRequiredService<IOptions<CloudinaryOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.CloudName) ||
        string.IsNullOrWhiteSpace(options.ApiKey) ||
        string.IsNullOrWhiteSpace(options.ApiSecret))
    {
        throw new InvalidOperationException("Cloudinary configuration is missing. Please set Cloudinary:CloudName, ApiKey, and ApiSecret.");
    }

    var account = new Account(options.CloudName, options.ApiKey, options.ApiSecret);
    return new Cloudinary(account);
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Admin/Login";
        options.LogoutPath = "/Admin/Logout";
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.Cookie.HttpOnly = false; // Allow JavaScript access for debugging
    });

builder.Services.AddTwinsFashionDomain(builder.Configuration);
builder.Services.AddTransient<IViewMapper, ViewMapper>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(SpaCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

var defaultFilesOptions = new DefaultFilesOptions();
defaultFilesOptions.DefaultFileNames.Clear();
defaultFilesOptions.DefaultFileNames.Add("app/twins-fashion-angular/browser/index.html");
app.UseDefaultFiles(defaultFilesOptions);
app.UseStaticFiles();

var spaStaticFilesPath = Path.Combine(app.Environment.WebRootPath ?? string.Empty, "app", "twins-fashion-angular", "browser");
if (Directory.Exists(spaStaticFilesPath))
{
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(spaStaticFilesPath),
        RequestPath = string.Empty
    });
}

app.MapControllers();

// Fallback to Angular app for non-API routes
app.MapFallbackToFile("app/twins-fashion-angular/browser/index.html");

app.Run();
