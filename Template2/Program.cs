using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Commands;
using Infrastructure.Persistance;
using Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(policy =>
{
    policy.AddDefaultPolicy(options => options.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Expreso de las diez - Microservicio de Usuarios", Version = "v1" });

    //Boton Autorize (Swagger)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Jwt Authorization",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
            new string[] {}
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

//Custom
var connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<ExpresoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<IUserCommands, UserCommands>();
builder.Services.AddTransient<IUserServices, UserServices>();
builder.Services.AddTransient<IUserQueries, UserQueries>();

builder.Services.AddTransient<IGenderServices, GenderServices>();
builder.Services.AddTransient<IGenderQueries, GenderQueries>();

builder.Services.AddTransient<IEncryptServices, EncryptServices>();
builder.Services.AddTransient<IValidateServices, ValidateServices>();
builder.Services.AddTransient<IImageCommands, ImageCommands>();
builder.Services.AddTransient<IImageQueries, ImageQueries>();
builder.Services.AddTransient<IImageServices, ImageServices>();
builder.Services.AddTransient<IValidateImageServices, ValidateImageServices>();
builder.Services.AddTransient<IServerImagesApiServices, ServerImagesApiServices>();
builder.Services.AddTransient<IAuthApiServices, AuthApiServices>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
