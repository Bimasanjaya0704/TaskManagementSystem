using Infrastructure.Presistence;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TaskManagementSystem.Application;
using TaskManagementSystem.Web.Api.Mappings;
using TaskManagementSystem.Web.Api.Models;
using TaskManagementSystem.Web.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var logRepository = LogManager.GetRepository(typeof(Program).Assembly);
var configPath = Path.Combine(Directory.GetCurrentDirectory(), "config", "log4net.config");
if (File.Exists(configPath))
{
    XmlConfigurator.Configure(logRepository, new FileInfo(configPath));
}
else
{
    Console.WriteLine("⚠️ log4net.config not found!");
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskManagementSystem",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

});

builder.Services.AddApplicationAndInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<TokenService>();
builder.Services.AddAutoMapper(typeof(MappingsPresentation));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/*app.UseHttpsRedirection();*/
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
var secret = builder.Configuration["JwtSettings:Secret"];
Console.WriteLine($"Loaded JWT Secret: {secret ?? "NULL"}");

app.Run();

