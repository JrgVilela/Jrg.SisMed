# ?? Swagger Corrigido - Guia de Acesso

## ? Correções Aplicadas

### 1. **launchSettings.json** Atualizado
Agora o navegador abre automaticamente no Swagger quando você executa a API.

### 2. **RoutePrefix Alterado**
O Swagger agora está disponível na **raiz** da aplicação para facilitar o acesso.

---

## ?? Como Acessar Agora

### Opção 1: Executar pelo Visual Studio
1. Pressione **F5** ou clique em "Play" (??)
2. O navegador abrirá automaticamente na URL: `https://localhost:7056/`
3. Você verá o Swagger UI imediatamente

### Opção 2: Executar pelo Terminal
```bash
dotnet run --project Jrg.SisMed.Api
```

Depois acesse uma destas URLs:
- **HTTPS:** `https://localhost:7056/`
- **HTTP:** `http://localhost:5063/`

---

## ?? URLs Disponíveis

| URL | Descrição |
|-----|-----------|
| `https://localhost:7056/` | **Swagger UI** (Interface visual) |
| `https://localhost:7056/swagger/v1/swagger.json` | Documento JSON da API |
| `https://localhost:7056/api/...` | Endpoints da sua API |

---

## ?? O Que Mudou?

### Antes ?
```csharp
options.RoutePrefix = "swagger"; // Precisava acessar /swagger
```
- Precisava navegar manualmente para `/swagger`
- launchSettings não abria o navegador automaticamente

### Depois ?
```csharp
options.RoutePrefix = string.Empty; // Swagger na raiz!
```
- Swagger disponível na raiz (`/`)
- Navegador abre automaticamente
- launchSettings configurado para abrir no Swagger

---

## ?? Troubleshooting

### Problema 1: "Não abre nada ao executar"
**Solução:**
1. Verifique se está usando o perfil correto no Visual Studio (https ou http)
2. Verifique se `ASPNETCORE_ENVIRONMENT=Development` está configurado
3. Tente executar com: `dotnet run --launch-profile https`

### Problema 2: "Página não encontrada (404)"
**Solução:**
1. Confirme que a aplicação iniciou corretamente (veja o console)
2. Verifique a porta correta (7056 para HTTPS, 5063 para HTTP)
3. Tente acessar diretamente: `https://localhost:7056/swagger/index.html`

### Problema 3: "ERR_CONNECTION_REFUSED"
**Solução:**
1. Certifique-se de que a aplicação está rodando
2. Verifique se outra aplicação não está usando a mesma porta
3. Tente trocar de perfil (http em vez de https)

### Problema 4: "Certificate Error (SSL)"
**Solução:**
1. Confie no certificado de desenvolvimento:
```bash
dotnet dev-certs https --trust
```
2. Ou use o perfil HTTP em vez de HTTPS

### Problema 5: "Swagger não aparece no Production"
**Isso é normal!** O Swagger está configurado apenas para **Development**.

Se quiser habilitar em outros ambientes, altere o `Program.cs`:
```csharp
// Remove a verificação de ambiente
// if (app.Environment.IsDevelopment())  ? Remove isso
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }
```

---

## ?? Checklist de Verificação

Antes de executar, confirme:
- [ ] Projeto compilando sem erros (`dotnet build`)
- [ ] `ASPNETCORE_ENVIRONMENT=Development` configurado
- [ ] Porta 7056 (HTTPS) ou 5063 (HTTP) disponível
- [ ] Certificado HTTPS confiável (se usar HTTPS)
- [ ] Visual Studio ou CLI atualizado

---

## ?? O Que Você Verá

Ao abrir `https://localhost:7056/`, você verá:

```
?????????????????????????????????????????????????
?           Jrg.SisMed API v1                  ?
?  API para Sistema Médico - Gerenciamento...  ?
?????????????????????????????????????????????????
?                                               ?
?  [Authorize] ??                              ?
?                                               ?
?  ?? WeatherForecast                          ?
?    GET /WeatherForecast                       ?
?                                               ?
?  Schemas                                      ?
?    WeatherForecast                            ?
?                                               ?
?????????????????????????????????????????????????
```

---

## ?? Dicas Úteis

### 1. Mudar a Porta
Edite `launchSettings.json`:
```json
"applicationUrl": "https://localhost:7056;http://localhost:5063"
                              ????                    ????
                            Altere aqui           Altere aqui
```

### 2. Desabilitar HTTPS Redirect (para testes locais)
Comente no `Program.cs`:
```csharp
// app.UseHttpsRedirection();  // Comentar temporariamente
```

### 3. Ver Logs Detalhados
Execute com:
```bash
dotnet run --project Jrg.SisMed.Api --verbosity detailed
```

### 4. Testar sem Visual Studio
```bash
cd Jrg.SisMed.Api
dotnet watch run
```
O `watch` reinicia automaticamente ao detectar mudanças.

---

## ?? Ainda Não Funciona?

### Passos de Diagnóstico:

1. **Verifique se a aplicação está rodando:**
```bash
dotnet run --project Jrg.SisMed.Api
```

Deve aparecer:
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7056
      Now listening on: http://localhost:5063
```

2. **Teste a API diretamente:**
```bash
curl https://localhost:7056/WeatherForecast
```

3. **Verifique o ambiente:**
```bash
echo $env:ASPNETCORE_ENVIRONMENT  # Windows PowerShell
echo $ASPNETCORE_ENVIRONMENT      # Linux/Mac
```

Deve retornar: `Development`

4. **Verifique se há erros no console:**
- Procure por mensagens de erro em vermelho
- Verifique se há problemas de configuração

---

## ?? Teste Rápido

Execute este comando no PowerShell/Terminal:

```powershell
# Windows PowerShell
Start-Process "https://localhost:7056/"

# Ou manualmente, abra seu navegador e digite:
# https://localhost:7056/
```

Se aparecer a interface do Swagger, está tudo funcionando! ?

---

## ?? Próximos Passos

Agora que o Swagger está funcionando:

1. ? Crie seus Controllers
2. ? Adicione atributos `[HttpGet]`, `[HttpPost]`, etc.
3. ? Documente com comentários XML
4. ? Teste seus endpoints no Swagger UI

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Status:** ? Swagger Corrigido e Funcional
**Acesso:** `https://localhost:7056/` ou `http://localhost:5063/`
