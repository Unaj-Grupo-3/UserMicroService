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
using Presentation.Authorization;

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

builder.Services.AddAuthentication(ApiKeySchemeOptions.Scheme)
    .AddScheme<ApiKeySchemeOptions, ApiKeySchemeHandler>(
        ApiKeySchemeOptions.Scheme, options =>
        {
            options.HeaderName = "X-API-KEY";
        });

//Custom
var connectionString = builder.Configuration["ConnectionString"];

builder.Services.AddDbContext<ExpresoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<IUserCommands, UserCommands>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IUserQueries, UserQueries>();

builder.Services.AddScoped<IGenderServices, GenderServices>();
builder.Services.AddScoped<IGenderQueries, GenderQueries>();

builder.Services.AddScoped<IValidateUserServices, ValidateUserServices>();
builder.Services.AddScoped<IImageCommands, ImageCommands>();
builder.Services.AddScoped<IImageQueries, ImageQueries>();
builder.Services.AddScoped<IImageServices, ImageServices>();
builder.Services.AddScoped<IValidateImageServices, ValidateImageServices>();
builder.Services.AddHttpClient<IServerImagesApiServices, ServerImagesApiServices>()
       .Services.AddScoped<IServerImagesApiServices, ServerImagesApiServices>();


builder.Services.AddScoped<IValidateLocationServices, ValidateLocationServices>();
builder.Services.AddScoped<ILocationCommands, LocationCommands>();
builder.Services.AddScoped<ILocationQueries, LocationQueries>();
builder.Services.AddScoped<ILocationServices, LocationServices>();


builder.Services.AddHttpClient<ILocationApiServices, LocationApiServices>()
       .Services.AddScoped<ILocationApiServices, LocationApiServices>();

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
