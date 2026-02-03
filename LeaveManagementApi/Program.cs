using System.Text;
using LeaveManagementApi.Authorization;
using LeaveManagementApi.Data;
using LeaveManagementApi.Repositories;
using LeaveManagementApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<LeaveManagementDbContext>(options =>
    options.UseInMemoryDatabase("LeaveManagementDb"));

builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("Auth"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IUserStore, UserStore>();

builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IApprovalRepository, ApprovalRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ILeaveService, LeaveService>();

var authSettings = builder.Configuration.GetSection("Auth").Get<AuthSettings>();
var key = Encoding.UTF8.GetBytes(authSettings?.SigningKey ?? "dev-signing-key");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = authSettings?.Issuer,
            ValidAudience = authSettings?.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
