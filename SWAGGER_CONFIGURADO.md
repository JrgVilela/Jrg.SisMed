# ?? Swagger Habilitado - Jrg.SisMed API

## ? Swagger Configurado com Sucesso!

O Swagger/OpenAPI foi habilitado na sua API para fornecer documentação interativa e testes de endpoints.

---

## ?? Como Acessar o Swagger

### 1. Execute a API
```bash
dotnet run --project Jrg.SisMed.Api
```

### 2. Acesse no Navegador

**URL do Swagger UI:**
```
https://localhost:{porta}/swagger
```

Por exemplo:
- `https://localhost:5001/swagger`
- `https://localhost:7001/swagger`

> ?? A porta específica será exibida no console quando você executar a API.

---

## ?? O Que Foi Configurado

### 1. Pacote Instalado
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="7.2.0" />
```

### 2. Configuração no Program.cs

#### Services Adicionados:
```csharp
// Adiciona suporte para exploração de endpoints
builder.Services.AddEndpointsApiExplorer();

// Configura o Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Jrg.SisMed API",
        Version = "v1",
        Description = "API para Sistema Médico",
        Contact = new OpenApiContact
        {
            Name = "Jrg Vilela",
            Url = new Uri("https://github.com/JrgVilela/Jrg.SisMed")
        }
    });
});
```

#### Middleware Configurado:
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Jrg.SisMed API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Jrg.SisMed API Documentation";
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
    });
}
```

---

## ?? Funcionalidades do Swagger UI

### 1. Documentação Automática
- ? Lista todos os endpoints da API
- ? Mostra métodos HTTP (GET, POST, PUT, DELETE, etc.)
- ? Exibe modelos de dados (DTOs, Entities)
- ? Documenta parâmetros e respostas

### 2. Teste Interativo
- ? **"Try it out"** - Teste endpoints diretamente no navegador
- ? Preencha parâmetros e body
- ? Execute requisições
- ? Veja respostas em tempo real

### 3. Informações Detalhadas
- ? Schemas de request/response
- ? Códigos de status HTTP
- ? Headers necessários
- ? Tempo de duração das requisições

---

## ??? Melhorias Opcionais

### 1. Habilitar Comentários XML

Para mostrar descrições dos métodos e parâmetros no Swagger, adicione ao `.csproj`:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

Depois, descomente no `Program.cs`:
```csharp
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
if (File.Exists(xmlPath))
{
    options.IncludeXmlComments(xmlPath);
}
```

### 2. Adicionar Autenticação JWT no Swagger

Se você usar JWT, adicione:
```csharp
builder.Services.AddSwaggerGen(options =>
{
    // Configuração existente...
    
    // Adiciona suporte a JWT
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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
```

### 3. Versionamento de API

Para múltiplas versões:
```csharp
options.SwaggerDoc("v1", new OpenApiInfo { Title = "API v1", Version = "v1" });
options.SwaggerDoc("v2", new OpenApiInfo { Title = "API v2", Version = "v2" });
```

### 4. Habilitar em Produção (Opcional)

Por padrão, Swagger só está habilitado em **Development**. Para habilitar em produção (não recomendado para APIs públicas):

```csharp
// Remove a validação de ambiente
// if (app.Environment.IsDevelopment())  // ? Remove isso
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }
```

---

## ?? Preview do Swagger UI

Quando acessar `/swagger`, você verá:

```
??????????????????????????????????????????????????
?         Jrg.SisMed API v1                     ?
?  API para Sistema Médico - Gerenciamento...   ?
??????????????????????????????????????????????????
?                                                ?
?  Endpoints:                                    ?
?                                                ?
?  GET  /api/professionals                       ?
?  POST /api/professionals/psychologist          ?
?  POST /api/professionals/nutritionist          ?
?  GET  /api/organizations                       ?
?  ...                                           ?
?                                                ?
?  Schemas:                                      ?
?  - CreatePsychologistDto                       ?
?  - CreateNutritionistDto                       ?
?  - Person                                      ?
?  ...                                           ?
??????????????????????????????????????????????????
```

---

## ?? Como Usar para Testar

### Exemplo: Testar Criação de Psicólogo

1. **Acesse:** `/swagger`
2. **Encontre o endpoint:** `POST /api/professionals/psychologist`
3. **Clique em:** "Try it out"
4. **Preencha o JSON:**
```json
{
  "name": "Dr. João Silva",
  "cpf": "12345678901",
  "rg": "123456789",
  "birthDate": "1980-01-01",
  "gender": 1,
  "email": "joao.silva@clinica.com",
  "password": "Senha@123",
  "crp": "CRP-06/123456"
}
```
5. **Clique em:** "Execute"
6. **Veja a resposta:**
   - Status: 200 OK
   - Body: Dados do profissional criado
   - Headers: Content-Type, etc.

---

## ?? Troubleshooting

### Problema: Swagger não abre
**Solução:**
1. Verifique se está em modo Development
2. Confirme a URL correta (com `/swagger`)
3. Verifique o console para ver a porta correta

### Problema: Endpoints não aparecem
**Solução:**
1. Certifique-se de ter Controllers com `[ApiController]` e `[Route]`
2. Rebuild do projeto: `dotnet build`
3. Limpe e reconstrua: `dotnet clean && dotnet build`

### Problema: Erros de validação
**Solução:**
- Swagger mostra os schemas esperados
- Compare seu JSON com o schema mostrado
- Verifique tipos de dados (string, int, DateTime, etc.)

---

## ?? Recursos Adicionais

- **Documentação Oficial Swashbuckle:** https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- **OpenAPI Specification:** https://swagger.io/specification/
- **Microsoft Docs:** https://learn.microsoft.com/aspnet/core/tutorials/web-api-help-pages-using-swagger

---

## ? Checklist de Configuração

- [x] Pacote `Swashbuckle.AspNetCore` instalado
- [x] `AddEndpointsApiExplorer()` adicionado
- [x] `AddSwaggerGen()` configurado
- [x] `UseSwagger()` e `UseSwaggerUI()` no pipeline
- [x] Build sem erros
- [ ] Testar acesso ao `/swagger` (após executar a API)
- [ ] (Opcional) Habilitar comentários XML
- [ ] (Opcional) Adicionar autenticação JWT

---

**Status:** ? Swagger Configurado e Pronto para Uso!

**Próximo Passo:** Execute a API e acesse `https://localhost:{porta}/swagger`

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Configurado por:** GitHub Copilot
