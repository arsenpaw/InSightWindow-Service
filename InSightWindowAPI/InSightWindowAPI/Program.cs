using InSightWindowAPI.Controllers;
using InSightWindowAPI.Hubs;
using InSightWindowAPI.Models;
using Microsoft.EntityFrameworkCore;
using InSightWindowAPI;
using Microsoft.Extensions.Options;


string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

// Add services to the container.

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

app.MapControllers();

app.Run();
