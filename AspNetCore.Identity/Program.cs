using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using AspNetCore.Identity.Data;
using AspNetCore.Identity.Data.Identity;
using AspNetCore.Identity.Data.Protection;
using AspNetCore.Identity.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization()
    .AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services
    .AddIdentityCore<ApplicationUser>(o =>
    {
        o.Password.RequireDigit = true;
        o.Password.RequireLowercase = true;
        o.Password.RequiredLength = 8;
        o.Password.RequireUppercase = true;
        o.Password.RequireNonAlphanumeric = true;
        o.User.RequireUniqueEmail = true;
        o.SignIn.RequireConfirmedAccount = false;
        o.SignIn.RequireConfirmedEmail = false;
        o.Stores.ProtectPersonalData = true;
        o.Stores.MaxLengthForKeys = 128;
    })
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddPersonalDataProtection<ApplicationLookupProtector, ApplicationILookupProtectorKeyRing>()
    .AddDefaultTokenProviders()
    .AddApiEndpoints();

builder.Services.AddDbContextPool<ApplicationDbContext>(options => options.UseNpgsql(
    builder.Configuration.GetConnectionString("Database")));

builder.Services
    .AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>()
    .ProtectKeysWithCertificate(new X509Certificate2("certificate.pfx", builder.Configuration["CertificatePassword"])) // https://stackoverflow.com/a/16481636
    .SetDefaultKeyLifetime(TimeSpan.FromDays(14))
    .SetApplicationName(builder.Environment.ContentRootPath.TrimEnd(Path.DirectorySeparatorChar))
    .UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration
    {
        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations();
}

app.MapGet("users/me", async (ClaimsPrincipal claims, ApplicationDbContext context) =>
{
    string userId = claims.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
    return await context.Users.FindAsync(new Guid(userId));
})
.RequireAuthorization();

app.MapIdentityApi<ApplicationUser>();

app.Run();
