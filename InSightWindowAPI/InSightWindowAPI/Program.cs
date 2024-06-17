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


string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = new JwtSettings();
builder.Configuration.GetSection("JwtSettings").Bind(jwtSettings);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });
builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
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




// Add services to the container.
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddDbContext<UsersContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<UsersDbController>();
builder.Services.AddTransient<DevicesDbController>();   
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddAuthorization();


builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
builder => {
    builder.WithOrigins("https://localhost:44324/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://localhost:81/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://localhost:81/api/UsersDb/login").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://localhost:81/api/UsersDb/create").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://localhost:81/api/UsersDb").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(MyAllowSpecificOrigins);

app.UseHttpsRedirection();
app.MapControllers();
app.MapHub<ClientStatusHub>("/client-hub");
app.MapHub<UserInputHub>("/user-input-hub");

app.UseAuthorization();
app.UseAuthentication();    
app.MapControllers();

app.Run();
