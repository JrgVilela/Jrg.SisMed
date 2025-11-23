# ?? Validações de Unicidade no Registro de Profissionais

## ?? Objetivo

Implementar validações de unicidade no banco de dados para garantir que não sejam cadastrados dados duplicados durante o registro de profissionais.

---

## ?? Validações Implementadas

### **1. Profissional** ?????

| Campo | Validação | Mensagem de Erro |
|-------|-----------|------------------|
| **CPF** | Deve ser único no sistema | "Já existe um profissional cadastrado com este CPF." |
| **Número de Registro** | Deve ser único por tipo de profissional<br>(CRP para Psicólogos, CRN para Nutricionistas) | "Já existe um profissional cadastrado com este número de registro." |

### **2. Usuário** ??

| Campo | Validação | Mensagem de Erro |
|-------|-----------|------------------|
| **Email** | Deve ser único no sistema | "Já existe um usuário cadastrado com este e-mail." |

### **3. Organização** ??

| Campo | Validação | Mensagem de Erro |
|-------|-----------|------------------|
| **CNPJ** | Deve ser único no sistema | "Já existe uma organização com o CNPJ {cnpj}." |
| **Razão Social** | Deve ser única no sistema | "Já existe uma organização com a mesma razão social." |

---

## ??? Arquitetura Implementada

```
RegisterProfessionalUseCase
           ?
   FluentValidation (formato dos dados)
           ?
PsychologyRegisterService
           ?
   ValidateUniquenessAsync() (unicidade no BD)
           ?
   ????????????????????????????????????????
   ?  Verifica no Banco de Dados:         ?
   ?  1. CPF exists?                      ?
   ?  2. RegisterNumber (CRP) exists?     ?
   ?  3. Email exists?                    ?
   ?  4. CNPJ exists?                     ?
   ?  5. RazaoSocial exists?              ?
   ????????????????????????????????????????
           ?
   Se algum existir ? ConflictException (HTTP 409)
   Se todos OK ? Registra no BD
```

---

## ?? Implementação

### **1. Interface IProfessionalRepository**

```csharp
public interface IProfessionalRepository<T> : IRepository<T>
{
    /// <summary>
    /// Verifica se um CPF já está cadastrado.
    /// </summary>
    Task<bool> CpfExistsAsync(
        string cpf, 
        int? excludeUserId = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um número de registro profissional já está cadastrado.
    /// Para Psychologist, verifica o CRP. Para Nutritionist, verifica o CRN, etc.
    /// </summary>
    Task<bool> RegisterNumberExistsAsync(
        string registerNumber, 
        int? excludeProfessionalId = null, 
        CancellationToken cancellationToken = default);
}
```

### **2. PsychologyRepository**

```csharp
public class PsychologyRepository : Repository<Psychologist>, IProfessionalRepository<Psychologist>
{
    public async Task<bool> CpfExistsAsync(
        string cpf, 
        int? excludeUserId = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        var query = _dbSet.Where(p => p.Cpf == cpf);

        if (excludeUserId.HasValue)
            query = query.Where(p => p.Id != excludeUserId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> RegisterNumberExistsAsync(
        string registerNumber, 
        int? excludeProfessionalId = null, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(registerNumber))
            return false;

        var query = _dbSet.Where(p => p.Crp == registerNumber);

        if (excludeProfessionalId.HasValue)
            query = query.Where(p => p.Id != excludeProfessionalId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}
```

### **3. PsychologyRegisterService**

```csharp
public class PsychologyRegisterService : IRegisterService<Psychologist>
{
    private readonly IProfessionalRepository<Psychologist> _repository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IUserRepository _userRepository;
    private readonly IStringLocalizer<Messages> _localizer;

    public async Task<int> ExecuteAsync(
        Psychologist professional, 
        CancellationToken cancellationToken = default)
    {
        // 1. Valida unicidade dos dados
        await ValidateUniquenessAsync(professional, cancellationToken);

        // 2. Registra o profissional
        await _repository.AddAsync(professional, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return professional.Id;
    }

    private async Task ValidateUniquenessAsync(
        Psychologist professional, 
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        // PROFISSIONAL
        if (await _repository.CpfExistsAsync(professional.Cpf, null, cancellationToken))
            errors.Add(_localizer.For(ProfessionalMessage.CpfAlreadyExists).Value);

        if (await _repository.RegisterNumberExistsAsync(professional.Crp, null, cancellationToken))
            errors.Add(_localizer.For(ProfessionalMessage.RegisterNumberAlreadyExists).Value);

        // USUÁRIO
        if (professional.User != null)
        {
            if (await _userRepository.EmailExistsAsync(professional.User.Email, null, cancellationToken))
                errors.Add(_localizer.For(UserMessage.AlreadyExistsByEmail).Value);
        }

        // ORGANIZAÇÃO
        foreach (var orgProf in professional.Organizations ?? [])
        {
            var org = orgProf.Organization;
            if (org != null)
            {
                if (await _organizationRepository.ExistsByCnpjAsync(org.Cnpj, cancellationToken))
                    errors.Add(_localizer.For(OrganizationMessage.AlreadyExistsByCnpj, org.Cnpj).Value);

                if (await _organizationRepository.ExistsByRazaoSocialAsync(org.RazaoSocial, cancellationToken))
                    errors.Add(_localizer.For(OrganizationMessage.AlreadyExistsByRazaoSocial).Value);
            }
        }

        if (errors.Any())
            throw new ConflictException(string.Join("; ", errors));
    }
}
```

---

## ?? Fluxo de Validação Completo

```
1. REQUEST ? Controller
   POST /api/professional/register
   {
       "cpf": "123.456.789-00",
       "email": "joao@exemplo.com",
       "crp": "06/12345",
       "cnpj": "12.345.678/0001-95",
       "razaoSocial": "Clínica Exemplo"
   }

2. UseCase ? FluentValidation
   ? Valida formato de CPF
   ? Valida formato de email
   ? Valida máscara de telefone
   ? Valida formato de CNPJ
   ? Etc...

3. Service ? Validação de Unicidade
   ? CPF já existe? ? Consulta BD
   ? CRP já existe? ? Consulta BD
   ? Email já existe? ? Consulta BD
   ? CNPJ já existe? ? Consulta BD
   ? Razão Social já existe? ? Consulta BD

4. Resultado
   ? Tudo OK ? Registra no BD ? HTTP 201 Created
   ? Duplicado ? ConflictException ? HTTP 409 Conflict
```

---

## ?? Exemplo de Respostas

### **? Sucesso (HTTP 201 Created)**

```json
{
    "id": 1,
    "message": "Professional registered successfully"
}
```

### **? Erro - CPF Duplicado (HTTP 409 Conflict)**

```json
{
    "status": 409,
    "title": "Conflict",
    "message": "Já existe um profissional cadastrado com este CPF."
}
```

### **? Erro - Múltiplos Conflitos (HTTP 409 Conflict)**

```json
{
    "status": 409,
    "title": "Conflict",
    "message": "Já existe um profissional cadastrado com este CPF; Já existe um usuário cadastrado com este e-mail; Já existe uma organização com o CNPJ 12.345.678/0001-95."
}
```

---

## ?? Normalização de Dados

Antes de verificar a unicidade, os dados são **normalizados**:

| Campo | Normalização | Exemplo |
|-------|--------------|---------|
| **CPF** | Remove formatação | `123.456.789-00` ? `12345678900` |
| **Email** | ToLower() | `Joao@Exemplo.COM` ? `joao@exemplo.com` |
| **CRP** | Remove formatação | `06/12345` ? `0612345` |
| **CNPJ** | Remove formatação | `12.345.678/0001-95` ? `12345678000195` |
| **Razão Social** | ToTitleCase() | `CLÍNICA exemplo` ? `Clínica Exemplo` |

---

## ?? Mensagens Internacionalizadas

### **Português (pt-BR)**

```xml
<data name="ProfessionalMessage_CpfAlreadyExists">
  <value>Já existe um profissional cadastrado com este CPF.</value>
</data>
<data name="ProfessionalMessage_RegisterNumberAlreadyExists">
  <value>Já existe um profissional cadastrado com este número de registro.</value>
</data>
```

### **English (en-US)**

```xml
<data name="ProfessionalMessage_CpfAlreadyExists">
  <value>A professional with this CPF already exists.</value>
</data>
<data name="ProfessionalMessage_RegisterNumberAlreadyExists">
  <value>A professional with this registration number already exists.</value>
</data>
```

---

## ?? Testes Recomendados

### **Teste 1: CPF Duplicado**
```csharp
[Fact]
public async Task Should_Throw_ConflictException_When_Cpf_Already_Exists()
{
    // Arrange: Criar profissional com CPF "12345678900"
    var existing = CreateProfessional(cpf: "12345678900");
    await _repository.AddAsync(existing);
    await _repository.SaveChangesAsync();
    
    // Act: Tentar criar outro com mesmo CPF
    var newProfessional = CreateProfessional(cpf: "12345678900");
    
    // Assert
    await Assert.ThrowsAsync<ConflictException>(
        () => _service.ExecuteAsync(newProfessional));
}
```

### **Teste 2: Email Duplicado**
```csharp
[Fact]
public async Task Should_Throw_ConflictException_When_Email_Already_Exists()
{
    // Similar ao anterior, mas validando email
}
```

### **Teste 3: Múltiplos Conflitos**
```csharp
[Fact]
public async Task Should_Return_Multiple_Errors_When_Multiple_Conflicts()
{
    // CPF, Email e CNPJ duplicados
    var exception = await Assert.ThrowsAsync<ConflictException>(...);
    Assert.Contains("CPF", exception.Message);
    Assert.Contains("e-mail", exception.Message);
    Assert.Contains("CNPJ", exception.Message);
}
```

---

## ? Performance

### **Otimizações Implementadas:**

1. **Queries Assíncronas**: Todas as validações usam `async/await`
2. **Índices no Banco**: CPF, Email, CNPJ e Razão Social têm índices únicos
3. **AnyAsync**: Usa `AnyAsync()` ao invés de buscar registro completo
4. **Normalização**: Dados normalizados antes da consulta

### **Tempo Estimado por Validação:**
- CPF: ~5-10ms
- Email: ~5-10ms
- CRP: ~5-10ms
- CNPJ: ~5-10ms
- Razão Social: ~5-10ms

**Total: ~25-50ms** para todas as validações

---

## ?? Benefícios da Implementação

### **1. Integridade de Dados** ?
- Garante unicidade em nível de aplicação
- Complementa constraints do banco de dados
- Mensagens de erro claras para o usuário

### **2. Escalabilidade** ??
- Queries otimizadas com `AnyAsync()`
- Índices no banco de dados
- Validações em paralelo (future improvement)

### **3. Manutenibilidade** ??
- Lógica centralizada no Service
- Fácil adicionar novas validações
- Mensagens internacionalizadas

### **4. Experiência do Usuário** ??
- Mensagens claras e específicas
- Retorna todos os erros de uma vez
- Suporte a múltiplos idiomas

---

## ?? Próximos Passos

1. **? Implementado**: Validações de unicidade
2. **? Pendente**: Criar testes unitários
3. **? Pendente**: Adicionar logging das tentativas de duplicação
4. **? Pendente**: Implementar validações para Nutritionist
5. **? Pendente**: Adicionar métricas de performance

---

## ? Checklist de Implementação

- [x] ? Adicionar método `RegisterNumberExistsAsync` na interface
- [x] ? Implementar `CpfExistsAsync` no repository
- [x] ? Implementar `RegisterNumberExistsAsync` no repository
- [x] ? Criar método `ValidateUniquenessAsync` no service
- [x] ? Adicionar mensagens de erro nos resources
- [x] ? Traduzir mensagens PT e EN
- [x] ? Normalizar dados antes da validação
- [x] ? Lançar `ConflictException` com múltiplos erros
- [x] ? Build bem-sucedido
- [ ] ? Criar testes unitários
- [ ] ? Testar em ambiente de desenvolvimento

---

**?? Validações de unicidade implementadas com sucesso!**

**?? Sistema agora garante integridade dos dados em nível de aplicação!**
