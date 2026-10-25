using Application.Constants;
using Application.ImportMappings;
using Application.Interfaces;
using Application.Models;
using Application.UseCases.Authentication;
using Application.UseCases.Countries;
using Application.UseCases.Medications;
using Application.UseCases.Medications.Dto;
using Application.UseCases.Medications.Service;
using Application.UseCases.Pharmacies;
using Application.UseCases.Pharmacies.Dto;
using digital_pharmacy_backend.Services;
using digital_pharmacy_backend.Services.Firebase;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Data.Repository;
using Infrastructure.Excel.Import;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()?
    .Where(origin => Uri.TryCreate(origin, UriKind.Absolute, out _))
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray()
    ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value is { Errors.Count: > 0 })
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? AppMessages.InvalidInput
                            : error.ErrorMessage)
                        .ToArray());

            var message = context.ModelState.Keys.Any(key =>
                key.EndsWith("SearchType", StringComparison.OrdinalIgnoreCase))
                ? AppMessages.InvalidSearchType
                : AppMessages.InvalidInput;

            return new BadRequestObjectResult(ApiResponse<object?>.Failure(
                StatusCodes.Status400BadRequest,
                message));
        };
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions => npgsqlOptions.MapEnum<UserRole>("user_role", "public")));

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<FirebaseOptions>(builder.Configuration.GetSection(FirebaseOptions.SectionName));
builder.Services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IGenericRepository, GenericRepository<AppDbContext>>();
builder.Services.AddScoped<IMedicationDetailsRepository, MedicationDetailsRepository>();
builder.Services.AddScoped<IMedicationDetailsValidation, MedicationDetailsValidation>();
builder.Services.AddScoped<IMedicationDetailsService, MedicationDetailsService>();
builder.Services.AddScoped<IExcelImportProvider<MedicationDetailExcelImportMap, MedicationDetailsExcelDto>, ExcelImportProvider<MedicationDetailExcelImportMap, MedicationDetailsExcelDto>>();
builder.Services.AddScoped<IExcelImportProvider<PharmaciesExcelImportMap, PharmaciesExcelDto>, ExcelImportProvider<PharmaciesExcelImportMap, PharmaciesExcelDto>>();
builder.Services.AddScoped<IPharmaciesRepository, PharmaciesRepository>();
builder.Services.AddScoped<IPharmaciesService, PharmaciesService>();
builder.Services.AddScoped<ICountryService, CountryService>();

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            RequireExpirationTime = false
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Failure(
                    StatusCodes.Status401Unauthorized,
                    AppMessages.AuthenticationRequired));
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponse<object?>.Failure(
                    StatusCodes.Status401Unauthorized,
                    AppMessages.UnauthorizedAccess));
            }
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<digital_pharmacy_backend.Middleware.ExceptionHandlingMiddleware>();
app.UseRouting();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
