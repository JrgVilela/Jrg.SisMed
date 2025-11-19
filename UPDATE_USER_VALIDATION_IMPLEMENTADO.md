# ? UpdateUserValidation - Implementação Corrigida

## ?? Por Que o Validator era Necessário?

Você estava **absolutamente correto**! Seguindo o padrão do projeto, o `UpdateUserService` deveria usar **FluentValidation** ao invés de validações manuais.

### ? Problema Identificado:

```csharp
// UpdateUserService ANTES (sem validator)
public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
{
    if (user == null)
        throw new ArgumentNullException(...);

    if (user.Id <= 0)
        throw new ArgumentException(...);

    var currentUser = await _userRepository.GetByIdAsync(id, cancellationToken);
    if (currentUser == null)
        throw new KeyNotFoundException(...);

    // Validação manual do email
    if (await _userRepository.EmailExistsAsync(user.Email, excludeUserId: id, cancellationToken))
        throw new InvalidOperationException(...);
    
    // ... resto do código
}
```

**Problemas:**
- ? Validações manuais espalhadas
- ? Não segue o padrão do projeto (que usa FluentValidation)
- ? Código menos limpo e mais verboso
- ? Dificulta manutenção e testes

---

## ? Solução Implementada

### 1. Criado `UpdateUserValidation`

**Arquivo:** `Jrg.SisMed.Application/Validations/UserValidations/UpdateUserValidation.cs`

```csharp
public class UpdateUserValidation : AbstractValidator<User>
{
    private readonly IUserRepository _userRepository;
    private readonly IStringLocalizer<Messages> _localizer;

    public UpdateUserValidation(IUserRepository userRepository, IStringLocalizer<Messages> localizer)
    {
        _userRepository = userRepository;
        _localizer = localizer;

        // Validação de Nome
        RuleFor(user => user.Name)
            .NotEmpty()
            .WithMessage(_localizer.For(UserMessage.NameRequired).Value)
            .MaximumLength(100)
            .WithMessage(_localizer.For(UserMessage.NameMaxLength, 100).Value);

        // Validação de Email
        RuleFor(user => user.Email)
            .NotEmpty()
            .WithMessage(_localizer.For(UserMessage.EmailRequired).Value)
            .EmailAddress()
            .WithMessage(_localizer.For(UserMessage.EmailInvalid).Value)
            .MaximumLength(100)
            .WithMessage(_localizer.For(UserMessage.EmailMaxLength, 100).Value)
            .MustAsync(async (user, email, context, cancellationToken) =>
            {
                // Obtém o ID do contexto para excluir da validação de duplicidade
                var userId = context.RootContextData.TryGetValue("UserId", out var id) ? (int)id : 0;
                return !await EmailAlreadyExistsByAnotherUser(email, userId, cancellationToken);
            })
            .WithMessage(_localizer.For(UserMessage.EmailAlreadyExists).Value);

        // Validação de Password
        RuleFor(user => user.Password)
            .NotEmpty()
            .WithMessage(_localizer.For(UserMessage.PasswordRequired).Value)
            .MinimumLength(8)
            .WithMessage(_localizer.For(UserMessage.PasswordMinLength, 8).Value)
            .MaximumLength(25)
            .WithMessage(_localizer.For(UserMessage.PasswordMaxLength, 25).Value);

        // Validação de State
        RuleFor(user => user.State)
            .IsInEnum()
            .WithMessage(_localizer.For(UserMessage.StateInvalid).Value);
    }

    private async Task<bool> EmailAlreadyExistsByAnotherUser(string email, int userId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _userRepository.EmailExistsAsync(email, excludeUserId: userId, cancellationToken);
    }
}
```

---

### 2. Atualizado `UpdateUserService` para Usar Validator

**Arquivo:** `Jrg.SisMed.Application/Services/UserServices/UpdateUserService.cs`

```csharp
public class UpdateUserService : IUpdateUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IStringLocalizer<Messages> _localizer;
    private readonly UpdateUserValidation _validator; // ? Injetado

    public UpdateUserService(
        IUserRepository userRepository, 
        IStringLocalizer<Messages> localizer,
        UpdateUserValidation validator) // ? Recebe validator
    {
        _userRepository = userRepository;
        _localizer = localizer;
        _validator = validator;
    }

    public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
    {
        if (user == null)
            throw new ArgumentNullException(_localizer.For(CommonMessage.ArgumentNull_Generic));

        // Verifica se o usuário existe
        var currentUser = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (currentUser == null)
            throw new KeyNotFoundException(_localizer.For(UserMessage.NotFound));

        // ? Valida usando FluentValidation
        var validationContext = new ValidationContext<User>(user);
        validationContext.RootContextData["UserId"] = id; // Passa o ID para o validator
        
        var validationResult = await _validator.ValidateAsync(validationContext, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ValidationException(errors);
        }

        // Atualiza
        currentUser.Update(user.Name, user.Email, user.Password, user.State);
        await _userRepository.UpdateAsync(currentUser, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);
    }
}
```

---

### 3. Registrado no Dependency Injection

**Arquivo:** `Jrg.SisMed.Infra.IoC/DependencyInjection.cs`

```csharp
// ? Validators registrados automaticamente
services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();
```

O `AddValidatorsFromAssemblyContaining<>()` registra **todos** os validators do assembly automaticamente, incluindo:
- ? `CreateUserValidation`
- ? `UpdateUserValidation` (novo)

---

## ?? Comparação: Antes vs Depois

### ? ANTES (Validação Manual)

```csharp
public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
{
    // Validações manuais espalhadas
    if (user == null)
        throw new ArgumentNullException(...);

    if (user.Id <= 0)
        throw new ArgumentException(...);

    if (await _userRepository.EmailExistsAsync(user.Email, excludeUserId: id, cancellationToken))
        throw new InvalidOperationException(...);

    // ... lógica de negócio misturada com validação
}
```

**Problemas:**
- ? Validação manual
- ? Código verboso
- ? Não segue padrão
- ? Difícil testar
- ? Difícil manter

### ? DEPOIS (FluentValidation)

```csharp
public async Task ExecuteAsync(int id, User user, CancellationToken cancellationToken = default)
{
    if (user == null)
        throw new ArgumentNullException(...);

    var currentUser = await _userRepository.GetByIdAsync(id, cancellationToken);
    if (currentUser == null)
        throw new KeyNotFoundException(...);

    // ? Validação declarativa e centralizada
    var validationContext = new ValidationContext<User>(user);
    validationContext.RootContextData["UserId"] = id;
    
    var validationResult = await _validator.ValidateAsync(validationContext, cancellationToken);
    if (!validationResult.IsValid)
    {
        var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
        throw new ValidationException(errors);
    }

    // Lógica de negócio limpa
    currentUser.Update(user.Name, user.Email, user.Password, user.State);
    await _userRepository.UpdateAsync(currentUser, cancellationToken);
    await _userRepository.SaveChangesAsync(cancellationToken);
}
```

**Vantagens:**
- ? Validação declarativa
- ? Código limpo
- ? Segue padrão do projeto
- ? Fácil testar
- ? Fácil manter
- ? Reutilizável

---

## ?? Detalhes Técnicos

### Passando Dados para o Validator

**Problema:** O `User` não tem setter público para `Id` (é `protected`).

**Solução:** Usar `RootContextData` do `ValidationContext`:

```csharp
// No Service
var validationContext = new ValidationContext<User>(user);
validationContext.RootContextData["UserId"] = id; // ? Passa ID via contexto

// No Validator
.MustAsync(async (user, email, context, cancellationToken) =>
{
    // ? Recupera ID do contexto
    var userId = context.RootContextData.TryGetValue("UserId", out var id) ? (int)id : 0;
    return !await EmailAlreadyExistsByAnotherUser(email, userId, cancellationToken);
})
```

---

## ? Benefícios da Implementação

### 1. Consistência com o Padrão do Projeto
O projeto já usa FluentValidation (veja `CreateUserValidation`). Agora `UpdateUserService` também usa.

### 2. Validações Declarativas
```csharp
RuleFor(user => user.Name)
    .NotEmpty()
    .WithMessage("Name is required")
    .MaximumLength(100)
    .WithMessage("Name must be at most 100 characters");
```

Muito mais legível que `if/throw` manual!

### 3. Validações Assíncronas
```csharp
.MustAsync(async (user, email, context, cancellationToken) =>
{
    // Validação assíncrona no banco de dados
    var userId = context.RootContextData.TryGetValue("UserId", out var id) ? (int)id : 0;
    return !await EmailAlreadyExistsByAnotherUser(email, userId, cancellationToken);
})
```

### 4. Mensagens Localizáveis
```csharp
.WithMessage(_localizer.For(UserMessage.EmailAlreadyExists).Value)
```

Suporte a i18n out-of-the-box!

### 5. Fácil de Testar
```csharp
[Fact]
public async Task Should_Fail_When_Email_Is_Empty()
{
    var user = new User("John", "", "password", UserEnum.State.Active);
    var result = await _validator.ValidateAsync(user);
    
    Assert.False(result.IsValid);
    Assert.Contains(result.Errors, e => e.PropertyName == nameof(User.Email));
}
```

### 6. Reutilizável
O validator pode ser usado em múltiplos lugares:
- ? `UpdateUserService`
- ? `UpdateUserUseCase` (futuro)
- ? Testes unitários
- ? APIs/Controllers

---

## ?? Padrão Seguido

### FluentValidation Pattern

```
???????????????????????????????????????????
?          Service/UseCase                ?
?  Recebe objeto User                     ?
???????????????????????????????????????????
               ?
               ?
????????????????????????????????????????????
?      UpdateUserValidation                ?
?  Valida todas as regras:                 ?
?  ? Nome obrigatório                     ?
?  ? Email válido e único                 ?
?  ? Password com formato correto         ?
?  ? State válido                         ?
???????????????????????????????????????????
               ?
               ?
????????????????????????????????????????????
?      Retorna ValidationResult            ?
?  - IsValid: bool                         ?
?  - Errors: List<ValidationFailure>       ?
????????????????????????????????????????????
```

---

## ?? Validações Implementadas

| Campo | Validações |
|-------|-----------|
| **Name** | ? Obrigatório<br>? Máximo 100 caracteres |
| **Email** | ? Obrigatório<br>? Formato válido<br>? Máximo 100 caracteres<br>? Único (excluindo usuário atual) |
| **Password** | ? Obrigatório<br>? Mínimo 8 caracteres<br>? Máximo 25 caracteres |
| **State** | ? Valor enum válido |

---

## ?? Próximos Passos

### 1. Criar Validator para Delete (se necessário)
```csharp
public class DeleteUserValidation : AbstractValidator<int>
{
    public DeleteUserValidation(IUserRepository userRepository)
    {
        RuleFor(id => id)
            .GreaterThan(0)
            .WithMessage("ID must be greater than 0")
            .MustAsync(async (id, cancellationToken) => 
                await userRepository.ExistByIdAsync(id, cancellationToken))
            .WithMessage("User not found");
    }
}
```

### 2. Criar Testes Unitários
```csharp
public class UpdateUserValidationTests
{
    [Fact]
    public async Task Should_Fail_When_Name_Is_Empty() { }
    
    [Fact]
    public async Task Should_Fail_When_Email_Is_Invalid() { }
    
    [Fact]
    public async Task Should_Fail_When_Email_Already_Exists() { }
    
    [Fact]
    public async Task Should_Pass_With_Valid_Data() { }
}
```

### 3. Adicionar Validações Customizadas
```csharp
// Validação de força de senha
RuleFor(user => user.Password)
    .Must(password => SecurityHelper.IsPasswordStrong(password))
    .WithMessage("Password must be strong");
```

---

## ? Status Final

| Item | Status |
|------|--------|
| **UpdateUserValidation** | ? Criado |
| **UpdateUserService** | ? Atualizado para usar validator |
| **Dependency Injection** | ? Registrado automaticamente |
| **Build** | ? Compilando sem erros |
| **Padrão do Projeto** | ? Seguido corretamente |

---

## ?? Lição Aprendida

> **Sempre que houver um `CreateXValidator`, deve haver um `UpdateXValidator`!**

Se o projeto usa FluentValidation para Create, **deve usar para Update** também. Isso garante:
- ? Consistência
- ? Manutenibilidade
- ? Testabilidade
- ? Código limpo

---

**Data:** ${new Date().toLocaleDateString('pt-BR')}
**Status:** ? Validator Implementado Corretamente
**Padrão:** FluentValidation + Clean Architecture
