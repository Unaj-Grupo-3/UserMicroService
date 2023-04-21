using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Commands;
using Infrastructure.Persistance;
using Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expreso de las diez - Microservicio de usuarios", Version = "v1" });
});

//Custom
var connectionString = "";
Console.WriteLine(Directory.GetCurrentDirectory());
var gab = "C:\\Users\\Gabo\\Documents\\Backup\\unaj\\ProyectoDeSoftware_1\\2023-Primer-cuatri\\Grupal\\AppDeCitas\\UserMicroService2\\Template2\\Template2";

if(Directory.GetCurrentDirectory() == gab)
{
    connectionString =
        builder.Configuration["ConnectionString2"];
}
else 
{
    // MSSQL running locally
    connectionString = builder.Configuration["ConnectionString"];
}


Console.WriteLine(connectionString);
builder.Services.AddDbContext<ExpresoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUserCommands, UserCommands>();
builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<IAuthCommands, AuthCommands>();
builder.Services.AddTransient<IAuthQueries, AuthQueries>();
builder.Services.AddTransient<IAuthServices, AuthServices>();
builder.Services.AddTransient<IEncryptServices, EncryptServices>();
builder.Services.AddTransient<IValidateServices, ValidateServices>();
builder.Services.AddTransient<IImageCommands, ImageCommands>();
builder.Services.AddTransient<IImageQueries, ImageQueries>();
builder.Services.AddTransient<IImageServices, ImageServices>();
builder.Services.AddTransient<IValidateImageServices, ValidateImageServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
