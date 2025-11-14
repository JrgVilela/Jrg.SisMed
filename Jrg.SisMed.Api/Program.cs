using Jrg.SisMed.Infra.IoC;

var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte à localização, apontando para a pasta dentro do Domain
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

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

app.UseAuthorization();

app.MapControllers();

app.Run();
