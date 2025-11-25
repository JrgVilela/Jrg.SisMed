using Jrg.SisMed.Api.Middleware;
using Jrg.SisMed.Infra.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte à localização, apontando para a pasta dentro do Domain
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    // Política mais restritiva para produção (recomendado)
    options.AddPolicy("Production", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
        //policy.WithOrigins(
        //        "https://seudominio.com.br",
        //        "https://www.seudominio.com.br",
        //        "https://app.seudominio.com.br"
        //    )
        //    .AllowAnyMethod()
        //    .AllowAnyHeader()
        //    .AllowCredentials();
    });
});

// Configuração JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey não configurada.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Apenas para desenvolvimento
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove atraso padrão de 5 minutos
    };
});

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();

// Configuração do Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Jrg.SisMed API",
        Version = "v1",
        Description = "API para Sistema Médico - Gerenciamento de Profissionais, Organizações e Agendamentos",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Jrg Vilela",
            Url = new Uri("https://github.com/JrgVilela/Jrg.SisMed")
        }
    });

    // Configuração de autenticação JWT no Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Configura o Swagger para usar nomes únicos para evitar conflitos
    options.CustomSchemaIds(type => 
    {
        // Se for um tipo genérico, use o nome genérico
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().Name.Replace("`1", "");
            var genericArgs = string.Join(",", type.GetGenericArguments().Select(t => t.Name));
            return $"{genericTypeName}Of{genericArgs}";
        }

        // Se for um tipo aninhado (como enums dentro de classes), use o nome completo
        if (type.DeclaringType != null)
        {
            return $"{type.DeclaringType.Name}{type.Name}";
        }

        // Caso contrário, use apenas o nome do tipo
        return type.Name;
    });

    // Habilita comentários XML (opcional)
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // if (File.Exists(xmlPath))
    // {
    //     options.IncludeXmlComments(xmlPath);
    // }
});

var app = builder.Build();

// Configure the HTTP request pipeline.

// Middleware de tratamento de exceções global (DEVE ser o primeiro middleware)
app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    // Habilita Swagger UI no ambiente de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jrg.SisMed API v1");
        options.RoutePrefix = "swagger"; // Swagger acessível em: https://localhost:{porta}/swagger
        options.DocumentTitle = "Jrg.SisMed API Documentation";
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
    });
}

// Só usa HTTPS redirect se estiver configurado HTTPS
if (app.Urls.Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}

// CORS - Deve vir ANTES de Authentication e Authorization
// Em desenvolvimento usa a política "AllowAll"
// Em produção, altere para "Production"
app.UseCors(app.Environment.IsDevelopment() ? "AllowAll" : "Production");

// IMPORTANTE: A ordem é crucial!
app.UseAuthentication();  // Deve vir ANTES do UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();
