# ?? Exemplos de Teste - Sistema de Tratamento de Exceções

## Como Testar o Sistema de Exceções

Use estas requisições para testar cada tipo de erro e verificar se as respostas estão corretas.

---

## 1?? Teste de Validação de Domínio (400 Bad Request)

### Cenário: Criar organização com dados inválidos

**Request:**
```http
POST /api/organizacao
Content-Type: application/json

{
  "nameFantasia": "",           // ? Vazio
  "razaoSocial": "Test",
  "cnpj": "11111111111111",     // ? CNPJ inválido (todos iguais)
  "state": 1
}
```

**Resposta Esperada:**
```json
{
  "statusCode": 400,
  "message": "One or more validation errors occurred.",
  "errors": [
    "Trade name is required",
    "CNPJ is invalid"
  ],
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao"
}
```

---

## 2?? Teste de Recurso Não Encontrado (404 Not Found)

### Cenário A: Buscar organização por ID inexistente

**Request:**
```http
GET /api/organizacao/99999
```

**Resposta Esperada:**
```json
{
  "statusCode": 404,
  "message": "Organization with identifier '99999' was not found.",
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao/99999"
}
```

### Cenário B: Buscar organização por CNPJ inexistente

**Request:**
```http
GET /api/organizacao/search?cnpj=12345678000195
```

**Resposta Esperada:**
```json
{
  "statusCode": 404,
  "message": "Organization with identifier '12345678000195' was not found.",
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao/search"
}
```

---

## 3?? Teste de Conflito/Duplicidade (409 Conflict)

### Cenário: Criar organização com CNPJ já existente

**Passo 1 - Criar primeira organização:**
```http
POST /api/organizacao
Content-Type: application/json

{
  "nameFantasia": "Clínica Saúde",
  "razaoSocial": "Clínica Saúde Ltda",
  "cnpj": "11.222.333/0001-81",
  "state": 1
}
```

**Passo 2 - Tentar criar outra com mesmo CNPJ:**
```http
POST /api/organizacao
Content-Type: application/json

{
  "nameFantasia": "Clínica Bem Estar",
  "razaoSocial": "Clínica Bem Estar Ltda",
  "cnpj": "11.222.333/0001-81",    // ? Mesmo CNPJ
  "state": 1
}
```

**Resposta Esperada:**
```json
{
  "statusCode": 409,
  "message": "Organization with CNPJ '11222333000181' already exists.",
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao"
}
```

---

## 4?? Teste de Argumento Inválido (400 Bad Request)

### Cenário: Buscar organização sem informar CNPJ

**Request:**
```http
GET /api/organizacao/search?cnpj=
```

**Resposta Esperada:**
```json
{
  "statusCode": 400,
  "message": "CNPJ is required",
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao/search"
}
```

### Cenário: CNPJ em formato inválido

**Request:**
```http
GET /api/organizacao/search?cnpj=123abc
```

**Resposta Esperada:**
```json
{
  "statusCode": 400,
  "message": "CNPJ '123abc' is invalid.",
  "timestamp": "2024-01-15T23:15:30Z",
  "path": "/api/organizacao/search"
}
```

---

## 5?? Teste de Sucesso (Status 2xx)

### Cenário A: Criar organização com dados válidos

**Request:**
```http
POST /api/organizacao
Content-Type: application/json

{
  "nameFantasia": "Clínica Saúde Total",
  "razaoSocial": "Clínica Saúde Total Ltda",
  "cnpj": "11.222.333/0001-81",
  "state": 1
}
```

**Resposta Esperada:**
```json
HTTP/1.1 201 Created
Location: /api/organizacao/1

{
  "id": 1,
  "message": "Organization created successfully"
}
```

### Cenário B: Obter organização existente

**Request:**
```http
GET /api/organizacao/1
```

**Resposta Esperada:**
```json
HTTP/1.1 200 OK

{
  "id": 1,
  "nameFantasia": "Clínica Saúde Total",
  "razaoSocial": "Clínica Saúde Total Ltda",
  "cnpj": "11222333000181",
  "principalDdi": "",
  "principalDdd": "",
  "principalPhone": "",
  "principalPhoneFormatted": "",
  "principalPhoneFullFormatted": "",
  "state": 1
}
```

### Cenário C: Atualizar organização

**Request:**
```http
PUT /api/organizacao/1
Content-Type: application/json

{
  "nameFantasia": "Clínica Saúde Total Atualizada",
  "razaoSocial": "Clínica Saúde Total Ltda",
  "cnpj": "11.222.333/0001-81",
  "state": 1
}
```

**Resposta Esperada:**
```json
HTTP/1.1 200 OK

{
  "message": "Organization updated successfully"
}
```

### Cenário D: Deletar organização

**Request:**
```http
DELETE /api/organizacao/1
```

**Resposta Esperada:**
```json
HTTP/1.1 204 No Content
```

---

## ?? Testando com Ferramentas

### Swagger UI
1. Execute a API
2. Abra: `https://localhost:{porta}/swagger`
3. Teste cada endpoint interativamente

### Postman
Importe esta collection:

```json
{
  "info": {
    "name": "Jrg.SisMed - Exception Handling Tests",
    "schema": "https://schema.getpostman.com/json/collection/v2.1.0/collection.json"
  },
  "item": [
    {
      "name": "1. Validation Error (400)",
      "request": {
        "method": "POST",
        "url": "{{baseUrl}}/api/organizacao",
        "body": {
          "mode": "raw",
          "raw": "{\n  \"nameFantasia\": \"\",\n  \"razaoSocial\": \"Test\",\n  \"cnpj\": \"11111111111111\",\n  \"state\": 1\n}",
          "options": {
            "raw": {
              "language": "json"
            }
          }
        }
      }
    },
    {
      "name": "2. Not Found (404)",
      "request": {
        "method": "GET",
        "url": "{{baseUrl}}/api/organizacao/99999"
      }
    },
    {
      "name": "3. Conflict (409)",
      "request": {
        "method": "POST",
        "url": "{{baseUrl}}/api/organizacao",
        "body": {
          "mode": "raw",
          "raw": "{\n  \"nameFantasia\": \"Duplicate\",\n  \"razaoSocial\": \"Duplicate Ltda\",\n  \"cnpj\": \"11.222.333/0001-81\",\n  \"state\": 1\n}",
          "options": {
            "raw": {
              "language": "json"
            }
          }
        }
      }
    }
  ],
  "variable": [
    {
      "key": "baseUrl",
      "value": "https://localhost:7147",
      "type": "string"
    }
  ]
}
```

### cURL

#### Teste 1: Validação (400)
```bash
curl -X POST "https://localhost:7147/api/organizacao" \
  -H "Content-Type: application/json" \
  -d '{
    "nameFantasia": "",
    "razaoSocial": "Test",
    "cnpj": "11111111111111",
    "state": 1
  }'
```

#### Teste 2: Not Found (404)
```bash
curl -X GET "https://localhost:7147/api/organizacao/99999"
```

#### Teste 3: Busca sem CNPJ (400)
```bash
curl -X GET "https://localhost:7147/api/organizacao/search?cnpj="
```

---

## ?? Matriz de Testes

| # | Endpoint | Método | Cenário | Status Esperado |
|---|----------|--------|---------|-----------------|
| 1 | `/api/organizacao` | POST | Dados inválidos | 400 Bad Request |
| 2 | `/api/organizacao/{id}` | GET | ID inexistente | 404 Not Found |
| 3 | `/api/organizacao/search` | GET | CNPJ inexistente | 404 Not Found |
| 4 | `/api/organizacao` | POST | CNPJ duplicado | 409 Conflict |
| 5 | `/api/organizacao/search` | GET | CNPJ vazio | 400 Bad Request |
| 6 | `/api/organizacao/search` | GET | CNPJ inválido | 400 Bad Request |
| 7 | `/api/organizacao` | POST | Dados válidos | 201 Created |
| 8 | `/api/organizacao/{id}` | GET | ID existente | 200 OK |
| 9 | `/api/organizacao/{id}` | PUT | Atualização válida | 200 OK |
| 10 | `/api/organizacao/{id}` | PUT | ID inexistente | 404 Not Found |
| 11 | `/api/organizacao/{id}` | DELETE | ID existente | 204 No Content |
| 12 | `/api/organizacao/{id}` | DELETE | ID inexistente | 404 Not Found |

---

## ? Checklist de Validação

Após executar todos os testes, verifique:

- [ ] Erros de validação retornam 400 com lista de erros
- [ ] Recursos não encontrados retornam 404 com mensagem clara
- [ ] Conflitos retornam 409 com mensagem descritiva
- [ ] Todas as respostas incluem `statusCode`, `message`, `timestamp` e `path`
- [ ] Respostas de sucesso não incluem estrutura de erro
- [ ] Em Development, erros 500 incluem `details` e stack trace
- [ ] Em Production, erros 500 não expõem detalhes técnicos
- [ ] Logs são gerados para todas as exceções
- [ ] JSON está bem formatado (indentado em Development)

---

## ?? CNPJs Válidos para Testes

Use estes CNPJs válidos para criar organizações de teste:

```
11.222.333/0001-81  ?  11222333000181
12.345.678/0001-95  ?  12345678000195
43.133.410/0001-13  ?  43133410000113
00.000.000/0001-91  ?  00000000000191
```

**Gerador Online**: https://www.4devs.com.br/gerador_de_cnpj

---

## ?? Notas Importantes

1. **CNPJ Formatting**: A API aceita CNPJ com ou sem formatação
2. **Normalização**: CNPJ é sempre armazenado apenas com números
3. **Case Sensitive**: Emails e nomes são normalizados (lowercase/titlecase)
4. **Estado Default**: Organizations são criadas como `Active` por padrão
5. **IDs Sequenciais**: Os IDs são gerados automaticamente pelo banco

---

**Última Atualização**: 15/01/2024  
**Versão da API**: 1.0
