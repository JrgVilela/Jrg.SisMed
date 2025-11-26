# ?? CORS - Configuração Completa

## ? O que foi implementado

Configuração completa de CORS (Cross-Origin Resource Sharing) na API para permitir que aplicações frontend em diferentes domínios possam acessar a API.

---

## ?? Políticas de CORS Configuradas

### **1. Política "AllowAll" (Desenvolvimento)**

Permite **TODAS** as origens, métodos e headers. Ideal para desenvolvimento local.

```csharp
options.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader();
});
```

**Uso:**
- ? Desenvolvimento local
- ? Testes com Postman/Insomnia
- ? Frontend em localhost
- ? **NÃO** usar em produção

### **2. Política "Production" (Produção)**

Permite **APENAS** origens específicas configuradas. Recomendado para produção.

```csharp
options.AddPolicy("Production", policy =>
{
    policy.WithOrigins(
            "https://seudominio.com.br",
            "https://www.seudominio.com.br",
            "https://app.seudominio.com.br"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});
```

**Uso:**
- ? Produção
- ? Staging
- ? Segurança reforçada
- ? Apenas domínios autorizados

---

## ?? Configuração no appsettings.json

### **appsettings.json**

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "https://seudominio.com.br",
      "https://www.seudominio.com.br",
      "https://app.seudominio.com.br"
    ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ],
    "AllowedHeaders": [ "*" ],
    "AllowCredentials": true
  }
}
```

### **appsettings.Development.json (para desenvolvimento)**

```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5173",
      "http://localhost:8080"
    ],
    "AllowedMethods": [ "*" ],
    "AllowedHeaders": [ "*" ],
    "AllowCredentials": true
  }
}
```

---

## ?? Configuração no Program.cs

### **Ordem Correta dos Middlewares:**

**?? IMPORTANTE:** A ordem é crucial para o funcionamento correto!

```csharp
var app = builder.Build();

// 1. Exception Handling (primeiro)
app.UseExceptionHandling();

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. CORS (ANTES de Authentication)
app.UseCors("AllowAll"); // ou "Production"

// 4. Authentication
app.UseAuthentication();

// 5. Authorization
app.UseAuthorization();

// 6. Controllers (por último)
app.MapControllers();

app.Run();
```

### **Seleção Automática por Ambiente:**

```csharp
// Usa "AllowAll" em desenvolvimento e "Production" em produção
var corsPolicy = app.Environment.IsDevelopment() ? "AllowAll" : "Production";
app.UseCors(corsPolicy);
```

---

## ?? Cenários de Uso

### **Cenário 1: Frontend React em localhost**

**Desenvolvimento:**
```javascript
// Frontend em http://localhost:3000
fetch('https://localhost:7000/api/auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        email: 'usuario@exemplo.com',
        password: 'senha123'
    })
})
.then(response => response.json())
.then(data => console.log(data));
```

**Configuração necessária:**
- Política CORS: `AllowAll` (desenvolvimento)
- Sem configuração adicional no frontend

### **Cenário 2: Frontend Angular em produção**

**Produção:**
```typescript
// Frontend em https://app.seudominio.com.br
this.http.post('https://api.seudominio.com.br/api/auth/login', {
    email: 'usuario@exemplo.com',
    password: 'senha123'
}, {
    headers: {
        'Content-Type': 'application/json'
    },
    withCredentials: true // Necessário com AllowCredentials
})
.subscribe(data => console.log(data));
```

**Configuração necessária:**
- Política CORS: `Production`
- Adicionar `https://app.seudominio.com.br` em `AllowedOrigins`
- `AllowCredentials: true` no appsettings.json

### **Cenário 3: Mobile App (React Native / Flutter)**

```javascript
// App mobile
fetch('https://api.seudominio.com.br/api/auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer token_aqui'
    },
    body: JSON.stringify({
        email: 'usuario@exemplo.com',
        password: 'senha123'
    })
})
```

**Nota:** Apps mobile nativos **não** são afetados por CORS, mas a configuração não causa problemas.

---

## ?? Segurança

### **Boas Práticas:**

#### **? Fazer:**

1. **Em Produção:**
   - Usar apenas a política `Production`
   - Listar **explicitamente** as origens permitidas
   - Evitar `AllowAnyOrigin()` em produção

2. **Com Credenciais:**
   - Se usar `AllowCredentials()`, **não** pode usar `AllowAnyOrigin()`
   - Deve especificar origens exatas

3. **Headers:**
   - Listar explicitamente headers permitidos
   - Evitar `AllowAnyHeader()` em produção

4. **Métodos:**
   - Permitir apenas métodos necessários (GET, POST, PUT, DELETE)

#### **? Evitar:**

1. `AllowAnyOrigin()` em produção
2. `AllowAnyHeader()` em produção
3. `AllowCredentials()` com `AllowAnyOrigin()`
4. Não validar origens

### **Exemplo de Política Segura:**

```csharp
options.AddPolicy("SecureProduction", policy =>
{
    policy.WithOrigins(
            "https://app.seudominio.com.br",
            "https://admin.seudominio.com.br"
        )
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .WithHeaders("Content-Type", "Authorization")
        .AllowCredentials()
        .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
});
```

---

## ?? Testando CORS

### **Teste 1: Requisição Simples (GET)**

```javascript
fetch('https://localhost:7000/api/auth/me', {
    headers: {
        'Authorization': 'Bearer seu_token'
    }
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Erro CORS:', error));
```

### **Teste 2: Requisição Preflight (POST)**

```javascript
fetch('https://localhost:7000/api/professional/register', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({ /* dados */ })
})
.then(response => response.json())
.then(data => console.log(data))
.catch(error => console.error('Erro CORS:', error));
```

**Nota:** O navegador envia automaticamente uma requisição OPTIONS (preflight) antes do POST.

### **Teste 3: com cURL**

```bash
# Teste de Preflight
curl -X OPTIONS https://localhost:7000/api/auth/login \
  -H "Origin: http://localhost:3000" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: Content-Type" \
  -v

# Resposta esperada:
# Access-Control-Allow-Origin: http://localhost:3000
# Access-Control-Allow-Methods: GET, POST, PUT, DELETE
# Access-Control-Allow-Headers: Content-Type
```

---

## ?? Troubleshooting

### **Problema 1: "CORS policy: No 'Access-Control-Allow-Origin' header"**

**Causa:** CORS não configurado ou origem não permitida.

**Solução:**
1. Verificar se `app.UseCors()` está antes de `app.UseAuthentication()`
2. Verificar se a origem está em `AllowedOrigins`
3. Usar política `AllowAll` para testes

### **Problema 2: "The 'Access-Control-Allow-Origin' header contains multiple values"**

**Causa:** CORS configurado mais de uma vez.

**Solução:**
1. Chamar `app.UseCors()` apenas **uma vez**
2. Verificar se não há configurações duplicadas

### **Problema 3: Preflight retorna 401 Unauthorized**

**Causa:** Middleware de autenticação antes do CORS.

**Solução:**
```csharp
// ORDEM CORRETA:
app.UseCors("AllowAll");      // 1º
app.UseAuthentication();      // 2º
app.UseAuthorization();       // 3º
```

### **Problema 4: Credenciais não funcionam**

**Causa:** `AllowAnyOrigin()` com `AllowCredentials()`.

**Solução:**
```csharp
// NÃO FUNCIONA:
policy.AllowAnyOrigin().AllowCredentials(); // ?

// FUNCIONA:
policy.WithOrigins("https://app.exemplo.com")
      .AllowCredentials(); // ?
```

---

## ?? Headers CORS

### **Headers de Resposta:**

| Header | Descrição | Exemplo |
|--------|-----------|---------|
| `Access-Control-Allow-Origin` | Origem permitida | `https://app.exemplo.com` |
| `Access-Control-Allow-Methods` | Métodos permitidos | `GET, POST, PUT, DELETE` |
| `Access-Control-Allow-Headers` | Headers permitidos | `Content-Type, Authorization` |
| `Access-Control-Allow-Credentials` | Permite credenciais | `true` |
| `Access-Control-Max-Age` | Cache do preflight | `600` (10 minutos) |

### **Headers de Requisição (Preflight):**

| Header | Descrição | Exemplo |
|--------|-----------|---------|
| `Origin` | Origem da requisição | `https://app.exemplo.com` |
| `Access-Control-Request-Method` | Método da requisição real | `POST` |
| `Access-Control-Request-Headers` | Headers da requisição real | `Content-Type, Authorization` |

---

## ?? Configuração por Ambiente

### **Development**
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5173"
    ]
  }
}
```

### **Staging**
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "https://staging.seudominio.com.br",
      "https://app-staging.seudominio.com.br"
    ]
  }
}
```

### **Production**
```json
{
  "CorsSettings": {
    "AllowedOrigins": [
      "https://seudominio.com.br",
      "https://www.seudominio.com.br",
      "https://app.seudominio.com.br"
    ]
  }
}
```

---

## ? Checklist de Implementação

- [x] ? CORS configurado no `Program.cs`
- [x] ? Política `AllowAll` para desenvolvimento
- [x] ? Política `Production` para produção
- [x] ? Configurações no `appsettings.json`
- [x] ? Middleware na ordem correta
- [x] ? Seleção automática por ambiente
- [x] ? Build bem-sucedido
- [ ] ? Testar com frontend
- [ ] ? Configurar domínios de produção

---

## ?? Próximos Passos

1. **Atualizar domínios em produção**
   - Editar `appsettings.json`
   - Adicionar domínios reais em `AllowedOrigins`

2. **Testar com frontend**
   - React/Angular/Vue em localhost
   - Verificar requisições OPTIONS (preflight)
   - Testar com credenciais

3. **Deploy em produção**
   - Verificar política CORS usada
   - Validar origens permitidas
   - Testar com aplicação real

4. **Monitorar logs**
   - Verificar política CORS ativa
   - Origens sendo permitidas/bloqueadas

---

**?? CORS configurado com sucesso!**

**?? API pronta para aceitar requisições de aplicações frontend!**
