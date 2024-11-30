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
using System.Configuration;
using Microsoft.Extensions.Configuration;
using InSightWindowAPI.Middlewares;
using Microsoft.AspNetCore.Identity;
using System;

using Azure.Core.Pipeline;
using System.Xml.Linq;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Google;
using InSightWindowAPI.Services;
using InSightWindowAPI.Enums;
using InSightWindowAPI.Extensions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using InSightWindowAPI.Hubs.ConnectionMapper;
using InSightWindowAPI.EntityFramework;
var myCors = "AllOriginsWithoutCredentials";
var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddEnvironmentVariables();


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

Console.WriteLine(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocs();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddTransient<IPushNotificationService, PushNotificationService>();
builder.Services.AddSingleton<IAesService>(provider =>
        new AesService("1234567890ABCDEF", "1234567890ABCDEF"));
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<UsersDbController>();
builder.Services.AddTransient<DevicesDbController>();
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddHttpsRedirection(opt =>
{
    opt.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
    opt.HttpsPort = 443;
});

builder.Host.UseSerilog((context, config) =>
{
    var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
    config.WriteTo.Console(LogEventLevel.Information)
     .WriteTo.Debug(LogEventLevel.Information)
     .WriteTo.AzureApp(LogEventLevel.Information)
     .WriteTo.File(logFilePath, LogEventLevel.Warning)
     .MinimumLevel.Information();


});

builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<UsersContext>() 
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.RequireUniqueEmail = true;

});

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
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole(UserRoles.ADMIN));
    options.AddPolicy("RequireUserRole", policy => policy.RequireRole(UserRoles.USER));
    // Add more policies as needed
});
// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(myCors, builder =>
    {
        builder.SetIsOriginAllowed(origin => true) // Allow all origins
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials(); // Allow credentials if the origin is allowed
    });

});




var pathToKeyFile = $"{Directory.GetCurrentDirectory()}//{builder.Configuration["Firebase:KeyFilePath"]}";

FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(pathToKeyFile),
});


var app = builder.Build();
app.MigrateDatabase<UsersContext>();
// Configure the HTTP request pipeline
app.UseCors(myCors);
//app.UseAllowCredentialsToSite();    

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection(); //azure not work with it
}

app.UseExceptionMiddleware();
app.UseRouting();   
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ClientStatusHub>("/client-hub");
app.Run();

