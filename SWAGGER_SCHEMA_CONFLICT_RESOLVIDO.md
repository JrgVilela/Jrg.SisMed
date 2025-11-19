# ? Problema de Schema Duplicado no Swagger - Resolvido

## ?? Problema Identificado

### Erro Original:
```
System.InvalidOperationException: Can't use schemaId "$State" for type 
"$Jrg.SisMed.Domain.Entities.UserEnum+State". 
The same schemaId is already used for type 
"$Jrg.SisMed.Domain.Entities.OrganizationEnum+State"
```

### Causa Raiz:
O Swagger estava tentando gerar schemas para dois enums diferentes com o **mesmo nome** `State`:
- `UserEnum.State` (Active, Inactive, Blocked)
- `OrganizationEnum.State` (Active, Inactive, Suspended)

Por padrão, o Swagger usa apenas o nome do tipo (`State`) como identificador do schema, causando **conflito de nomes**.

---

## ?? Solução Implementada

### Configuração Adicionada no `Program.cs`:

```csharp
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { ... });

    // ? Configura o Swagger para usar nomes únicos
    options.CustomSchemaIds(type => 
    {
        // Se for um tipo genérico
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().Name.Replace("`1", "");
            var genericArgs = string.Join(",", type.GetGenericArguments().Select(t => t.Name));
            return $"{genericTypeName}Of{genericArgs}";
        }

        // Se for um tipo aninhado (enums dentro de classes)
        if (type.DeclaringType != null)
        {
            return $"{type.DeclaringType.Name}{type.Name}";
        }

        // Caso contrário, use apenas o nome do tipo
        return type.Name;
    });
});
```

---

## ?? Como Funciona

### Antes (Conflito) ?:
```
UserEnum.State       ? Schema ID: "State"  ?
OrganizationEnum.State ? Schema ID: "State"  ? Conflito!
```

### Depois (Único) ?:
```
UserEnum.State       ? Schema ID: "UserEnumState"  ?
OrganizationEnum.State ? Schema ID: "OrganizationEnumState"  ?
```

A função `CustomSchemaIds` detecta que são **tipos aninhados** (nested types) e concatena:
- Nome da classe declarante (`UserEnum` ou `OrganizationEnum`)
- Nome do tipo (`State`)

---

## ?? Resultados

### No Swagger UI:

Agora você verá schemas distintos:

```json
{
  "UserEnumState": {
    "enum": [1, 2, 3],
    "type": "integer",
    "description": "Active = 1, Inactive = 2, Blocked = 3",
    "format": "int32"
  },
  "OrganizationEnumState": {
    "enum": [1, 2, 3],
    "type": "integer",
    "description": "Active = 1, Inactive = 2, Suspended = 3",
    "format": "int32"
  }
}
```

### Nas DTOs:

**CreateUserDto:**
```json
{
  "name": "string",
  "email": "string",
  "password": "string",
  "state": "UserEnumState"  ? Agora está claro qual enum usar
}
```

**CreateOrganizationDto:**
```json
{
  "nameFantasia": "string",
  "razaoSocial": "string",
  "cnpj": "string",
  "state": "OrganizationEnumState"  ? Diferente do User
}
```

---

## ?? Testando a Correção

### 1. Execute a aplicação:
```bash
dotnet run --project Jrg.SisMed.Api
```

### 2. Acesse o Swagger:
```
http://localhost:5063/swagger
```

### 3. Verifique os Schemas:
No Swagger UI, procure pela seção **"Schemas"** no final da página:
- ? `UserEnumState`
- ? `OrganizationEnumState`
- ? `CreateUserDto`
- ? `UpdateUserDto`
- ? `ReadUserDto`
- ? Todos os outros schemas

### 4. Teste um Endpoint:
Tente criar um usuário usando o Swagger:

```json
POST /api/user
{
  "name": "João Silva",
  "email": "joao@email.com",
  "password": "Senha@123",
  "state": 1  // UserEnumState.Active
}
```

---

## ?? Comparação: Antes vs Depois

### ? Antes (Erro):
```
Swagger Exception:
  Can't use schemaId "State" for type "UserEnum+State".
  The same schemaId is already used for type "OrganizationEnum+State"

Resultado: Swagger não carrega ?
```

### ? Depois (Funcionando):
```
UserEnum.State ? Schema ID: "UserEnumState"
OrganizationEnum.State ? Schema ID: "OrganizationEnumState"

Resultado: Swagger carrega perfeitamente ?
```

---

## ?? Outros Casos Tratados pela Solução

### 1. Tipos Genéricos
```csharp
List<User> ? Schema ID: "ListOfUser"
IEnumerable<Organization> ? Schema ID: "IEnumerableOfOrganization"
```

### 2. Tipos Aninhados (Nested Types)
```csharp
UserEnum.State ? "UserEnumState"
OrganizationEnum.State ? "OrganizationEnumState"
PersonEnum.Gender ? "PersonEnumGender"
```

### 3. Tipos Simples
```csharp
User ? "User"
Organization ? "Organization"
CreateUserDto ? "CreateUserDto"
```

---

## ?? Por Que Esse Erro Aconteceu?

### Estrutura das Entidades:

**User.cs:**
```csharp
public class User : Entity
{
    public UserEnum.State State { get; private set; }
}

public class UserEnum
{
    public enum State  // ? Nome: State
    {
        Active = 1,
        Inactive = 2,
        Blocked = 3
    }
}
```

**Organization.cs:**
```csharp
public class Organization : Entity
{
    public OrganizationEnum.State State { get; private set; }
}

public class OrganizationEnum
{
    public enum State  // ? Nome: State (mesmo nome!)
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }
}
```

Ambos os enums têm o nome `State`, mas em classes diferentes. Sem o `CustomSchemaIds`, o Swagger usa apenas `State` como identificador, causando conflito.

---

## ?? Lições Aprendidas

### 1. Swagger e Tipos Aninhados
Quando você tem tipos aninhados (nested types) com o mesmo nome, precisa configurar o `CustomSchemaIds` para evitar conflitos.

### 2. Alternativas ao Problema

**Opção A: CustomSchemaIds (Recomendado)** ?
```csharp
options.CustomSchemaIds(type => type.DeclaringType != null 
    ? $"{type.DeclaringType.Name}{type.Name}" 
    : type.Name);
```

**Opção B: Renomear Enums** (Menos flexível)
```csharp
public enum UserState { ... }  // Ao invés de UserEnum.State
public enum OrganizationState { ... }  // Ao invés de OrganizationEnum.State
```

**Opção C: Namespaces Completos** (Verboso)
```csharp
options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
// Resultado: Jrg.SisMed.Domain.Entities.UserEnum.State
```

### 3. Nossa Escolha
Optamos pela **Opção A** porque:
- ? Mantém os enums organizados como nested types
- ? Nomes curtos e legíveis no Swagger
- ? Não requer refatoração do código existente
- ? Escalável para futuros enums

---

## ? Checklist de Verificação

Após a correção, verifique:

- [x] Build compila sem erros
- [x] Swagger carrega sem exceções
- [x] Swagger UI acessível em `/swagger`
- [x] Todos os endpoints aparecem
- [x] Schemas `UserEnumState` e `OrganizationEnumState` existem
- [x] DTOs exibem os enums corretamente
- [x] É possível testar endpoints pelo Swagger

---

## ?? Próximos Passos

Agora que o Swagger está funcionando:

### 1. Teste os Endpoints de User
```bash
# Listar todos
GET /api/user

# Buscar por ID
GET /api/user/1

# Buscar por email
GET /api/user/search?email=user@email.com

# Criar usuário
POST /api/user
{
  "name": "Teste",
  "email": "teste@email.com",
  "password": "Senha@123",
  "state": 1
}

# Atualizar usuário
PUT /api/user/1
{
  "name": "Teste Atualizado",
  "email": "teste@email.com",
  "password": "Senha@456",
  "state": 1
}

# Excluir usuário
DELETE /api/user/1
```

### 2. Documente com XML Comments (Opcional)
Para melhorar ainda mais o Swagger, adicione comentários XML:

**UserController.cs:**
```csharp
/// <summary>
/// Cria um novo usuário no sistema.
/// </summary>
/// <param name="createUserDto">Dados do usuário</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>ID do usuário criado</returns>
/// <remarks>
/// Exemplo de requisição:
/// 
///     POST /api/user
///     {
///        "name": "João Silva",
///        "email": "joao@email.com",
///        "password": "Senha@123",
///        "state": 1
///     }
/// 
/// </remarks>
/// <response code="201">Usuário criado com sucesso</response>
/// <response code="400">Dados inválidos ou email duplicado</response>
[HttpPost]
public async Task<IActionResult> Create(...)
```

### 3. Habilite XML Documentation
No `Jrg.SisMed.Api.csproj`:
```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

No `Program.cs`:
```csharp
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
if (File.Exists(xmlPath))
{
    options.IncludeXmlComments(xmlPath);
}
```

---

## ?? Referências

- [Swashbuckle CustomSchemaIds](https://github.com/domaindrivendev/Swashbuckle.AspNetCore#change-the-unique-id)
- [OpenAPI Schema Naming](https://swagger.io/docs/specification/components/)
- [.NET Nested Types](https://learn.microsoft.com/dotnet/csharp/programming-guide/classes-and-structs/nested-types)

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Status:** ? Problema Resolvido
**Build:** ? Sucesso
**Swagger:** ? Funcionando Corretamente
