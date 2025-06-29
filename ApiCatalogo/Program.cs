using ApiCatalogo.Context;
using ApiCatalogo.DTOs.AutomaticMapper;
using ApiCatalogo.Extensions;
using ApiCatalogo.Filter;
using ApiCatalogo.Logging;
using ApiCatalogo.Models;
using ApiCatalogo.Repositories;
using ApiCatalogo.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
}).AddNewtonsoftJson();

// Implementa��o do CORS (Padr�p)
// var OrigensComAcessoPermitido = "_origensComAcessoPermitido";

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
                    policy =>
                    {
                        policy.WithOrigins("http://www.apirequest.io")
                        .WithMethods("GET", "POST")
                        .AllowAnyHeader();
                    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PoliticaCors1",
                    policy =>
                    {
                        policy.WithOrigins("http://www.apirequest.io")
                        .WithMethods("GET");
                    });

    options.AddPolicy("PoliticaCors2",
                    policy =>
                    {
                        policy.WithOrigins("http://www.gabrielaraujo.net")
                        .WithMethods("GET", "DELETE");
                    });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "APIcatalogo", 
        Description = "Cat�logo de produtos e categorias.",
        TermsOfService = new Uri("https://apideaprendizado.net/terms"),
        Contact = new OpenApiContact
        {
            Name = "Gabriel",
            Email= "aprendizado@gmail.com",
            Url = new Uri("https://apideaprendizado.net/terms")
        },
        License = new OpenApiLicense
        {
            Name = "Usar sobre LICX",
            Url = new Uri("https://apideaprendizado.net/terms")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer JWT"
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
            new string[] { }
        }
    });
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Configuration.AddUserSecrets<Program>();

var mySqlConnection = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(mySqlConnection))
{
    throw new Exception("String de conex�o n�o encontrada. Verifique os User Secrets.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(mySqlConnection, ServerVersion.AutoDetect(mySqlConnection))
);

var secretKey = builder.Configuration["JWT:Secretkey"] ?? throw new ArgumentException("Invalid Secret Key!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey (Encoding.UTF8.GetBytes (secretKey))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("SuperAdminOnly", policy => policy.RequireRole("Admin").RequireClaim("id", "teste"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("ExclusiveOnly", policy => policy.RequireAssertion(context =>
                                                    context.User.HasClaim(claim => claim.Type == "id" &&
                                                                                   claim.Value == "teste") ||
                                                                                   context.User.IsInRole("SuperAdmin")));
});

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(policyName: "fixedWindow", options =>
    {
        options.PermitLimit = 1;
        options.Window = TimeSpan.FromSeconds(5);
        options.QueueLimit = 0;
    });
});

builder.Services.AddScoped<ApiLoggingFilter>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenService, TokenService>();

/*
    AddScope n�o permite que seja registrado tipos gen�ricos. 
    Para resolver isso, ser� feito a "adapta��o" abaixo.
*/
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddAutoMapper(typeof(ProductDtoMappingProfile));

builder.Logging.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
{
    LogLevel = LogLevel.Information
}));

builder.Services.AddMemoryCache();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ConfigureExceptionHandler();
}


app.UseHttpsRedirection();

// Implementa��o do CORS
app.UseCors();

// Implementa��o o RateLimiter
app.UseRateLimiter();

app.UseAuthorization();

app.MapControllers();

app.Run();
