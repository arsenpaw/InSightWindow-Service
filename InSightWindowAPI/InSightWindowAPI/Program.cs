using InSightWindowAPI.Controllers;
using InSightWindowAPI.Hubs;
using InSightWindowAPI.Models;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InSightWindowAPI.JwtSetting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Serilog;
using Serilog.Events;
using FluentValidation.AspNetCore;
using InSightWindowAPI.Filters;
using AutoMapper;
using InSightWindowAPI.Serivces;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Microsoft.VisualBasic;
using FirebaseAdmin.Messaging;
string myCorses = "AllowAllOrigins";

var builder = WebApplication.CreateBuilder(args);

// Configure JWT settings
var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddSingleton(jwtSettings);

// Configure JSON options for controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
      
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddTransient<IPushNotificationService, PushNotificationService>();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<UsersDbController>();
builder.Services.AddTransient<DevicesDbController>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Host.UseSerilog((context, config) =>
{
    var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LOGS.txt");
    config.WriteTo.Console()
        .WriteTo.Debug()
        .WriteTo.File(logFilePath, LogEventLevel.Warning)
        .MinimumLevel.Information();
    
});
// Configure authentication
builder.Services.AddAuthentication(options =>
{
    
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    
})
.AddJwtBearer(options =>
{
  
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myCorses, builder =>
    {
        builder.WithOrigins("http://localhost:3000", "http://192.168.0.188:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});

builder.Services.AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssembly(typeof(Program).Assembly);
 });


var pathToKeyFile = $"{Directory.GetCurrentDirectory()}\\{builder.Configuration["Firebase:KeyFilePath"]}";

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(pathToKeyFile),
});



var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(myCorses);

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ClientStatusHub>("/client-hub");
app.Run();

