using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Commands;
using Infrastructure.Persistance;
using Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
var connectionString = builder.Configuration["ConnectionString"];
builder.Services.AddDbContext<ExpresoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUserCommands, UserCommands>();
builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<IAuthCommands, AuthCommands>();
builder.Services.AddTransient<IAuthQueries, AuthQueries>();
builder.Services.AddTransient<IAuthServices, AuthServices>();
builder.Services.AddTransient<IEncryptServices, EncryptServices>();

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
