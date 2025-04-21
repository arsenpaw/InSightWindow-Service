using Application.Configuration;
using Domain.Entity.Enums;
using Domain.EntityFramework;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Infrastructure.Data;
using InSightWindowAPI;
using InSightWindowAPI.Extensions;
using InSightWindowAPI.Hubs;
using InSightWindowAPI.JwtSetting;
using InSightWindowAPI.Models.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
using System.Text.Json;

var myCors = "AllOriginsWithoutCredentials";
var builder = WebApplication.CreateBuilder(args);
var jwtSettings = new JwtSettings();

builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton(jwtSettings);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerDocs();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddServices();
builder.Services.AddDataProcessors();
builder.Services.AddRepository();
builder.Services.AddDbContext<UsersContext>((options) =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorNumbersToAdd: null
            );
        });
    });
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
     //.WriteTo.AzureApp(LogEventLevel.Information)
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
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(myCors, builder =>
    {
        builder.SetIsOriginAllowed(origin => true) // Allow all origins
             .AllowAnyMethod()
             .AllowAnyHeader()
             .AllowCredentials();
    });

});
//builder.Configuration.AddAzureCredentialsFromAppsettingToEnv();

//if (builder.Environment.IsProduction())
//{
//    var vaultConfig = new KeyVault();
//    builder.Configuration.GetSection("KeyVault").Bind(vaultConfig);
//    builder.WebHost.AddVaultCertificate(vaultConfig);
//}
var firebaseSettings = new FirebaseConfig();
builder.Configuration.GetSection("Firebase").Bind(firebaseSettings);
try
{
    var rawConfig = JsonSerializer.Serialize(firebaseSettings);
    FirebaseApp.Create(new AppOptions()
    {
        Credential = GoogleCredential.FromJson(rawConfig)
    });
}
catch (Exception ex)
{
    Log.Error(ex, "FirebaseApp.Create failed");
}




var app = builder.Build();
app.MigrateDatabase<UsersContext>();

// Configure the HTTP request pipeline
app.UseCors(myCors);
app.UseHttpsRedirection();

//if (app.Environment.IsDevelopment())
if (true) //Tempoorary measure
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseExceptionMiddleware();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ClientStatusHub>("/client-hub");
app.Run();

