using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using InSightWindowAPI;
using InSightWindowAPI.EntityFramework;
using InSightWindowAPI.Enums;
using InSightWindowAPI.Extensions;
using InSightWindowAPI.Hubs;
using InSightWindowAPI.JwtSetting;
using InSightWindowAPI.Models;
using InSightWindowAPI.Models.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Events;
using System.Text;
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

//if (app.Environment.IsDevelopment())
if (true) //Tempoorary measure
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsProduction())
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

