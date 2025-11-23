# ?? Integração FluentValidation no RegisterProfessionalUseCase

## ?? O que foi implementado

A validação `RegisterProfessionalValidation` foi integrada ao `RegisterProfessionalUseCase` através do **FluentValidation**, garantindo que todos os dados sejam validados **antes** de serem processados.

---

## ?? Arquitetura Implementada

```
???????????????????????????????????????????????????????????????
?                      API LAYER                               ?
?  ProfessionalController                                      ?
?  ??> POST /api/professional/register                        ?
???????????????????????????????????????????????????????????????
                       ?
                       ?
???????????????????????????????????????????????????????????????
?                  APPLICATION LAYER                           ?
?  ?????????????????????????????????????????????????????????  ?
?  ?  RegisterProfessionalUseCase                          ?  ?
?  ?  1. Valida DTO (FluentValidation)                    ?  ?
?  ?  2. Converte DTO ? Domain Entity                     ?  ?
?  ?  3. Delega para Factory                              ?  ?
?  ?????????????????????????????????????????????????????????  ?
?                       ?                                      ?
?                       ?                                      ?
?  ?????????????????????????????????????????????????????????  ?
?  ?  RegisterProfessionalValidation                       ?  ?
?  ?  - Valida Professional                                ?  ?
?  ?  - Valida User                                        ?  ?
?  ?  - Valida Address                                     ?  ?
?  ?  - Valida Phone                                       ?  ?
?  ?  - Valida Organization                                ?  ?
?  ?????????????????????????????????????????????????????????  ?
???????????????????????????????????????????????????????????????
```

---

## ?? Código Implementado

### **RegisterProfessionalUseCase.cs**

```csharp
public class RegisterProfessionalUseCase
{
    private readonly IProfessionalFactoryProvider _provider;
    private readonly IValidator<RegisterDto> _validator;

    public RegisterProfessionalUseCase(
        IProfessionalFactoryProvider provider,
        IValidator<RegisterDto> validator)
    {
        _provider = provider;
        _validator = validator;
    }

    public async Task<int> ExecuteAsync(RegisterDto dto)
    {
        // 1. VALIDA DTO
        var validationResult = await _validator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // 2. CONVERTE DTO ? DOMAIN
        var professional = dto.ToDomain();

        // 3. DELEGA PARA FACTORY
        dynamic factory = _provider.GetFactory(dto.ProfessionalType);
        dynamic service = factory.CreateRegister();

        // 4. EXECUTA REGISTRO
        return await service.ExecuteAsync(professional);
    }
}
```

---

## ?? Injeção de Dependência

### **Registrar no DI Container**

```csharp
// ServiceDependencyInjection.cs ou UseCaseDependencyInjection.cs

services.AddScoped<IValidator<RegisterDto>, RegisterProfessionalValidation>();
services.AddScoped<RegisterProfessionalUseCase>();
```

ou usando registro automático do FluentValidation:

```csharp
// Program.cs ou Startup.cs
services.AddValidatorsFromAssemblyContaining<RegisterProfessionalValidation>();
services.AddScoped<RegisterProfessionalUseCase>();
```

---

## ?? Fluxo de Validação

### **Passo 1: Validação**
```csharp
var validationResult = await _validator.ValidateAsync(dto);
```
- Executa **todas as regras** definidas em `RegisterProfessionalValidation`
- Valida: Name, CPF, Email, Phone, Address, Organization
- Retorna `ValidationResult` com lista de erros (se houver)

### **Passo 2: Verificação de Erros**
```csharp
if (!validationResult.IsValid)
{
    throw new ValidationException(validationResult.Errors);
}
```
- Se houver **qualquer erro**, lança `ValidationException`
- A exceção contém **todos os erros** encontrados
- O controller pode capturar e retornar HTTP 400 Bad Request

### **Passo 3: Processamento**
```csharp
var professional = dto.ToDomain();
// ... continua com registro
```
- **Apenas executa** se validação passou
- Garante que **dados estão corretos** antes de processar

---

## ? Benefícios da Implementação

### **1. Validação Centralizada**
- ? Todas as regras em um único lugar
- ? Fácil manutenção e atualização
- ? Reutilizável em outros use cases

### **2. Separação de Responsabilidades**
- ? Use Case: Orquestra o fluxo
- ? Validator: Valida dados
- ? Service: Executa lógica de negócio

### **3. Mensagens Internacionalizadas**
- ? Suporte a PT-BR e EN-US
- ? Mensagens claras para o usuário
- ? Fácil adicionar novos idiomas

### **4. Testabilidade**
- ? Fácil testar validações isoladamente
- ? Fácil testar use case com mock do validator
- ? Testes unitários mais simples

### **5. Fail Fast**
- ? Valida **antes** de processar
- ? Economiza recursos
- ? Retorna erro rapidamente ao usuário

---

## ?? Exemplo de Uso

### **Request válido**
```json
POST /api/professional/register
{
    "name": "Dr. João Silva",
    "cpf": "12345678900",
    "registerNumber": "06/12345",
    "professionalType": "Psychologist",
    "email": "joao@exemplo.com",
    "password": "SenhaForte@123",
    "phone": "+55 (11) 98399-1005",
    "street": "Rua das Flores",
    "number": "123",
    "neighborhood": "Centro",
    "zipCode": "01234567",
    "city": "São Paulo",
    "state": "SP",
    "razaoSocial": "Clínica Psicologia LTDA",
    "nomeFantasia": "Clínica Mente Sã",
    "cnpj": "12345678000195"
}

Response: 201 Created
{
    "id": 1,
    "message": "Professional registered successfully"
}
```

### **Request inválido**
```json
POST /api/professional/register
{
    "name": "Jo",
    "cpf": "123",
    "email": "invalid-email",
    "password": "weak",
    "phone": "123456"
}

Response: 400 Bad Request
{
    "status": 400,
    "title": "Validation Error",
    "errors": {
        "Name": ["O nome deve conter no mínimo 3 caracteres."],
        "Cpf": ["O CPF informado é inválido."],
        "Email": ["O e-mail informado é inválido."],
        "Password": [
            "A senha deve conter no mínimo 8 caracteres.",
            "A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais."
        ],
        "Phone": ["O telefone informado é inválido. Formato esperado: '+55 (11) 98399-1005' (celular) ou '+55 (11) 8399-1005' (fixo)."]
    }
}
```

---

## ?? Testes Unitários

### **Teste 1: Validação com dados válidos**
```csharp
[Fact]
public async Task Should_Execute_When_Dto_Is_Valid()
{
    // Arrange
    var dto = CreateValidDto();
    var mockProvider = new Mock<IProfessionalFactoryProvider>();
    var mockValidator = new Mock<IValidator<RegisterDto>>();
    
    mockValidator
        .Setup(v => v.ValidateAsync(dto, default))
        .ReturnsAsync(new ValidationResult());
    
    var useCase = new RegisterProfessionalUseCase(
        mockProvider.Object,
        mockValidator.Object);
    
    // Act & Assert
    await useCase.ExecuteAsync(dto); // Não deve lançar exceção
}
```

### **Teste 2: Validação com dados inválidos**
```csharp
[Fact]
public async Task Should_Throw_ValidationException_When_Dto_Is_Invalid()
{
    // Arrange
    var dto = CreateInvalidDto();
    var mockProvider = new Mock<IProfessionalFactoryProvider>();
    var mockValidator = new Mock<IValidator<RegisterDto>>();
    
    var validationFailures = new List<ValidationFailure>
    {
        new ValidationFailure("Name", "O nome é obrigatório.")
    };
    
    mockValidator
        .Setup(v => v.ValidateAsync(dto, default))
        .ReturnsAsync(new ValidationResult(validationFailures));
    
    var useCase = new RegisterProfessionalUseCase(
        mockProvider.Object,
        mockValidator.Object);
    
    // Act & Assert
    await Assert.ThrowsAsync<ValidationException>(
        () => useCase.ExecuteAsync(dto));
}
```

### **Teste 3: Validação é chamada antes do processamento**
```csharp
[Fact]
public async Task Should_Validate_Before_Processing()
{
    // Arrange
    var dto = CreateValidDto();
    var mockProvider = new Mock<IProfessionalFactoryProvider>();
    var mockValidator = new Mock<IValidator<RegisterDto>>();
    
    var callOrder = new List<string>();
    
    mockValidator
        .Setup(v => v.ValidateAsync(dto, default))
        .Callback(() => callOrder.Add("Validate"))
        .ReturnsAsync(new ValidationResult());
    
    mockProvider
        .Setup(p => p.GetFactory(It.IsAny<ProfessionalType>()))
        .Callback(() => callOrder.Add("GetFactory"))
        .Returns(Mock.Of<IProfessionalModuleFactory>());
    
    var useCase = new RegisterProfessionalUseCase(
        mockProvider.Object,
        mockValidator.Object);
    
    // Act
    await useCase.ExecuteAsync(dto);
    
    // Assert
    Assert.Equal("Validate", callOrder[0]);
    Assert.Equal("GetFactory", callOrder[1]);
}
```

---

## ?? Tratamento de Erros no Controller

```csharp
[HttpPost("register")]
public async Task<IActionResult> Register([FromBody] RegisterDto dto)
{
    try
    {
        var professionalId = await _registerUseCase.ExecuteAsync(dto);
        
        return CreatedAtAction(
            nameof(GetById),
            new { id = professionalId },
            new { id = professionalId, message = "Professional registered successfully" });
    }
    catch (ValidationException ex)
    {
        // FluentValidation exception
        var errors = ex.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());
        
        return BadRequest(new
        {
            status = 400,
            title = "Validation Error",
            errors = errors
        });
    }
    catch (ConflictException ex)
    {
        // Conflito (ex: email já existe)
        return Conflict(new
        {
            status = 409,
            title = "Conflict",
            message = ex.Message
        });
    }
    catch (Exception ex)
    {
        // Erro interno
        _logger.LogError(ex, "Error registering professional");
        
        return StatusCode(500, new
        {
            status = 500,
            title = "Internal Server Error",
            message = "An error occurred while processing your request"
        });
    }
}
```

---

## ?? Comparação: Antes vs Depois

| Aspecto | Antes | Depois |
|---------|-------|--------|
| **Validação** | Sem validação explícita | FluentValidation integrado |
| **Mensagens de Erro** | Genéricas | Específicas e internacionalizadas |
| **Momento da Validação** | Durante processamento | Antes do processamento |
| **Testabilidade** | Difícil | Fácil (mock do validator) |
| **Manutenção** | Validações espalhadas | Centralizadas |
| **Performance** | Processa dados inválidos | Fail fast |

---

## ? Checklist de Implementação

- [x] ? Adicionar `IValidator<RegisterDto>` ao construtor
- [x] ? Injetar dependência no construtor
- [x] ? Validar DTO antes do processamento
- [x] ? Lançar `ValidationException` se inválido
- [x] ? Documentar método com XML comments
- [x] ? Build bem-sucedido
- [ ] ? Registrar no DI container
- [ ] ? Implementar tratamento no controller
- [ ] ? Criar testes unitários

---

## ?? Próximos Passos

1. **Registrar no DI**
   ```csharp
   services.AddScoped<IValidator<RegisterDto>, RegisterProfessionalValidation>();
   services.AddScoped<RegisterProfessionalUseCase>();
   ```

2. **Implementar Controller**
   - Tratar `ValidationException`
   - Retornar HTTP 400 com erros
   - Documentar com Swagger

3. **Criar Testes**
   - Testar validação com dados válidos
   - Testar validação com dados inválidos
   - Testar ordem de execução

4. **Configurar Middleware**
   - Capturar `ValidationException` globalmente
   - Formatar resposta padronizada
   - Log de erros de validação

---

## ?? Referências

- [FluentValidation Documentation](https://docs.fluentvalidation.net/)
- [ASP.NET Core Validation](https://learn.microsoft.com/en-us/aspnet/core/mvc/models/validation)
- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

---

**? Integração FluentValidation implementada com sucesso!**

**?? Use Case agora valida todos os dados antes de processar, garantindo robustez e segurança!**
