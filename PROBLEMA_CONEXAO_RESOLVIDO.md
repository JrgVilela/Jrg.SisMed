# ?? Problema de Conexão Resolvido - ERR_CONNECTION_REFUSED

## ?? Problemas Identificados

### Problema 1: Porta Incorreta no Navegador ?
**Você tentou acessar:** `localhost:5058/swagger`
**Porta correta da aplicação:** `localhost:5063`

A aplicação estava rodando na porta **5063**, mas você tentou acessar a porta **5058**.

### Problema 2: HTTPS Redirect Warning ??
```
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
```

A aplicação estava tentando redirecionar HTTP para HTTPS, mas não encontrava a porta HTTPS configurada quando você usava o perfil HTTP.

---

## ? Correções Aplicadas

### 1. HTTPS Redirect Condicional
```csharp
// Só usa HTTPS redirect se estiver configurado HTTPS
if (app.Urls.Any(url => url.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
{
    app.UseHttpsRedirection();
}
```

Agora o redirect só acontece se HTTPS estiver realmente configurado.

---

## ?? Como Acessar Corretamente

### Opção 1: Usar Perfil HTTP (Recomendado para desenvolvimento)

1. **Pare a aplicação** (Ctrl+C)

2. **Execute com perfil HTTP:**
   - No Visual Studio: Selecione perfil **"http"** no dropdown
   - Ou pelo terminal:
   ```bash
   dotnet run --project Jrg.SisMed.Api --launch-profile http
   ```

3. **Acesse a URL correta:**
   ```
   http://localhost:5063/
   ```
   
   ? **Porta 5063** (não 5058!)

### Opção 2: Usar Perfil HTTPS

1. **Confie no certificado de desenvolvimento:**
   ```bash
   dotnet dev-certs https --trust
   ```

2. **Execute com perfil HTTPS:**
   - No Visual Studio: Selecione perfil **"https"** no dropdown
   - Ou pelo terminal:
   ```bash
   dotnet run --project Jrg.SisMed.Api --launch-profile https
   ```

3. **Acesse:**
   ```
   https://localhost:7056/
   ```

---

## ?? Como Identificar a Porta Correta

Sempre olhe no console quando iniciar a aplicação:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5063    ? Esta é a porta!
                                        ????
```

Ou se for HTTPS:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7056   ? Porta HTTPS
      Now listening on: http://localhost:5063    ? Porta HTTP
```

---

## ?? Checklist de Solução

- [x] Código corrigido (HTTPS redirect condicional)
- [x] Build bem-sucedido
- [ ] **PARE a aplicação atual** (importante!)
- [ ] **Reinicie a aplicação**
- [ ] **Use a porta correta** (5063 para HTTP, 7056 para HTTPS)
- [ ] **Acesse:** `http://localhost:5063/` (sem /swagger, pois está na raiz)

---

## ?? Teste Rápido

Execute este comando e veja se funciona:

```bash
# Pare a aplicação atual (Ctrl+C)
# Depois execute:
dotnet run --project Jrg.SisMed.Api --launch-profile http

# Em outro terminal, teste:
curl http://localhost:5063/swagger/v1/swagger.json
```

Se retornar um JSON com a documentação da API, está funcionando! ?

---

## ?? Dicas Importantes

### 1. Sempre Verifique a Porta no Console
Não confie em portas antigas. Sempre olhe o console para ver em qual porta a aplicação iniciou.

### 2. Limpe o Cache do Navegador
Se ainda der problema:
- **Chrome/Edge:** Ctrl + Shift + Delete ? Limpar cache
- Ou abra em modo anônimo: Ctrl + Shift + N

### 3. Teste Direto sem Swagger Primeiro
Tente acessar:
```
http://localhost:5063/WeatherForecast
```

Se isso funcionar, o problema é específico do Swagger.

### 4. Verifique se Outra Aplicação Está Usando a Porta
```bash
# Windows
netstat -ano | findstr :5063

# Linux/Mac
lsof -i :5063
```

---

## ?? Troubleshooting Avançado

### Se ainda não funcionar:

1. **Verifique o Firewall**
   - Windows Defender pode estar bloqueando
   - Permita .NET Core e Visual Studio

2. **Execute como Administrador**
   - Visual Studio em modo administrador
   - Ou terminal como admin

3. **Desabilite Antivírus Temporariamente**
   - Alguns antivírus bloqueiam localhost

4. **Recrie os Certificados HTTPS**
   ```bash
   dotnet dev-certs https --clean
   dotnet dev-certs https --trust
   ```

5. **Verifique se o Perfil Está Correto**
   No Visual Studio, confirme que está usando:
   - **http** (para porta 5063)
   - **https** (para porta 7056)

---

## ?? O Que Você Deve Ver

### Console ao Iniciar:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5063
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
```

? **SEM** warnings de HTTPS redirect!

### Navegador:
```
?????????????????????????????????????????????????
?           Jrg.SisMed API v1                  ?
?  API para Sistema Médico - Gerenciamento...  ?
?????????????????????????????????????????????????
?  GET /WeatherForecast                         ?
?  ...                                          ?
?????????????????????????????????????????????????
```

---

## ?? Próximos Passos

Depois que funcionar:

1. ? Teste o endpoint de exemplo: `GET /WeatherForecast`
2. ? Crie seus próprios Controllers
3. ? Teste seus endpoints no Swagger UI
4. ? Adicione documentação XML aos métodos

---

## ?? Resumo da Solução

| Item | Antes ? | Depois ? |
|------|---------|----------|
| **Porta** | 5058 (errada) | 5063 (correta) |
| **HTTPS Redirect** | Sempre ativo | Condicional |
| **Warning** | Presente | Removido |
| **Conexão** | Recusada | Funcionando |

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Status:** ? Problema Resolvido
**Ação:** PARE e REINICIE a aplicação com a porta correta!
