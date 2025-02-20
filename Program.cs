using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TuketAppAPI.Models;

var builder = WebApplication.CreateBuilder(args);

//  Veritabanı Bağlantısını Yapılandır
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TuketDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

//  JWT Authentication Ayarları
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

//  Secret Key Kontrolü ve Dönüşümü (Base64 yerine UTF-8 kullanıldı)
var secretKeyString = jwtSettings["Secret"];
if (string.IsNullOrEmpty(secretKeyString))
{
    throw new Exception(" Error: Secret Key is missing from configuration!");
}

//  **Base64 yerine UTF-8 olarak dönüştür**
var secretKeyBytes = Convert.FromBase64String(secretKeyString);
var secretKey = new SymmetricSecurityKey(secretKeyBytes);

Console.WriteLine($" Loaded Secret Key: {secretKeyString}");

//  Authentication & Authorization Middleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = secretKey,  //  UTF-8 ile encode edilen Secret Key Kullanıldı
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // **Token süreleri kesin olsun**
    };
    
});

//  API Servislerini Ekleyelim
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "TuketAppAPI", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Token'ınızı 'Bearer {token}' formatında girin."
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
            new string[] {}
        }
    });
});

//  Uygulamayı Başlat
var app = builder.Build();
Console.WriteLine($" Application is running in {app.Environment.EnvironmentName} mode.");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();