# ?? Code Review - Jrg.SisMed.Domain

## ? Pontos Positivos

### 1. **Arquitetura e Organização**
- ? Separação clara entre Entities, Interfaces e Exceptions
- ? Uso correto de Domain-Driven Design (DDD)
- ? Encapsulamento adequado com setters `private`
- ? Validações no domínio (onde devem estar)
- ? Rich Domain Model (entidades com comportamento)
- ? Uso de CancellationToken nos repositórios
- ? Pattern Unit of Work implementado
- ? Repository Pattern bem estruturado

### 2. **Validações**
- ? ValidationCollector é uma ótima solução para acumular erros
- ? DomainValidationException bem estruturada
- ? Validações centralizadas nas entidades
- ? Normalização de dados antes da validação

### 3. **Helpers**
- ? StringHelper, FormatHelper e ValidateHelper bem implementados
- ? SecurityHelper com PBKDF2 para senhas (seguro!)

---

## ?? Problemas Críticos a Corrigir

### 1. **?? CRÍTICO: Validação de Enum Invertida**

**Arquivo:** `Person.cs` (linhas 106-108)

```csharp
// ? ERRADO - Lógica invertida!
v.When(Enum.IsDefined(typeof(PersonEnum.Gender), Gender), "O gênero informado é inválido.");
v.When(Enum.IsDefined(typeof(PersonEnum.State), State), "O status da pessoa é inválido.");
```

**Problema:** `IsDefined` retorna `true` quando o valor É VÁLIDO, mas você está dizendo "quando for válido, lance erro".

**Correção:**
```csharp
// ? CORRETO
v.When(!Enum.IsDefined(typeof(PersonEnum.Gender), Gender), "O gênero informado é inválido.");
v.When(!Enum.IsDefined(typeof(PersonEnum.State), State), "O status da pessoa é inválido.");
```

---

### 2. **?? CRÍTICO: Hash de Senha no Momento Errado**

**Arquivo:** `Person.cs` (linha 48)

```csharp
// ? PROBLEMA: Hash é feito ANTES da validação
public void Update(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string passwordHash)
{
    Validate(); // Valida ANTES de atribuir
    Name = name;
    Cpf = cpf.FormatCpf();
    Rg = rg;
    BirthDate = birthDate;
    Gender = gender;
    Email = email;
    PasswordHash = SecurityHelper.HashPasswordPbkdf2(passwordHash); // Hash é feito aqui
}

private void Validate()
{
    // ? Aqui valida PasswordHash que ainda é vazio ou o valor antigo!
    v.When(PasswordHash.IsNullOrWhiteSpace(), "A senha é obrigatória.");
    v.When(PasswordHash.Length < MinPasswordHashLength || PasswordHash.Length > MaxPasswordHashLength, 
        $"A senha deve conter entre {MinPasswordHashLength} e {MaxPasswordHashLength} caracteres.");
}
```

**Problema:** 
1. Valida `PasswordHash` ANTES de atribuir o novo valor
2. Valida comprimento do HASH, não da senha original
3. Hash PBKDF2 tem ~88 caracteres, sempre vai falhar na validação de comprimento

**Correção:** Validar senha ANTES de fazer hash

---

### 3. **?? CRÍTICO: Parâmetro com Nome Enganoso**

**Arquivo:** `Person.cs` (linha 42)

```csharp
// ? Nome enganoso - parece que já é um hash, mas é a senha em texto plano
public Person(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string passwordHash)
{
    Update(name, cpf, rg, birthDate, gender, email, passwordHash);
}
```

**Correção:**
```csharp
// ? Nome claro - indica que é senha em texto plano
public Person(string name, string cpf, string? rg, DateTime? birthDate, PersonEnum.Gender gender, string email, string password)
```

---

### 4. **?? IMPORTANTE: EntityBase sem Lógica de Auditoria**

**Arquivo:** `EntityBase.cs`

```csharp
public class EntityBase : Entity
{
    public virtual Person CreatedBy { get; protected set; }
    public virtual Person? UpdatedBy { get; protected set; }
}
```

**Problemas:**
1. Não há método para definir CreatedBy/UpdatedBy
2. Entity tem CreatedAt/UpdatedAt mas não são setados automaticamente
3. Falta lógica de auditoria

---

### 5. **?? Validação Chamada no Local Errado**

**Arquivo:** `Address.cs` (linha 26)

```csharp
// ? Valida ANTES de ter valores
public Address(string street, string number, string? complement, string zipCode, string city, string state)
{
    Validate(); // ? Validando antes de atribuir valores!

    Update(street, number, complement, zipCode, city, state);
}
```

---

## ?? Sugestões de Melhorias

### 1. **Value Objects para Tipos Específicos**

Em vez de strings simples, use Value Objects para tipos que têm regras específicas:

```csharp
// ? Sugestão
public class Cpf
{
    public string Value { get; }
    
    private Cpf(string value)
    {
        Value = value;
    }
    
    public static Cpf Create(string value)
    {
        if (!value.IsCpf())
            throw new DomainValidationException(new[] { "CPF inválido" });
            
        return new Cpf(value.GetOnlyNumbers());
    }
    
    public override string ToString() => Value.FormatCpf();
}

// Uso na entidade Person
public Cpf Cpf { get; private set; }
```

**Benefícios:**
- Impossível ter CPF inválido
- Formatação encapsulada
- Reutilizável em outras entidades

### 2. **Domain Events para Mudanças Importantes**

```csharp
// ? Sugestão
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; protected set; }
    
    protected DomainEvent()
    {
        OccurredOn = DateTime.UtcNow;
    }
}

public class PersonCreatedEvent : DomainEvent
{
    public int PersonId { get; }
    public string Email { get; }
    
    public PersonCreatedEvent(int personId, string email)
    {
        PersonId = personId;
        Email = email;
    }
}

// Na entidade
public class Entity
{
    private readonly List<DomainEvent> _domainEvents = new();
    public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;
    
    protected void AddDomainEvent(DomainEvent @event)
    {
        _domainEvents.Add(@event);
    }
    
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}

// Uso
public Person(...)
{
    // ... construção
    AddDomainEvent(new PersonCreatedEvent(Id, Email));
}
```

### 3. **Specification Pattern para Consultas Complexas**

```csharp
// ? Sugestão
public abstract class Specification<T>
{
    public abstract Expression<Func<T, bool>> ToExpression();
    
    public bool IsSatisfiedBy(T entity)
    {
        var predicate = ToExpression().Compile();
        return predicate(entity);
    }
}

public class ActivePersonsSpecification : Specification<Person>
{
    public override Expression<Func<Person, bool>> ToExpression()
    {
        return person => person.State == PersonEnum.State.Active;
    }
}

public class PersonByEmailSpecification : Specification<Person>
{
    private readonly string _email;
    
    public PersonByEmailSpecification(string email)
    {
        _email = email;
    }
    
    public override Expression<Func<Person, bool>> ToExpression()
    {
        return person => person.Email == _email;
    }
}
```

### 4. **Separar Enums do Entity**

```csharp
// ? Atualmente
public class PersonEnum
{
    public enum State { ... }
    public enum Gender { ... }
}

// ? Melhor
namespace Jrg.SisMed.Domain.Enums
{
    public enum PersonState
    {
        Active = 1,
        Inactive = 2
    }
    
    public enum Gender
    {
        None = 0,
        Male = 1,
        Female = 2,
        Other = 3
    }
}
```

### 5. **Melhorar EntityBase com Métodos de Auditoria**

```csharp
// ? Sugestão
public abstract class EntityBase : Entity
{
    public virtual int? CreatedById { get; protected set; }
    public virtual Person? CreatedBy { get; protected set; }
    
    public virtual int? UpdatedById { get; protected set; }
    public virtual Person? UpdatedBy { get; protected set; }
    
    public void SetCreatedBy(Person person)
    {
        if (person == null)
            throw new ArgumentNullException(nameof(person));
            
        CreatedById = person.Id;
        CreatedBy = person;
        CreatedAt = DateTime.UtcNow;
    }
    
    public void SetUpdatedBy(Person person)
    {
        if (person == null)
            throw new ArgumentNullException(nameof(person));
            
        UpdatedById = person.Id;
        UpdatedBy = person;
        UpdatedAt = DateTime.UtcNow;
    }
}
```

### 6. **Readonly para Constantes**

```csharp
// ? Atualmente em Address.cs
private readonly int MaxStreetLength = 200;

// ? Melhor
private const int MaxStreetLength = 200;
```

**Motivo:** `const` é resolvido em tempo de compilação e tem melhor performance.

### 7. **Adicionar Método Factory em Person**

```csharp
// ? Sugestão
public static class PersonFactory
{
    public static Person CreatePatient(string name, string cpf, string email, string password)
    {
        // Lógica específica para criar paciente
        var person = new Person(name, cpf, null, null, Gender.None, email, password);
        // Adicionar role/tipo específico
        return person;
    }
    
    public static Person CreateDoctor(string name, string cpf, string email, string password, string crm)
    {
        // Lógica específica para criar médico
        var person = new Person(name, cpf, null, null, Gender.None, email, password);
        // Adicionar CRM, role, etc
        return person;
    }
}
```

### 8. **Melhorar IRepository com Paginação**

```csharp
// ? Sugestão
public interface IRepository<T>
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PagedResult<T>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    IQueryable<T> AsQueryable();
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; }
    public int TotalCount { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    
    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items.ToList();
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
}
```

### 9. **Adicionar Interface para Auditável**

```csharp
// ? Sugestão
public interface IAuditable
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    int? CreatedById { get; }
    int? UpdatedById { get; }
}

public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
    int? DeletedById { get; }
    void Delete(Person deletedBy);
    void Restore();
}
```

### 10. **Usar Records para DTOs no Domain**

```csharp
// ? Sugestão para criar DTOs imutáveis
namespace Jrg.SisMed.Domain.DTOs
{
    public record PersonSummary(
        int Id,
        string Name,
        string Email,
        PersonState State
    );
    
    public record CreatePersonCommand(
        string Name,
        string Cpf,
        string Email,
        string Password
    );
}
```

---

## ?? Resumo de Prioridades

### ?? Corrigir IMEDIATAMENTE
1. ? Validação de Enum invertida em Person.cs
2. ? Lógica de validação de senha (validar antes de hash)
3. ? Renomear parâmetro `passwordHash` para `password`
4. ? Remover Validate() do construtor de Address

### ?? Corrigir em Breve
5. Adicionar Value Objects (Cpf, Email, Phone)
6. Separar enums em arquivo próprio
7. Adicionar Domain Events
8. Melhorar EntityBase com auditoria

### ?? Melhorias Futuras
9. Specification Pattern
10. Factory Pattern
11. Paginação no Repository
12. Soft Delete
13. Records para DTOs

---

## ?? Referências e Boas Práticas Aplicadas

### ? Você já está seguindo:
- **SOLID Principles**
- **Domain-Driven Design (DDD)**
- **Clean Architecture**
- **Repository Pattern**
- **Unit of Work Pattern**
- **Encapsulation**
- **Validation in Domain**

### ?? Recomendações de Leitura:
- "Domain-Driven Design" - Eric Evans
- "Implementing Domain-Driven Design" - Vaughn Vernon
- "Clean Architecture" - Robert C. Martin
- "Patterns of Enterprise Application Architecture" - Martin Fowler

---

## ?? Conclusão

Seu Domain está **muito bem estruturado** para um início de projeto! Os problemas encontrados são facilmente corrigíveis e as melhorias sugeridas são incrementais.

**Nota Geral: 8.5/10** ?

**Pontos Fortes:**
- Arquitetura limpa e bem organizada
- Validações robustas
- Encapsulamento adequado
- Uso de patterns modernos

**Próximos Passos:**
1. Corrigir os 4 problemas críticos (1h)
2. Implementar Value Objects para Cpf e Email (2h)
3. Adicionar Domain Events (3h)
4. Melhorar testes unitários das entidades (4h)

Continue com esse padrão de qualidade! ??
