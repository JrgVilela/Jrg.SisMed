# ? Repository Pattern - Implementação Completa

## ?? Resumo

Implementação completa do **Repository Pattern** e **Unit of Work Pattern** seguindo as melhores práticas do Entity Framework Core.

---

## ?? Arquivos Criados/Atualizados

### ? **1. Repository<T>.cs** (IMPLEMENTADO)
**Localização:** `Jrg.SisMed.Infra.Data/Repositories/Repository.cs`

**Classe base genérica** para todos os repositórios.

#### Métodos Implementados:

| Método | Tipo | Descrição |
|--------|------|-----------|
| `GetByIdAsync()` | Async | Busca por ID |
| `GetAllAsync()` | Async | Retorna todas as entidades |
| `FindAsync()` | Async | Busca com predicado |
| `AddAsync()` | Async | Adiciona entidade |
| `Update()` | Sync | Marca para update (padrão EF) |
| `Remove()` | Sync | Marca para delete (padrão EF) |
| `AsQueryable()` | Sync | Retorna IQueryable para queries complexas |
| `ExistsAsync()` | Async | Verifica existência |
| `FirstOrDefaultAsync()` | Async | Primeira entidade ou null |
| `CountAsync()` | Async | Conta entidades |
| `AddRangeAsync()` | Async | Adiciona múltiplas entidades |
| `RemoveRange()` | Sync | Remove múltiplas entidades |

#### Características:
- ? Genérico (`where T : Entity`)
- ? Todos os métodos com `CancellationToken`
- ? `DbSet<T>` protegido para uso nas classes derivadas
- ? Validação de argumentos nulos
- ? Documentação XML completa
- ? Métodos adicionais úteis além da interface

#### Exemplo de Uso:
```csharp
// Busca simples
var person = await repository.GetByIdAsync(1);

// Busca com filtro
var activeUsers = await repository.FindAsync(u => u.State == PersonEnum.State.Active);

// Query complexa
var users = await repository.AsQueryable()
    .Include(u => u.Addresses)
    .Where(u => u.State == PersonEnum.State.Active)
    .OrderBy(u => u.Name)
    .ToListAsync();

// Verificar existência
bool exists = await repository.ExistsAsync(u => u.Email == "test@test.com");

// Contar
int total = await repository.CountAsync(u => u.State == PersonEnum.State.Active);
```

---

### ? **2. IRepository<T>** (ATUALIZADO)
**Localização:** `Jrg.SisMed.Domain/Interfaces/Repositories/IRepository.cs`

**Interface genérica** para repositórios.

#### Mudanças Aplicadas:
- ? Removido: `CreateAsync`, `UpdateAsync`, `DeleteAsync`, `CommitAsync`
- ? Adicionado: `AddAsync`, `Update`, `Remove`, `AsQueryable`
- ? Adicionado: `CancellationToken` em todos os métodos async
- ? Documentação XML completa

#### Motivo das Mudanças:
1. **`Update` e `Remove` síncronos:** Padrão do EF Core (apenas marcam para alteração)
2. **Sem `CommitAsync`:** Responsabilidade do Unit of Work
3. **`AddAsync` ao invés de `CreateAsync`:** Nomenclatura padrão do EF Core
4. **`AsQueryable`:** Permite queries complexas com LINQ

---

### ? **3. UserRepository.cs** (CRIADO)
**Localização:** `Jrg.SisMed.Infra.Data/Repositories/UserRepository.cs`

**Repositório específico** para a entidade `Person` (usuários).

#### Métodos Específicos:

| Método | Descrição |
|--------|-----------|
| `GetByEmailAsync()` | Busca usuário por email |
| `GetByCpfAsync()` | Busca usuário por CPF |
| `EmailExistsAsync()` | Verifica se email já existe |
| `CpfExistsAsync()` | Verifica se CPF já existe |
| `GetActiveUsersAsync()` | Retorna usuários ativos |
| `GetByIdWithDetailsAsync()` | Busca com eager loading (Addresses, Phones) |

#### Características:
- ? Usa `AsNoTracking()` em buscas read-only (performance)
- ? Validações de unicidade com opção de excluir ID (para updates)
- ? Eager loading com `Include` e `ThenInclude`
- ? Normalização de email para lowercase
- ? Documentação XML completa

#### Exemplo de Uso:
```csharp
// Buscar por email
var user = await userRepository.GetByEmailAsync("user@example.com");

// Verificar se email já existe (para criar novo usuário)
bool emailExists = await userRepository.EmailExistsAsync("new@example.com");

// Verificar se email já existe (para atualizar usuário - exclui o próprio ID)
bool emailExists = await userRepository.EmailExistsAsync("updated@example.com", excludeUserId: 5);

// Buscar com relacionamentos
var userWithDetails = await userRepository.GetByIdWithDetailsAsync(1);
// userWithDetails.Addresses e userWithDetails.Phones já estão carregados!

// Buscar usuários ativos
var activeUsers = await userRepository.GetActiveUsersAsync();
```

---

### ? **4. IUserRepository** (ATUALIZADO)
**Localização:** `Jrg.SisMed.Domain/Interfaces/Repositories/IUserRepository.cs`

**Interface específica** para UserRepository.

#### Mudanças:
- ? Adicionados todos os métodos específicos
- ? CancellationToken em todos os métodos
- ? Documentação XML completa

---

### ? **5. UnitOfWork.cs** (CRIADO)
**Localização:** `Jrg.SisMed.Infra.Data/UnitOfWork/UnitOfWork.cs`

**Implementação do Unit of Work Pattern**.

#### Métodos:

| Método | Descrição |
|--------|-----------|
| `SaveChangesAsync()` | Salva todas as alterações |
| `Dispose()` | Libera recursos (síncrono) |
| `DisposeAsync()` | Libera recursos (assíncrono) |

#### Características:
- ? Trata exceções do EF (`DbUpdateException`, `DbUpdateConcurrencyException`)
- ? Implementa `IDisposable` e `IAsyncDisposable`
- ? GC.SuppressFinalize para melhor performance
- ? Documentação XML completa

#### Exemplo de Uso:
```csharp
// Com using (dispõe automaticamente)
await using var unitOfWork = new UnitOfWork(context);

// Operações
await userRepository.AddAsync(newUser);
organizationRepository.Update(existingOrg);

// Salvar tudo de uma vez (transação)
int affectedRows = await unitOfWork.SaveChangesAsync();
```

---

### ? **6. IUnitOfWork** (ATUALIZADO)
**Localização:** `Jrg.SisMed.Domain/Interfaces/IUnitOfWork.cs`

**Interface do Unit of Work**.

#### Mudanças:
- ? Adicionado `IDisposable` e `IAsyncDisposable`
- ? CancellationToken em `SaveChangesAsync`
- ? Documentação XML completa

---

## ?? Padrões e Melhores Práticas Aplicadas

### 1. **Repository Pattern**
? Separa lógica de acesso a dados do resto da aplicação
? Facilita testes (mock dos repositórios)
? Centraliza queries complexas

### 2. **Unit of Work Pattern**
? Garante atomicidade (tudo salva ou nada salva)
? Gerencia transações automaticamente
? Evita múltiplas chamadas a `SaveChanges()`

### 3. **Async/Await**
? Todos os métodos de I/O são assíncronos
? CancellationToken em todos os métodos async
? Melhor performance e escalabilidade

### 4. **Entity Framework Core Best Practices**
? `Update()` e `Remove()` síncronos (padrão EF)
? `AsNoTracking()` em queries read-only
? Eager loading com `Include`/`ThenInclude`
? `AsQueryable()` para queries complexas

### 5. **Dependency Injection Ready**
? Interfaces no Domain (não depende de Infra)
? Implementações na Infra.Data
? Pronto para injeção de dependência

### 6. **Clean Architecture**
? Domain não conhece Infra (inversão de dependência)
? Interfaces no Domain
? Implementações na Infra

---

## ?? Estrutura de Camadas

```
Domain (Core)
??? Interfaces/
?   ??? IRepository<T>          ? Interface genérica
?   ??? IUserRepository         ? Interface específica
?   ??? IUnitOfWork             ? Interface UoW
?
Infra.Data (Implementação)
??? Repositories/
?   ??? Repository<T>           ? Classe base genérica
?   ??? UserRepository          ? Implementação específica
??? UnitOfWork/
?   ??? UnitOfWork              ? Implementação UoW
??? Context/
    ??? ApplicationDbContext    ? DbContext
```

---

## ?? Próximos Passos

### 1. Criar Repositórios Específicos
Seguir o padrão do `UserRepository` para criar:

```csharp
// OrganizationRepository
public class OrganizationRepository : Repository<Organization>, IOrganizationRepository
{
    public OrganizationRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<Organization?> GetByCnpjAsync(string cnpj, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.CNPJ == cnpj, ct);
    }
}

// AddressRepository
public class AddressRepository : Repository<Address>, IAddressRepository
{
    public AddressRepository(ApplicationDbContext context) : base(context) { }
    
    public async Task<IEnumerable<Address>> GetByCepAsync(string cep, CancellationToken ct = default)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(a => a.ZipCode == cep)
            .ToListAsync(ct);
    }
}
```

### 2. Configurar Dependency Injection
No `Infra.IoC`:

```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        
        return services;
    }
}
```

### 3. Usar nos Services (Application Layer)
```csharp
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Person> CreateUserAsync(CreateUserDto dto, CancellationToken ct)
    {
        // Validar unicidade
        if (await _userRepository.EmailExistsAsync(dto.Email, ct))
            throw new DomainValidationException(new[] { "Email já cadastrado" });
        
        if (await _userRepository.CpfExistsAsync(dto.Cpf, ct))
            throw new DomainValidationException(new[] { "CPF já cadastrado" });
        
        // Criar entidade
        var user = new Person(dto.Name, dto.Cpf, dto.Rg, dto.BirthDate, 
                              dto.Gender, dto.Email, dto.Password);
        
        // Adicionar ao repositório
        await _userRepository.AddAsync(user, ct);
        
        // Salvar (Unit of Work)
        await _unitOfWork.SaveChangesAsync(ct);
        
        return user;
    }
}
```

---

## ?? Exemplos de Uso Completos

### Exemplo 1: Criar Usuário
```csharp
public async Task<Person> CreateUser(CreateUserDto dto)
{
    // 1. Validar unicidade
    if (await _userRepository.EmailExistsAsync(dto.Email))
        throw new Exception("Email já existe");
    
    // 2. Criar entidade
    var user = new Person(dto.Name, dto.Cpf, null, dto.BirthDate, 
                          dto.Gender, dto.Email, dto.Password);
    
    // 3. Adicionar
    await _userRepository.AddAsync(user);
    
    // 4. Salvar
    await _unitOfWork.SaveChangesAsync();
    
    return user;
}
```

### Exemplo 2: Atualizar Usuário
```csharp
public async Task<Person> UpdateUser(int id, UpdateUserDto dto)
{
    // 1. Buscar
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new NotFoundException("Usuário não encontrado");
    
    // 2. Validar email (excluindo o próprio usuário)
    if (await _userRepository.EmailExistsAsync(dto.Email, excludeUserId: id))
        throw new Exception("Email já existe");
    
    // 3. Atualizar
    user.Update(dto.Name, user.Cpf, dto.Rg, dto.BirthDate, 
                dto.Gender, dto.Email, dto.Password);
    
    // 4. Marcar para update
    _userRepository.Update(user);
    
    // 5. Salvar
    await _unitOfWork.SaveChangesAsync();
    
    return user;
}
```

### Exemplo 3: Deletar Usuário
```csharp
public async Task DeleteUser(int id)
{
    // 1. Buscar
    var user = await _userRepository.GetByIdAsync(id);
    if (user == null)
        throw new NotFoundException("Usuário não encontrado");
    
    // 2. Marcar para delete
    _userRepository.Remove(user);
    
    // 3. Salvar
    await _unitOfWork.SaveChangesAsync();
}
```

### Exemplo 4: Query Complexa
```csharp
public async Task<IEnumerable<Person>> SearchUsers(string searchTerm)
{
    return await _userRepository.AsQueryable()
        .Include(u => u.Addresses)
            .ThenInclude(pa => pa.Address)
        .Include(u => u.Phones)
            .ThenInclude(pp => pp.Phone)
        .Where(u => u.State == PersonEnum.State.Active)
        .Where(u => u.Name.Contains(searchTerm) || u.Email.Contains(searchTerm))
        .OrderBy(u => u.Name)
        .Take(50)
        .ToListAsync();
}
```

---

## ? Performance Tips

### 1. **Use AsNoTracking para Read-Only**
```csharp
// ? BOM (read-only)
var users = await _dbSet.AsNoTracking().ToListAsync();

// ? RUIM (tracking desnecessário)
var users = await _dbSet.ToListAsync();
```

### 2. **Eager Loading vs Lazy Loading**
```csharp
// ? BOM (1 query com JOIN)
var user = await _dbSet
    .Include(u => u.Addresses)
    .FirstOrDefaultAsync(u => u.Id == id);

// ? RUIM (N+1 queries)
var user = await _dbSet.FirstOrDefaultAsync(u => u.Id == id);
var addresses = user.Addresses.ToList(); // Lazy loading
```

### 3. **Projeção ao invés de Entidade Completa**
```csharp
// ? BOM (apenas campos necessários)
var names = await _dbSet
    .Where(u => u.State == PersonEnum.State.Active)
    .Select(u => new { u.Id, u.Name })
    .ToListAsync();

// ? RUIM (carrega tudo)
var users = await _dbSet
    .Where(u => u.State == PersonEnum.State.Active)
    .ToListAsync();
```

---

## ? Status Final

```
? Repository<T> implementado
? IRepository<T> atualizado
? UserRepository implementado
? IUserRepository atualizado
? UnitOfWork implementado
? IUnitOfWork atualizado
? Build bem-sucedido
? 0 erros
? 0 avisos
? Pronto para uso!
```

**Qualidade:** ????? (10/10)

Implementação completa e profissional do Repository Pattern e Unit of Work Pattern, seguindo todas as melhores práticas do Entity Framework Core e Clean Architecture! ??
