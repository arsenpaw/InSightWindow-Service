using InSightWindowAPI.Controllers;
using Microsoft.Extensions.Options;

string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
builder => {
    builder.WithOrigins("https://localhost:44324/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://localhost:81/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://192.168.4.2:81/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
    builder.WithOrigins("http://192.168.4.1:81/client-hub").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
