# Guia de Tratamento de Exceções - Jrg.SisMed

## ?? Visão Geral

Este documento descreve como o sistema de tratamento de exceções foi implementado na API Jrg.SisMed, seguindo as melhores práticas de Clean Architecture e ASP.NET Core.

## ??? Arquitetura

### Fluxo de Tratamento de Exceções

```
Controller ? UseCase ? Service ? Domain Entity
                                      ?
                                  Exception
                                      ?
              Middleware captura e trata
                                      ?
              ErrorResponse padronizada
```

## ?? Componentes Principais

### 1. Exceções Customizadas (Domain Layer)

#### `DomainValidationException`
- **Uso**: Erros de validação de regras de negócio
- **Status HTTP**: 400 Bad Request
- **Exemplo**: 
```csharp
var v = new ValidationCollector();
v.When(Name.IsNullOrWhiteSpace(), "Name is required");
v.ThrowIfAny(); // Lança DomainValidationException
```

#### `NotFoundException`
- **Uso**: Recurso não encontrado
- **Status HTTP**: 404 Not Found
- **Exemplo**:
```csharp
throw new NotFoundException("Organization", id);
// Resultado: "Organization with identifier '123' was not found."
```

#### `ConflictException`
- **Uso**: Recurso já existe (duplicidade)
- **Status HTTP**: 409 Conflict
- **Exemplo**:
```csharp
throw new ConflictException("Organization", "CNPJ", "12345678000195");
// Resultado: "Organization with CNPJ '12345678000195' already exists."
```

### 2. Modelo de Resposta de Erro (API Layer)

#### `ErrorResponse`
Modelo padronizado para todas as respostas de erro:

```json
{
  "statusCode": 400,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Name is required",
    "CNPJ is invalid"
  ],
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/organizacao"
}
```

**Propriedades**:
- `statusCode`: Código HTTP (400, 404, 409, 500, etc.)
- `message`: Mensagem principal
- `errors`: Lista de erros (opcional, para validações múltiplas)
- `details`: Detalhes técnicos (somente em Development)
- `timestamp`: Data/hora do erro
- `path`: Endpoint que gerou o erro

### 3. Middleware de Tratamento de Exceções

#### `ExceptionHandlingMiddleware`
Middleware global que captura todas as exceções não tratadas e retorna respostas padronizadas.

**Ordem de Registro** (no `Program.cs`):
```csharp
// DEVE ser o primeiro middleware
app.UseExceptionHandling();

// Outros middlewares...
app.UseSwagger();
app.UseHttpsRedirection();
app.UseAuthorization();
```

## ?? Mapeamento de Exceções

| Exceção | Status HTTP | Quando Usar |
|---------|-------------|-------------|
| `DomainValidationException` | 400 Bad Request | Validações de regras de negócio na entidade |
| `ArgumentException` | 400 Bad Request | Argumentos inválidos em métodos |
| `ArgumentNullException` | 400 Bad Request | Argumentos obrigatórios não fornecidos |
| `NotFoundException` | 404 Not Found | Recurso não encontrado |
| `ConflictException` | 409 Conflict | Recurso já existe (duplicidade) |
| `InvalidOperationException` | 409 Conflict | Operação inválida no estado atual |
| Outras exceções | 500 Internal Server Error | Erros não mapeados |

## ?? Exemplos de Uso

### Exemplo 1: Validação de Domínio

**Entidade (Domain)**:
```csharp
public class Organization : Entity
{
    private void Validate()
    {
        var v = new ValidationCollector();
        v.When(NameFantasia.IsNullOrWhiteSpace(), "Trade name is required");
        v.When(Cnpj.IsNullOrWhiteSpace(), "CNPJ is required");
        v.When(!Cnpj.IsCnpj(), "CNPJ is invalid");
        v.ThrowIfAny(); // Lança DomainValidationException
    }
}
```

**Resposta da API**:
```json
{
  "statusCode": 400,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Trade name is required",
    "CNPJ is invalid"
  ],
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/organizacao"
}
```

### Exemplo 2: Recurso Não Encontrado

**Service (Application)**:
```csharp
public async Task<Organization?> GetByIdAsync(int id, CancellationToken ct)
{
    var organization = await _repository.GetByIdAsync(id, ct);
    
    if (organization == null)
        throw new NotFoundException("Organization", id);
    
    return organization;
}
```

**Resposta da API**:
```json
{
  "statusCode": 404,
  "message": "Organization with identifier '123' was not found.",
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/organizacao/123"
}
```

### Exemplo 3: Conflito (Duplicidade)

**Service (Application)**:
```csharp
public async Task<int> ExecuteAsync(Organization organization, CancellationToken ct)
{
    if(await _repository.ExistsByCnpjAsync(organization.Cnpj, ct))
        throw new ConflictException("Organization", "CNPJ", organization.Cnpj);
    
    // Continua...
}
```

**Resposta da API**:
```json
{
  "statusCode": 409,
  "message": "Organization with CNPJ '12345678000195' already exists.",
  "timestamp": "2024-01-15T10:30:00Z",
  "path": "/api/organizacao"
}
```

## ?? Implementação em Novas Features

### Checklist para Novos Services

1. ? Use `NotFoundException` quando um recurso não for encontrado
2. ? Use `ConflictException` para duplicidades
3. ? Use `ArgumentException` para parâmetros inválidos
4. ? Deixe `DomainValidationException` ser lançada pelas entidades
5. ? **NÃO capture exceções** nos Services - deixe propagar
6. ? **NÃO capture exceções** nos UseCases - deixe propagar
7. ? **NÃO trate exceções** nos Controllers - o Middleware cuida disso

### Exemplo de Service Completo

```csharp
public class CreateOrganizationService : ICreateOrganizationService
{
    private readonly IOrganizationRepository _repository;

    public async Task<int> ExecuteAsync(Organization organization, CancellationToken ct)
    {
        // Verifica duplicidades - lança ConflictException
        if(await _repository.ExistsByCnpjAsync(organization.Cnpj, ct))
            throw new ConflictException("Organization", "CNPJ", organization.Cnpj);

        // Validações de domínio - Organization.Validate() pode lançar DomainValidationException
        await _repository.AddAsync(organization);
        await _repository.SaveChangesAsync(ct);

        return organization.Id;
    }
}
```

### Exemplo de Controller Simplificado

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateOrganizationDto dto, CancellationToken ct)
{
    // Sem try-catch - o middleware trata as exceções
    var id = await _createOrganizationUseCase.ExecuteAsync(dto, ct);
    return CreatedAtAction(nameof(Get), new { id }, new { id, message = "Created successfully" });
}
```

## ?? Ambiente de Desenvolvimento vs Produção

### Development
- Inclui detalhes técnicos da exceção (`Details` e `StackTrace`)
- JSON formatado (indentado)

### Production
- Mensagens genéricas para erros internos
- Sem detalhes técnicos sensíveis
- JSON compacto

## ?? Boas Práticas

### ? DO (Faça)
- Lance exceções específicas (`NotFoundException`, `ConflictException`)
- Deixe as exceções propagarem até o middleware
- Use `ValidationCollector` nas entidades
- Retorne respostas de sucesso simples nos controllers

### ? DON'T (Não Faça)
- **NÃO** use `try-catch` nos controllers
- **NÃO** use `try-catch` nos use cases
- **NÃO** capture exceções sem relançá-las
- **NÃO** retorne códigos de status manualmente no controller
- **NÃO** crie múltiplos modelos de erro

## ?? Testando o Tratamento de Exceções

### Teste 1: Validação de Domínio
```bash
POST /api/organizacao
{
  "nameFantasia": "",  // Inválido
  "razaoSocial": "Test",
  "cnpj": "invalid"    // Inválido
}
```

**Resposta Esperada**: 400 Bad Request com lista de erros

### Teste 2: Recurso Não Encontrado
```bash
GET /api/organizacao/99999
```

**Resposta Esperada**: 404 Not Found

### Teste 3: Duplicidade
```bash
POST /api/organizacao
{
  "nameFantasia": "Test",
  "razaoSocial": "Test Ltda",
  "cnpj": "12345678000195"  // Já existe
}
```

**Resposta Esperada**: 409 Conflict

## ?? Logs

O middleware registra automaticamente todas as exceções:

```
[Error] An unhandled exception occurred: Organization with CNPJ '12345678000195' already exists.
```

Use estes logs para monitorar erros em produção.

## ?? Referências

- [ASP.NET Core Exception Handling](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)
- [HTTP Status Codes](https://developer.mozilla.org/en-US/docs/Web/HTTP/Status)
- [REST API Error Handling Best Practices](https://www.baeldung.com/rest-api-error-handling-best-practices)

---

**Última Atualização**: 2024-01-15  
**Versão**: 1.0  
**Autor**: Jrg.SisMed Team
