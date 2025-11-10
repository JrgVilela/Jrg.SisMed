using Jrg.SisMed.Domain.Resources;


var builder = WebApplication.CreateBuilder(args);

// Adiciona suporte à localização, apontando para a pasta dentro do Domain
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
