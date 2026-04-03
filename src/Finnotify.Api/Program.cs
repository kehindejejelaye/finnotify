using System.Security.Claims;
using System.Text.Json;
using Finnotify.Application.ConfigurationOptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
    
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtOptions = builder.Configuration
            .GetSection(KeycloakJwtOptions.SECTION)
            .Get<KeycloakJwtOptions>();

        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new() {
            RoleClaimType = "roles",
            ValidateIssuer = true,
            ValidIssuers = jwtOptions?.ValidIssuers,
            ValidateAudience = true,
            ValidAudiences = jwtOptions?.ValidAudiences,
            ValidateLifetime = true,
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var identity = context.Principal?.Identity as ClaimsIdentity;

                if (identity == null)
                    return Task.CompletedTask;

                var realmAccess = context?.Principal?.FindFirst("realm_access")?.Value;

                if (!string.IsNullOrEmpty(realmAccess))
                {
                    var roles = JsonDocument.Parse(realmAccess)
                        .RootElement
                        .GetProperty("roles");

                    foreach (var role in roles.EnumerateArray())
                    {
                        identity.AddClaim(new Claim("roles", role.GetString()!));
                    }
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlatformSuperAdmin", policy =>
        policy.RequireRole("platform_super_admin"));

    options.AddPolicy("PlatformAdmin", policy =>
        policy.RequireRole("platform_admin"));

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
