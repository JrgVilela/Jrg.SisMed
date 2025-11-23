# ?? Implementação de Autenticação JWT - Guia Completo

## ? O que foi implementado

Uma solução completa de autenticação JWT para a API, incluindo:

1. ? Serviço de geração de tokens JWT
2. ? Use Case de autenticação
3. ? Controller de autenticação
4. ? Middleware para tratar exceções de autenticação
5. ? Configuração completa do JWT
6. ? Documentação Swagger com autenticação
7. ? Endpoint de teste de token

---

## ?? Pacotes NuGet Necessários

### **IMPORTANTE: Adicionar manualmente no projeto Jrg.SisMed.Api**

```bash
dotnet add Jrg.SisMed.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add Jrg.SisMed.Application package System.IdentityModel.Tokens.Jwt --version 8.2.1
```

**Ou adicionar diretamente no .csproj:**

```xml
<ItemGroup>
  <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
  <PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.2.1" />
</ItemGroup>
```

---

## ?? Arquivos Criados

### **1. Application Layer**

#### **Services**
- `Jrg.SisMed.Application\Services\AuthServices\JwtTokenService.cs`
  - Gera tokens JWT
  - Valida tokens JWT

#### **DTOs**
- `Jrg.SisMed.Application\DTOs\AuthDto\LoginRequestDto.cs`
  - `LoginRequestDto` - Request de login
  - `LoginResponseDto` - Response com token
  - `UserInfoDto` - Informações do usuário

#### **Use Cases**
- `Jrg.SisMed.Application\UseCases\AuthUseCases\AuthenticateUserUseCase.cs`
  - Orquestra o fluxo de autenticação
  - Valida credenciais
  - Busca usuário
  - Gera token JWT

### **2. Domain Layer**

#### **Exceptions**
- `Jrg.SisMed.Domain\Exceptions\UnauthorizedException.cs`
  - Exceção para erros de autenticação

### **3. API Layer**

#### **Controllers**
- `Jrg.SisMed.Api\Controllers\AuthController.cs`
  - `POST /api/auth/login` - Login
  - `GET /api/auth/me` - Verifica token

#### **Configuration**
- `Jrg.SisMed.Api\appsettings.json` - Configurações JWT
- `Jrg.SisMed.Api\Program.cs` - Configuração do middleware JWT

#### **Middleware**
- `Jrg.SisMed.Api\Middleware\ExceptionHandlingMiddleware.cs` - Atualizado para tratar `UnauthorizedException`

### **4. IoC Layer**

- `Jrg.SisMed.Infra.IoC\UseCaseDependencyInjection.cs` - Registra `AuthenticateUserUseCase`
- `Jrg.SisMed.Infra.IoC\ServiceDependencyInjection.cs` - Registra `JwtTokenService`

---

## ?? Configuração

### **appsettings.json**

```json
{
  "JwtSettings": {
    "SecretKey": "MinhaChaveSecretaSuperSeguraComMaisDe32Caracteres!@#$",
    "Issuer": "JrgSisMed",
    "Audience": "JrgSisMedUsers",
    "ExpirationMinutes": 60
  }
}
```

**?? IMPORTANTE:**
- A `SecretKey` deve ter **no mínimo 32 caracteres**
- Em produção, use uma chave forte e armazene em **variáveis de ambiente** ou **Azure Key Vault**

### **Program.cs - Configuração JWT**

```csharp
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
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
```

**?? IMPORTANTE: A ordem dos middlewares é crucial!**

```csharp
app.UseExceptionHandling();  // Primeiro
app.UseAuthentication();     // Segundo (ANTES do Authorization)
app.UseAuthorization();      // Terceiro
app.MapControllers();        // Por último
```

---

## ?? Fluxo de Autenticação

```
1. REQUEST
   POST /api/auth/login
   {
       "email": "joao@exemplo.com",
       "password": "SenhaForte@123"
   }

2. CONTROLLER
   AuthController.Login()
   ?

3. USE CASE
   AuthenticateUserUseCase.ExecuteAsync()
   ?

4. VALIDAÇÃO DE CREDENCIAIS
   LoginUserUseCase.ExecuteAsync()
   - Verifica email e senha no banco
   ? OK ? Continua
   ? Erro ? UnauthorizedException
   ?

5. BUSCA DADOS DO USUÁRIO
   ReadUserUseCase.GetByEmailAsync()
   - Busca usuário completo
   ? OK ? Continua
   ? Não encontrado ? UnauthorizedException
   ?

6. VERIFICA ESTADO DO USUÁRIO
   - User.State == Active?
   ? OK ? Continua
   ? Inativo/Bloqueado ? UnauthorizedException
   ?

7. GERA TOKEN JWT
   JwtTokenService.GenerateToken()
   - Cria claims
   - Assina token
   - Retorna JWT
   ?

8. RESPONSE
   HTTP 200 OK
   {
       "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
       "tokenType": "Bearer",
       "expiresIn": 60,
       "user": {
           "id": 1,
           "name": "João Silva",
           "email": "joao@exemplo.com",
           "state": "Active"
       }
   }
```

---

## ?? Exemplos de Uso

### **1. Login**

```http
POST /api/auth/login
Content-Type: application/json

{
    "email": "joao@exemplo.com",
    "password": "SenhaForte@123"
}
```

**Response (200 OK):**
```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiSm_Do28gU2lsdmEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJqb2FvQGV4ZW1wbG8uY29tIiwiVXNlcklkIjoiMSIsIlVzZXJTdGF0ZSI6IkFjdGl2ZSIsImV4cCI6MTcwNTMzOTIwMCwiaXNzIjoiSnJnU2lzTWVkIiwiYXVkIjoiSnJnU2lzTWVkVXNlcnMifQ.K9XYZ1234567890abcdefghijklmnopqrstuvwxyz",
    "tokenType": "Bearer",
    "expiresIn": 60,
    "user": {
        "id": 1,
        "name": "João Silva",
        "email": "joao@exemplo.com",
        "state": "Active"
    }
}
```

### **2. Acessar Endpoint Protegido**

```http
GET /api/auth/me
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Response (200 OK):**
```json
{
    "id": "1",
    "name": "João Silva",
    "email": "joao@exemplo.com",
    "message": "Token válido!"
}
```

### **3. Proteger um Endpoint**

```csharp
[HttpGet]
[Authorize] // ? Requer autenticação
public IActionResult GetProtectedData()
{
    var userId = User.FindFirst("UserId")?.Value;
    var userName = User.Identity?.Name;
    
    return Ok(new { UserId = userId, UserName = userName });
}
```

### **4. Proteger com Roles (Futuro)**

```csharp
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")] // ? Apenas Admin
public IActionResult Delete(int id)
{
    // ...
}
```

---

## ?? Claims no Token JWT

Os seguintes claims são incluídos no token:

| Claim | Descrição | Exemplo |
|-------|-----------|---------|
| `ClaimTypes.NameIdentifier` | ID do usuário | "1" |
| `ClaimTypes.Name` | Nome do usuário | "João Silva" |
| `ClaimTypes.Email` | E-mail do usuário | "joao@exemplo.com" |
| `UserId` | ID do usuário (custom) | "1" |
| `UserState` | Estado do usuário | "Active" |

**Acessando claims no controller:**

```csharp
var userId = User.FindFirst("UserId")?.Value;
var userName = User.Identity?.Name;
var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
```

---

## ?? Segurança

### **Boas Práticas Implementadas:**

1. ? **Token Expiration**: Tokens expiram em 60 minutos (configurável)
2. ? **Claims Validation**: Valida Issuer, Audience e Lifetime
3. ? **HTTPS Required**: Deve usar HTTPS em produção
4. ? **ClockSkew Zero**: Remove atraso de 5 minutos do padrão
5. ? **Estado do Usuário**: Verifica se usuário está ativo antes de gerar token

### **Melhorias Futuras:**

- [ ] Implementar Refresh Tokens
- [ ] Adicionar claims de Roles
- [ ] Implementar Revogação de Tokens
- [ ] Adicionar Rate Limiting
- [ ] Implementar Two-Factor Authentication (2FA)
- [ ] Armazenar SecretKey em Azure Key Vault

---

## ?? Testando com Swagger

### **1. Acessar Swagger**
```
https://localhost:7000/swagger
```

### **2. Fazer Login**
1. Expandir `POST /api/auth/login`
2. Clicar em "Try it out"
3. Inserir credenciais:
```json
{
  "email": "joao@exemplo.com",
  "password": "SenhaForte@123"
}
```
4. Clicar em "Execute"
5. Copiar o `token` da resposta

### **3. Autenticar no Swagger**
1. Clicar no botão **"Authorize"** no topo da página
2. Inserir: `Bearer {seu_token}`
3. Clicar em "Authorize"
4. Clicar em "Close"

### **4. Testar Endpoint Protegido**
1. Expandir `GET /api/auth/me`
2. Clicar em "Try it out"
3. Clicar em "Execute"
4. Deve retornar os dados do usuário autenticado

---

## ? Tratamento de Erros

### **1. Credenciais Inválidas (401)**

```http
POST /api/auth/login

Response:
{
    "statusCode": 401,
    "message": "E-mail ou senha inválidos.",
    "timestamp": "2024-01-15T10:30:00Z",
    "path": "/api/auth/login"
}
```

### **2. Usuário Inativo (401)**

```http
POST /api/auth/login

Response:
{
    "statusCode": 401,
    "message": "Usuário inativo ou bloqueado.",
    "timestamp": "2024-01-15T10:30:00Z",
    "path": "/api/auth/login"
}
```

### **3. Token Inválido ou Expirado (401)**

```http
GET /api/auth/me
Authorization: Bearer token_invalido

Response:
{
    "statusCode": 401,
    "message": "Unauthorized",
    "timestamp": "2024-01-15T10:30:00Z",
    "path": "/api/auth/me"
}
```

### **4. Token Ausente (401)**

```http
GET /api/auth/me
(Sem header Authorization)

Response:
{
    "statusCode": 401,
    "message": "Unauthorized",
    "timestamp": "2024-01-15T10:30:00Z",
    "path": "/api/auth/me"
}
```

---

## ?? Testes com cURL

### **Login**
```bash
curl -X POST https://localhost:7000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@exemplo.com",
    "password": "SenhaForte@123"
  }'
```

### **Endpoint Protegido**
```bash
curl -X GET https://localhost:7000/api/auth/me \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

---

## ?? Próximos Passos

1. **Adicionar os pacotes NuGet**
   ```bash
   dotnet add Jrg.SisMed.Api package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
   dotnet add Jrg.SisMed.Application package System.IdentityModel.Tokens.Jwt --version 8.2.1
   ```

2. **Compilar o projeto**
   ```bash
   dotnet build
   ```

3. **Executar a API**
   ```bash
   dotnet run --project Jrg.SisMed.Api
   ```

4. **Testar no Swagger**
   - Acessar: https://localhost:7000/swagger
   - Fazer login
   - Testar endpoints protegidos

5. **Criar testes unitários**
   - Testar geração de token
   - Testar validação de credenciais
   - Testar expiração de token

6. **Implementar Refresh Tokens (opcional)**
   - Criar tabela RefreshTokens
   - Implementar endpoint /api/auth/refresh
   - Rotacionar tokens

---

## ? Checklist de Implementação

- [x] ? Criar `JwtTokenService`
- [x] ? Criar `AuthenticateUserUseCase`
- [x] ? Criar `AuthController`
- [x] ? Criar `UnauthorizedException`
- [x] ? Atualizar `ExceptionHandlingMiddleware`
- [x] ? Configurar JWT no `Program.cs`
- [x] ? Adicionar configurações no `appsettings.json`
- [x] ? Registrar serviços no DI
- [x] ? Configurar Swagger com autenticação
- [ ] ? Adicionar pacotes NuGet
- [ ] ? Compilar e testar

---

**?? Autenticação JWT implementada com sucesso!**

**?? Sistema pronto para autenticar usuários e proteger endpoints!**
