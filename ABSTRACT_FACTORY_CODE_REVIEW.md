# ?? Code Review - Abstract Factory Pattern Implementation

## ? Status Geral

```
? Build bem-sucedido
? 0 erros de compilação
? Padrão Abstract Factory implementado corretamente
?? Alguns pontos de melhoria identificados
```

**Nota Geral:** 8.0/10 ????

---

## ?? Arquitetura Analisada

### Estrutura do Padrão
```
Domain/
??? Entities/
?   ??? Person (abstract) ?
?   ??? Psychologist : Person ?
?   ??? Nutritionist : Person ?
??? Interfaces/
?   ??? Factories/
?   ?   ??? IProfessionalModuleFactory ?
?   ??? Providers/
?       ??? IProfessionalFactoryProvider ?
??? Enumerators/
?   ??? ProfessionalType ?

Application/
??? Factories/
?   ??? PsychologyModuleFactory ?
??? Providers/
    ??? ProfessionalModuleFactoryProvider ?

Infra.IoC/
??? DependencyInjection ?
```

---

## ? Pontos Positivos

### 1. **Padrão Abstract Factory Bem Implementado**
? Interface abstrata (`IProfessionalModuleFactory`)
? Factories concretas (`PsychologyModuleFactory`)
? Provider para resolver factories (`ProfessionalModuleFactoryProvider`)
? Separação de responsabilidades clara

### 2. **Herança Correta**
```csharp
// ? BOM: Psychologist e Nutritionist herdam de Person
public class Psychologist : Person { }
public class Nutritionist : Person { }
```

### 3. **Validação no Domain**
```csharp
// ? BOM: Validações específicas nas classes derivadas
protected override void Validate()
{
    base.Validate(); // Chama validação da base
    var v = new ValidationCollector();
    v.When(Crp.IsNullOrWhiteSpace(), "O CRP é obrigatório.");
    // ...
}
```

### 4. **Person como Classe Abstrata**
```csharp
// ? EXCELENTE: Person agora é abstract
public abstract class Person : Entity
```
Isso impede instanciação direta e força uso das classes especializadas.

### 5. **Provider com Dependency Injection**
```csharp
// ? BOM: Provider recebe factories via DI
public ProfessionalModuleFactoryProvider(IEnumerable<IProfessionalModuleFactory> factories)
{
    _factories = factories.ToDictionary(f => GetTypeFromFactory(f));
}
```

---

## ?? Problemas Críticos

### 1. **? CRÍTICO: Assinatura Genérica do Factory**

**Problema:**
```csharp
// ? RUIM: Todos os parâmetros expostos
Person CreateProfessional(
    string name, 
    string cpf, 
    string? rg, 
    DateTime? birthDate, 
    PersonEnum.Gender gender, 
    string email, 
    string password, 
    string crp  // ? CRP só existe para Psychologist!
);
```

**Impacto:** 
- Interface genérica com parâmetro específico (CRP)
- Nutricionista vai receber CRP que não usa
- Viola princípios SOLID (Interface Segregation)

**Solução:**
```csharp
// ? MELHOR: Usar DTO
public interface IProfessionalModuleFactory
{
    Person CreateProfessional(CreateProfessionalDto dto);
    IAgendaService CreateAgendaService();
    IAppointmentService CreateAppointmentService();
}

// DTO específico para cada tipo
public record CreatePsychologistDto(
    string Name, string Cpf, string? Rg, DateTime? BirthDate,
    PersonEnum.Gender Gender, string Email, string Password,
    string Crp // ? Específico para psicólogo
);

public record CreateNutritionistDto(
    string Name, string Cpf, string? Rg, DateTime? BirthDate,
    PersonEnum.Gender Gender, string Email, string Password,
    string Crn // ? Específico para nutricionista
);
```

---

### 2. **?? CRÍTICO: Person Abstrata mas Usado no UserRepository**

**Problema encontrado:**
```csharp
// Na interface IUserRepository
public interface IUserRepository : IRepository<Person>
```

Se `Person` é abstrata, o repositório deveria ser genérico ou ter repositórios específicos.

**Recomendação:**
```csharp
// ? Opção 1: Repository genérico para qualquer Person
public interface IPersonRepository<T> : IRepository<T> where T : Person
{
    // Métodos comuns
}

// ? Opção 2: Repositories específicos
public interface IPsychologistRepository : IRepository<Psychologist> { }
public interface INutritionistRepository : IRepository<Nutritionist> { }
```

---

### 3. **?? IMPORTANTE: Provider com Pattern Matching Hardcoded**

**Problema:**
```csharp
// ?? PROBLEMA: Hardcoded type detection
private static ProfessionalType GetTypeFromFactory(IProfessionalModuleFactory factory)
{
    return factory switch
    {
        PsychologyModuleFactory => ProfessionalType.Psychologist,
        //NutritionModuleFactory => ProfessionalType.Nutritionist,
        _ => throw new ArgumentException("Tipo de módulo desconhecido.")
    };
}
```

**Impacto:**
- Adicionar nova factory = modificar Provider (viola Open/Closed)
- Acoplamento forte entre Provider e Factory concreta

**Solução 1 (Recomendada): Attribute-Based**
```csharp
// ? MELHOR: Usar atributo
[AttributeUsage(AttributeTargets.Class)]
public class ProfessionalTypeAttribute : Attribute
{
    public ProfessionalType Type { get; }
    
    public ProfessionalTypeAttribute(ProfessionalType type)
    {
        Type = type;
    }
}

// Uso nas factories
[ProfessionalType(ProfessionalType.Psychologist)]
public class PsychologyModuleFactory : IProfessionalModuleFactory { }

[ProfessionalType(ProfessionalType.Nutritionist)]
public class NutritionModuleFactory : IProfessionalModuleFactory { }

// Provider
private static ProfessionalType GetTypeFromFactory(IProfessionalModuleFactory factory)
{
    var attribute = factory.GetType()
        .GetCustomAttribute<ProfessionalTypeAttribute>();
        
    return attribute?.Type 
        ?? throw new ArgumentException("Factory sem atributo ProfessionalType");
}
```

**Solução 2: Property na Interface**
```csharp
// ? ALTERNATIVA: Property na interface
public interface IProfessionalModuleFactory
{
    ProfessionalType Type { get; }
    Person CreateProfessional(CreateProfessionalDto dto);
    // ...
}

// Implementação
public class PsychologyModuleFactory : IProfessionalModuleFactory
{
    public ProfessionalType Type => ProfessionalType.Psychologist;
    // ...
}
```

---

### 4. **?? IMPORTANTE: Factories Recebem Serviços Desnecessariamente**

**Problema:**
```csharp
// ?? PROBLEMA: Factory recebe serviços mas só os retorna
public class PsychologyModuleFactory : IProfessionalModuleFactory
{
    private readonly IAgendaService _agendaService;
    private readonly IAppointmentService _appointmentService;

    public PsychologyModuleFactory(
        IAgendaService agendaService, 
        IAppointmentService appointmentService)
    {
        _agendaService = agendaService;
        _appointmentService = appointmentService;
    }

    public IAgendaService CreateAgendaService() => _agendaService; // ? Só retorna
    public IAppointmentService CreateAppointmentService() => _appointmentService; // ? Só retorna
}
```

**Impacto:**
- Factory não "cria" os serviços, apenas os retorna
- Confunde o propósito do Factory (criar vs retornar)

**Solução:**
```csharp
// ? MELHOR: Factory realmente cria instâncias
public class PsychologyModuleFactory : IProfessionalModuleFactory
{
    private readonly IServiceProvider _serviceProvider;

    public PsychologyModuleFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAgendaService CreateAgendaService()
    {
        // Cria instância específica para psicologia
        return new PsychologyAgendaService(
            _serviceProvider.GetRequiredService<IAgendaRepository>());
    }

    public IAppointmentService CreateAppointmentService()
    {
        return new PsychologyAppointmentService(
            _serviceProvider.GetRequiredService<IAppointmentRepository>());
    }
}
```

**OU Simplificar:**
```csharp
// ? ALTERNATIVA: Remover do factory se não faz sentido
public interface IProfessionalModuleFactory
{
    Person CreateProfessional(CreateProfessionalDto dto);
    // Remover se não cria realmente
    // IAgendaService CreateAgendaService();
    // IAppointmentService CreateAppointmentService();
}
```

---

## ?? Problemas Médios

### 5. **?? Falta NutritionModuleFactory**

Você tem `Nutritionist` mas não tem `NutritionModuleFactory`:

```csharp
// ?? FALTA IMPLEMENTAR
public class NutritionModuleFactory : IProfessionalModuleFactory
{
    public Person CreateProfessional(CreateProfessionalDto dto)
    {
        // Assumindo DTO específico
        var nutritionistDto = dto as CreateNutritionistDto;
        return new Nutritionist(
            nutritionistDto.Name,
            nutritionistDto.Cpf,
            nutritionistDto.Rg,
            nutritionistDto.BirthDate,
            nutritionistDto.Gender,
            nutritionistDto.Email,
            nutritionistDto.Password,
            nutritionistDto.Crn
        );
    }

    // ... outros métodos
}
```

**Registrar no DI:**
```csharp
services.AddTransient<IProfessionalModuleFactory, NutritionModuleFactory>();
```

---

### 6. **?? Falta Documentação XML**

**Problema:**
Nenhuma das novas classes tem documentação XML.

**Solução:**
```csharp
/// <summary>
/// Entidade que representa um psicólogo no sistema.
/// Contém informações específicas como CRP (Conselho Regional de Psicologia).
/// </summary>
public class Psychologist : Person
{
    /// <summary>
    /// Número do CRP (Conselho Regional de Psicologia).
    /// </summary>
    public string Crp { get; private set; } = string.Empty;
    
    // ...
}

/// <summary>
/// Factory responsável por criar instâncias de profissionais de psicologia
/// e seus serviços relacionados.
/// </summary>
public class PsychologyModuleFactory : IProfessionalModuleFactory
{
    /// <summary>
    /// Cria um novo profissional psicólogo.
    /// </summary>
    /// <param name="dto">Dados para criação do psicólogo.</param>
    /// <returns>Instância de Psychologist.</returns>
    public Person CreateProfessional(CreateProfessionalDto dto)
    {
        // ...
    }
}
```

---

### 7. **?? Normalização de CRP/CRN**

**Sugestão:**
```csharp
// ? Adicionar normalização nos profissionais
public class Psychologist : Person
{
    // ...
    
    public void Update(/* ... */, string crp)
    {
        base.Update(/* ... */);
        Crp = crp;
        Normalize();
        Validate();
    }
    
    private void Normalize()
    {
        // Remove espaços e caracteres especiais
        Crp = Crp.GetOnlyNumbers(); // Se for só números
        // OU
        Crp = Crp.Trim().ToUpper(); // Se tiver letras
    }
}
```

---

## ?? Melhorias Sugeridas

### 8. **? Criar DTOs Específicos**

```csharp
// Domain/DTOs/CreateProfessionalDto.cs
public abstract record CreateProfessionalDto(
    string Name,
    string Cpf,
    string? Rg,
    DateTime? BirthDate,
    PersonEnum.Gender Gender,
    string Email,
    string Password
);

public record CreatePsychologistDto(
    string Name, string Cpf, string? Rg, DateTime? BirthDate,
    PersonEnum.Gender Gender, string Email, string Password,
    string Crp
) : CreateProfessionalDto(Name, Cpf, Rg, BirthDate, Gender, Email, Password);

public record CreateNutritionistDto(
    string Name, string Cpf, string? Rg, DateTime? BirthDate,
    PersonEnum.Gender Gender, string Email, string Password,
    string Crn
) : CreateProfessionalDto(Name, Cpf, Rg, BirthDate, Gender, Email, Password);
```

---

### 9. **? Factory com Validation**

```csharp
public class PsychologyModuleFactory : IProfessionalModuleFactory
{
    public Person CreateProfessional(CreateProfessionalDto dto)
    {
        if (dto is not CreatePsychologistDto psychologistDto)
            throw new ArgumentException("DTO inválido para PsychologyModuleFactory");
        
        // Validar CRP específico (ex: formato, região, etc)
        if (!ValidateCrp(psychologistDto.Crp))
            throw new DomainValidationException(new[] { "CRP inválido" });
        
        return new Psychologist(
            psychologistDto.Name,
            psychologistDto.Cpf,
            psychologistDto.Rg,
            psychologistDto.BirthDate,
            psychologistDto.Gender,
            psychologistDto.Email,
            psychologistDto.Password,
            psychologistDto.Crp
        );
    }
    
    private bool ValidateCrp(string crp)
    {
        // Lógica de validação de CRP
        return !string.IsNullOrWhiteSpace(crp) && crp.Length >= 5;
    }
}
```

---

### 10. **? Configurações do EF Core**

```csharp
// EntityTypeConfiguration para as novas entidades
public class PsychologistConfiguration : IEntityTypeConfiguration<Psychologist>
{
    public void Configure(EntityTypeBuilder<Psychologist> builder)
    {
        builder.Property(p => p.Crp)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("Número do CRP");
        
        builder.HasIndex(p => p.Crp)
            .IsUnique()
            .HasDatabaseName("IX_Psychologists_CRP");
    }
}

public class NutritionistConfiguration : IEntityTypeConfiguration<Nutritionist>
{
    public void Configure(EntityTypeBuilder<Nutritionist> builder)
    {
        builder.Property(n => n.Crn)
            .IsRequired()
            .HasMaxLength(20)
            .HasComment("Número do CRN");
        
        builder.HasIndex(n => n.Crn)
            .IsUnique()
            .HasDatabaseName("IX_Nutritionists_CRN");
    }
}
```

---

## ?? Comparação: Antes vs Depois

| Aspecto | Antes | Depois | Status |
|---------|-------|--------|--------|
| **Person** | Classe concreta | Classe abstrata | ? Melhorado |
| **Profissionais** | Não existiam | Psychologist, Nutritionist | ? Adicionado |
| **Factory Pattern** | Não tinha | Abstract Factory | ? Implementado |
| **Provider** | Não tinha | Factory Provider | ? Implementado |
| **Dependency Injection** | Parcial | Factories registradas | ? Melhorado |
| **DTOs Específicos** | Não tem | ? Faltando | ?? Pendente |
| **Documentação** | Person documentada | Novas classes sem docs | ?? Pendente |
| **Validações Específicas** | Não tinha | CRP/CRN validados | ? Implementado |

---

## ?? Checklist de Melhorias

### Prioridade Alta (Fazer Agora)
- [ ] Criar DTOs específicos (`CreatePsychologistDto`, `CreateNutritionistDto`)
- [ ] Implementar `NutritionModuleFactory`
- [ ] Corrigir assinatura do `IProfessionalModuleFactory` para usar DTOs
- [ ] Decidir: Factory cria ou retorna serviços?

### Prioridade Média (Próximas Iterações)
- [ ] Adicionar atributo `[ProfessionalType]` nas factories
- [ ] Documentação XML completa
- [ ] Normalização de CRP/CRN
- [ ] Validações específicas de CRP/CRN
- [ ] Entity Configurations para Psychologist e Nutritionist

### Prioridade Baixa (Futuro)
- [ ] Testes unitários das factories
- [ ] Testes de integração do provider
- [ ] Logs no provider

---

## ?? Exemplo de Uso Melhorado

### Como Seria Usado (Com Melhorias)
```csharp
// Controller ou Service
public class ProfessionalService
{
    private readonly IProfessionalFactoryProvider _factoryProvider;
    
    public async Task<Person> CreateProfessional(
        ProfessionalType type, 
        CreateProfessionalDto dto)
    {
        // Obter factory apropriada
        var factory = _factoryProvider.GetFactory(type);
        
        // Criar profissional com validações específicas
        var professional = factory.CreateProfessional(dto);
        
        // Salvar no repositório
        await _professionalRepository.AddAsync(professional);
        await _unitOfWork.SaveChangesAsync();
        
        return professional;
    }
}

// Uso
var psychologistDto = new CreatePsychologistDto(
    "Dr. João Silva", "12345678901", null, new DateTime(1980, 1, 1),
    PersonEnum.Gender.Male, "joao@clinica.com", "SecureP@ss123",
    "CRP-06/123456" // CRP específico
);

var psychologist = await service.CreateProfessional(
    ProfessionalType.Psychologist, 
    psychologistDto
);
```

---

## ? Conclusão

### Pontos Fortes
? Padrão Abstract Factory bem implementado
? Separação de responsabilidades clara
? Person corretamente abstraída
? Validações específicas funcionando
? Dependency Injection configurado

### Pontos a Melhorar
?? Interface genérica com parâmetro específico (CRP)
?? Provider com pattern matching hardcoded
?? Factories não "criam" serviços realmente
?? Falta `NutritionModuleFactory`
?? Falta documentação XML
?? Falta DTOs específicos

### Nota Final
**8.0/10** ????

Implementação sólida do padrão Abstract Factory! Os problemas identificados são todos facilmente corrigíveis e não impedem o funcionamento. Com as melhorias sugeridas, chegará a 10/10.

### Próximos Passos Recomendados
1. Criar DTOs específicos (1-2h)
2. Implementar NutritionModuleFactory (30min)
3. Adicionar atributo ProfessionalType (1h)
4. Documentar classes (1h)
5. Criar EntityTypeConfigurations (1h)

**Total estimado: 4-5 horas** para implementação completa das melhorias.

---

## ?? Referências

- **Design Patterns**: Gang of Four - Abstract Factory
- **Clean Architecture**: Robert C. Martin
- **Domain-Driven Design**: Eric Evans
- **.NET Best Practices**: Microsoft Documentation

Parabéns pela implementação! Continue assim! ??
