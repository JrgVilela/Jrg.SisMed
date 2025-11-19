# ? Implementação Completa - User CRUD (UseCases + Controller)

## ?? Resumo da Implementação

Implementação completa do CRUD de usuários seguindo o padrão arquitetural do projeto (Clean Architecture + UseCases + DTOs).

---

## ?? O Que Foi Implementado

### 1. ? DTOs (Application Layer)

**Localização:** `Jrg.SisMed.Application/DTOs/UserDto/`

#### CreateUserDto.cs
```csharp
public class CreateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserEnum.State State { get; set; } = UserEnum.State.Active;

    public User ToDomainUser() { ... }
}
```

#### UpdateUserDto.cs
```csharp
public class UpdateUserDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public UserEnum.State State { get; set; }

    public User ToDomainUser() { ... }
}
```

#### ReadUserDto.cs
```csharp
public class ReadUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public UserEnum.State State { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static ReadUserDto FromDomainUser(User user) { ... }
}
```

---

### 2. ? UseCases (Application Layer)

**Localização:** `Jrg.SisMed.Application/UseCases/User/`

#### CreateUserUseCase.cs
```csharp
public class CreateUserUseCase
{
    private readonly ICreateUserService _createUserService;
    private readonly IStringLocalizer<Messages> _localizer;

    public async Task<int> ExecuteAsync(CreateUserDto userDto, CancellationToken cancellationToken = default)
    {
        // Valida DTO
        // Converte para entidade de domínio
        // Chama service
        // Retorna ID do usuário criado
    }
}
```

**Responsabilidades:**
- ? Validação do DTO
- ? Conversão de DTO para entidade de domínio
- ? Orquestração do service
- ? Tratamento de erros

#### UpdateUserUseCase.cs
```csharp
public class UpdateUserUseCase
{
    private readonly IUpdateUserService _updateUserService;
    private readonly IStringLocalizer<Messages> _localizer;

    public async Task ExecuteAsync(int id, UpdateUserDto userDto, CancellationToken cancellationToken = default)
    {
        // Valida ID e DTO
        // Converte para entidade
        // Chama service
    }
}
```

#### ReadUserUseCase.cs
```csharp
public class ReadUserUseCase
{
    private readonly IReadUserService _readUserService;

    // Métodos disponíveis:
    public async Task<bool> ExistsByIdAsync(int id, ...)
    public async Task<IEnumerable<ReadUserDto>> GetAllAsync(...)
    public async Task<ReadUserDto?> GetByEmailAsync(string email, ...)
    public async Task<ReadUserDto?> GetByIdAsync(int id, ...)
}
```

**Responsabilidades:**
- ? Consulta ao service
- ? Conversão de entidade para DTO
- ? Retorna lista vazia se não houver dados

#### DeleteUserUseCase.cs
```csharp
public class DeleteUserUseCase
{
    private readonly IDeleteUserService _deleteUserService;
    private readonly IStringLocalizer<Messages> _localizer;

    public async Task ExecuteAsync(int id)
    {
        // Valida ID
        // Chama service de exclusão
    }
}
```

---

### 3. ? Dependency Injection Configurado

**Arquivo Modificado:** `Jrg.SisMed.Infra.IoC/DependencyInjection.cs`

```csharp
// User Services
services.AddScoped<ICreateUserService, CreateUserService>();
services.AddScoped<IUpdateUserService, UpdateUserService>();
services.AddScoped<IReadUserService, ReadUserService>();
services.AddScoped<IDeleteUserService, DeleteUserService>();

// User UseCases
services.AddScoped<CreateUserUseCase>();
services.AddScoped<UpdateUserUseCase>();
services.AddScoped<ReadUserUseCase>();
services.AddScoped<DeleteUserUseCase>();

// User Repository
services.AddScoped<IUserRepository, UserRepository>();

// FluentValidation (registra todos os validators automaticamente)
services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();
```

**Ciclo de Vida:**
- ? `Scoped` - Uma instância por request HTTP

---

### 4. ? UserController (API Layer)

**Arquivo Criado:** `Jrg.SisMed.Api/Controllers/UserController.cs`

#### Endpoints Implementados:

| Método | Endpoint | Descrição | Response |
|--------|----------|-----------|----------|
| **GET** | `/api/user` | Lista todos os usuários | 200, 404 |
| **GET** | `/api/user/{id}` | Busca usuário por ID | 200, 404 |
| **GET** | `/api/user/search?email={email}` | Busca usuário por email | 200, 400, 404 |
| **POST** | `/api/user` | Cria novo usuário | 201, 400 |
| **PUT** | `/api/user/{id}` | Atualiza usuário | 200, 400, 404 |
| **DELETE** | `/api/user/{id}` | Exclui usuário | 204, 404 |

#### Exemplo de Implementação:

```csharp
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly CreateUserUseCase _createUserUseCase;
    private readonly UpdateUserUseCase _updateUserUseCase;
    private readonly ReadUserUseCase _readUserUseCase;
    private readonly DeleteUserUseCase _deleteUserUseCase;

    // GET /api/user
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _readUserUseCase.GetAllAsync(cancellationToken);
        if (!result.Any())
            return NotFound("No users found.");
        return Ok(result);
    }

    // GET /api/user/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, ...)

    // GET /api/user/search?email=user@email.com
    [HttpGet("search")]
    public async Task<IActionResult> SearchByEmail([FromQuery] string email, ...)

    // POST /api/user
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto, ...)

    // PUT /api/user/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto, ...)

    // DELETE /api/user/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
}
```

**Características:**
- ? Documentação XML nos métodos
- ? Response codes documentados (Swagger)
- ? Validação de parâmetros
- ? Tratamento de erros
- ? CancellationToken support
- ? ProducesResponseType para Swagger

---

## ?? Estrutura Completa Implementada

```
Jrg.SisMed.Api/
  ??? Controllers/
      ??? UserController.cs                    ? Novo

Jrg.SisMed.Application/
  ??? DTOs/
  ?   ??? UserDto/
  ?       ??? CreateUserDto.cs                 ? Novo
  ?       ??? UpdateUserDto.cs                 ? Novo
  ?       ??? ReadUserDto.cs                   ? Novo
  ??? UseCases/
  ?   ??? User/
  ?       ??? CreateUserUseCase.cs             ? Novo
  ?       ??? UpdateUserUseCase.cs             ? Novo
  ?       ??? ReadUserUseCase.cs               ? Novo
  ?       ??? DeleteUserUseCase.cs             ? Novo
  ??? Services/
  ?   ??? UserServices/
  ?       ??? CreateUserService.cs             ? Já existia
  ?       ??? UpdateUserService.cs             ? Implementado anteriormente
  ?       ??? ReadUserService.cs               ? Implementado anteriormente
  ?       ??? DeleteUserService.cs             ? Implementado anteriormente
  ??? Validations/
      ??? UserValidations/
          ??? CreateUserValidation.cs          ? Já existia
          ??? UpdateUserValidation.cs          ? Implementado anteriormente

Jrg.SisMed.Domain/
  ??? Entities/
  ?   ??? User.cs                              ? Já existia
  ??? Interfaces/
      ??? Repositories/
      ?   ??? IUserRepository.cs               ? Já existia
      ??? Services/
          ??? UserServices/
              ??? ICreateUserService.cs        ? Já existia
              ??? IUpdateUserService.cs        ? Já existia
              ??? IReadUserService.cs          ? Corrigido anteriormente
              ??? IDeleteUserService.cs        ? Já existia

Jrg.SisMed.Infra.Data/
  ??? Repositories/
      ??? UserRepository.cs                    ? Já existia

Jrg.SisMed.Infra.IoC/
  ??? DependencyInjection.cs                   ? Atualizado
```

---

## ?? Fluxo de Execução (Exemplo: Create User)

```
???????????????????????????????????????????????????????????????
?  1. HTTP Request                                            ?
?     POST /api/user                                          ?
?     Body: CreateUserDto                                     ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  2. UserController                                          ?
?     - Recebe CreateUserDto                                  ?
?     - Valida request                                        ?
?     - Chama CreateUserUseCase                              ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  3. CreateUserUseCase (Application Layer)                   ?
?     - Valida DTO                                            ?
?     - Converte DTO para User (domínio)                      ?
?     - Chama CreateUserService                               ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  4. CreateUserService (Application Layer)                   ?
?     - Valida regras de negócio com FluentValidation        ?
?     - Verifica email duplicado                              ?
?     - Ativa usuário                                         ?
?     - Chama UserRepository                                  ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  5. UserRepository (Infrastructure Layer)                   ?
?     - AddAsync(user)                                        ?
?     - SaveChangesAsync()                                    ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  6. Database (SQL Server)                                   ?
?     - INSERT INTO Users...                                  ?
?     - Retorna ID gerado                                     ?
???????????????????????????????????????????????????????????????
                  ?
???????????????????????????????????????????????????????????????
?  7. HTTP Response                                           ?
?     201 Created                                             ?
?     Location: /api/user/{id}                                ?
?     Body: { id: 123 }                                       ?
???????????????????????????????????????????????????????????????
```

---

## ?? Testando os Endpoints

### 1. Criar Usuário
```http
POST /api/user
Content-Type: application/json

{
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "password": "Senha@123",
  "state": 1
}
```

**Response:**
```json
201 Created
Location: /api/user/1

1
```

### 2. Buscar Todos os Usuários
```http
GET /api/user
```

**Response:**
```json
200 OK

[
  {
    "id": 1,
    "name": "João Silva",
    "email": "joao.silva@email.com",
    "state": 1,
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": null
  }
]
```

### 3. Buscar Usuário por ID
```http
GET /api/user/1
```

**Response:**
```json
200 OK

{
  "id": 1,
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "state": 1,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### 4. Buscar Usuário por Email
```http
GET /api/user/search?email=joao.silva@email.com
```

**Response:**
```json
200 OK

{
  "id": 1,
  "name": "João Silva",
  "email": "joao.silva@email.com",
  "state": 1,
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### 5. Atualizar Usuário
```http
PUT /api/user/1
Content-Type: application/json

{
  "name": "João Silva Santos",
  "email": "joao.silva@email.com",
  "password": "NovaSenha@456",
  "state": 1
}
```

**Response:**
```json
200 OK

{
  "message": "User updated successfully"
}
```

### 6. Excluir Usuário
```http
DELETE /api/user/1
```

**Response:**
```
204 No Content
```

---

## ?? Validações Implementadas

### CreateUserDto / UpdateUserDto

| Campo | Validações |
|-------|-----------|
| **Name** | ? Obrigatório<br>? Máximo 100 caracteres |
| **Email** | ? Obrigatório<br>? Formato válido<br>? Máximo 100 caracteres<br>? Único no banco |
| **Password** | ? Obrigatório<br>? Mínimo 8 caracteres<br>? Máximo 25 caracteres<br>? Senha forte (letras, números, especiais) |
| **State** | ? Valor enum válido (Active, Inactive, Blocked) |

---

## ? Funcionalidades Implementadas

| Funcionalidade | Status | Descrição |
|----------------|--------|-----------|
| **Create** | ? | Criação de usuário com validações |
| **Read (All)** | ? | Lista todos os usuários |
| **Read (By ID)** | ? | Busca por ID |
| **Read (By Email)** | ? | Busca por email |
| **Update** | ? | Atualização com validações |
| **Delete** | ? | Exclusão lógica |
| **Validation** | ? | FluentValidation integrado |
| **DTOs** | ? | Separação de concerns |
| **UseCases** | ? | Orquestração de lógica |
| **Swagger** | ? | Documentação automática |
| **Error Handling** | ? | Tratamento de erros |

---

## ?? Padrões Seguidos

1. ? **Clean Architecture** - Separação de camadas
2. ? **CQRS Pattern** - Separação de comandos e consultas
3. ? **UseCase Pattern** - Casos de uso explícitos
4. ? **DTO Pattern** - Transferência de dados
5. ? **Repository Pattern** - Abstração de dados
6. ? **Dependency Injection** - Inversão de controle
7. ? **FluentValidation** - Validações declarativas
8. ? **RESTful API** - Padrões REST
9. ? **Async/Await** - Operações assíncronas
10. ? **Swagger/OpenAPI** - Documentação

---

## ?? Próximos Passos Recomendados

### 1. Implementar Autenticação/Autorização
```csharp
[HttpGet]
[Authorize] // ? Adicionar autorização
public async Task<IActionResult> GetAll(...)
```

### 2. Implementar Paginação
```csharp
public async Task<IActionResult> GetAll(
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 10)
{
    var result = await _readUserUseCase.GetAllPaginatedAsync(page, pageSize);
    return Ok(result);
}
```

### 3. Implementar Filtros
```csharp
[HttpGet("search")]
public async Task<IActionResult> Search(
    [FromQuery] string? name,
    [FromQuery] string? email,
    [FromQuery] UserEnum.State? state)
```

### 4. Adicionar Logging
```csharp
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    [HttpPost]
    public async Task<IActionResult> Create(...)
    {
        _logger.LogInformation("Creating user with email: {Email}", dto.Email);
        // ...
    }
}
```

### 5. Implementar Testes
```csharp
public class CreateUserUseCaseTests
{
    [Fact]
    public async Task Should_Create_User_Successfully() { }
    
    [Fact]
    public async Task Should_Throw_When_Email_Already_Exists() { }
}
```

---

## ? Checklist Final

| Item | Status |
|------|--------|
| **DTOs** | ? Criados (Create, Update, Read) |
| **UseCases** | ? Implementados (CRUD completo) |
| **Services** | ? Já existiam |
| **Validations** | ? FluentValidation configurado |
| **DI** | ? Todos os componentes registrados |
| **Controller** | ? API REST completa |
| **Repository** | ? Já existia |
| **Build** | ? Compilando sem erros |
| **Swagger** | ? Documentação gerada |
| **Padrão** | ? Seguindo Organization |

---

## ?? Documentação Swagger Gerada

Ao acessar `/swagger`, você verá:

```
?????????????????????????????????????????????????
?              User API                         ?
?????????????????????????????????????????????????
?  GET    /api/user                            ?
?  GET    /api/user/{id}                       ?
?  GET    /api/user/search?email={email}       ?
?  POST   /api/user                            ?
?  PUT    /api/user/{id}                       ?
?  DELETE /api/user/{id}                       ?
?????????????????????????????????????????????????
?  Schemas                                      ?
?  - CreateUserDto                              ?
?  - UpdateUserDto                              ?
?  - ReadUserDto                                ?
?  - UserEnum.State                             ?
?????????????????????????????????????????????????
```

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Status:** ? Implementação Completa
**Build:** ? Sucesso
**Padrão:** Clean Architecture + UseCases + CQRS
**API:** ? REST completa com 6 endpoints
