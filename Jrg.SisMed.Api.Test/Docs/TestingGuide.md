# ?? Guia de Testes para Controllers - Jrg.SisMed.Api

## ?? Sumário

Este documento descreve a estratégia de testes implementada para os controllers `UserController` e `OrganizacaoController`.

---

## ?? Problema Identificado com DTOs

Os DTOs da aplicação têm propriedades com `private set`, o que impede inicializá-los diretamente nos testes:

```csharp
public class ReadOrganizationDto
{
    public int Id { get; set; }
    public string NameFantasia { get; private set; } = string.Empty;  // ? private set
    public string RazaoSocial { get; private set; } = string.Empty;   // ? private set
    public string Cnpj { get; private set; } = string.Empty;          // ? private set
    public OrganizationEnum.State State { get; private set; }         // ? private set
}
```

---

## ? Solução Recomendada

Existem **3 abordagens** para resolver este problema:

### **Opção 1: Usar Métodos Factory (RECOMENDADO)**

Criar mocks que retornem objetos já construídos via métodos factory:

```csharp
[Fact]
public async Task Get_WithExistingOrganizations_ShouldReturn200OK()
{
    // Arrange - Cria Organization do domínio
    var domainOrganization = new Organization(
        "Clínica Saúde",
        "Clínica Saúde Ltda",
        "11.222.333/0001-81",
        OrganizationEnum.State.Active
    );
    
    // Usa o método factory para criar o DTO
    var organizationDto = ReadOrganizationDto.FromDomainOrganization(domainOrganization);
    
    var organizations = new List<ReadOrganizationDto> { organizationDto };

    _mockReadUseCase
        .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
        .ReturnsAsync(organizations);

    // Act
    var result = await _controller.Get(CancellationToken.None);

    // Assert
    result.Should().BeOfType<OkObjectResult>();
}
```

### **Opção 2: Tornar DTOs Testáveis**

Modificar os DTOs para terem setters públicos ou construtores:

```csharp
public class ReadOrganizationDto
{
    public int Id { get; set; }
    public string NameFantasia { get; set; } = string.Empty;   // ? public set
    public string RazaoSocial { get; set; } = string.Empty;    // ? public set
    public string Cnpj { get; set; } = string.Empty;           // ? public set
    public OrganizationEnum.State State { get; set; }          // ? public set
}
```

**OU** usar `init`:

```csharp
public class ReadOrganizationDto
{
    public int Id { get; init; }
    public string NameFantasia { get; init; } = string.Empty;   // ? init
    public string RazaoSocial { get; init; } = string.Empty;    // ? init
    public string Cnpj { get; init; } = string.Empty;           // ? init
    public OrganizationEnum.State State { get; init; }          // ? init
}
```

### **Opção 3: Usar Reflection (NÃO RECOMENDADO)**

Usar reflection para setar propriedades privadas (código complexo e frágil):

```csharp
// ? Não recomendado - código complexo
var dto = new ReadOrganizationDto();
typeof(ReadOrganizationDto)
    .GetProperty("NameFantasia")!
    .SetValue(dto, "Clínica Saúde");
```

---

## ?? Estrutura de Testes Implementada

### **UserControllerTests** - 36 testes

| Categoria | Quantidade | Descrição |
|-----------|------------|-----------|
| GetAll | 3 | Retorna lista, lista vazia, propaga exceção |
| GetById | 4 | Retorna usuário, não encontrado, múltiplos IDs |
| SearchByEmail | 5 | Busca válida, email vazio, inválido, não encontrado |
| Create | 4 | Criação válida, dados inválidos, email duplicado, DTO null |
| Update | 3 | Atualização válida, não encontrado, dados inválidos |
| Delete | 3 | Exclusão válida, não encontrado, múltiplos IDs |
| CancellationToken | 2 | Propaga token, lança OperationCanceledException |

### **OrganizacaoControllerTests** - 38 testes

| Categoria | Quantidade | Descrição |
|-----------|------------|-----------|
| Get (GetAll) | 3 | Retorna lista, lista vazia, propaga exceção |
| Get by ID | 4 | Retorna organização, não encontrado, múltiplos IDs |
| Search by CNPJ | 6 | Busca válida, CNPJ vazio, inválido, formatos diferentes |
| Post (Create) | 5 | Criação válida, inválida, CNPJ duplicado, Razão Social duplicada, DTO null |
| Put (Update) | 4 | Atualização válida, não encontrado, inválida, múltiplos estados |
| Delete | 3 | Exclusão válida, não encontrado, múltiplos IDs |
| CancellationToken | 3 | Propaga token, cancelamento, Post com token |

---

## ?? Tipos de Testes Implementados

### **1. Testes de Sucesso (Happy Path)**
```csharp
[Fact]
public async Task GetById_WithExistingUser_ShouldReturn200OK()
```

### **2. Testes de Erro (Error Path)**
```csharp
[Fact]
public async Task GetById_WithNonExistingUser_ShouldThrowNotFoundException()
```

### **3. Testes Parametrizados (Data-Driven)**
```csharp
[Theory]
[InlineData(1)]
[InlineData(10)]
[InlineData(100)]
public async Task GetById_WithDifferentValidIds_ShouldCallUseCaseWithCorrectId(int userId)
```

### **4. Testes de Validação**
```csharp
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public async Task SearchByEmail_WithEmptyEmail_ShouldReturn400BadRequest(string? email)
```

### **5. Testes de Propagação de Exceções**
```csharp
[Fact]
public async Task Create_WithDuplicateEmail_ShouldThrowConflictException()
```

### **6. Testes de CancellationToken**
```csharp
[Fact]
public async Task GetAll_ShouldPropagateCancellationToken()
```

---

## ?? Como Ajustar os Testes

### Passo 1: Modificar DTOs para Usar `init`

Edite todos os DTOs para usar `init` ao invés de `private set`:

**Antes:**
```csharp
public string NameFantasia { get; private set; } = string.Empty;
```

**Depois:**
```csharp
public string NameFantasia { get; init; } = string.Empty;
```

**Arquivos a modificar:**
- `ReadOrganizationDto.cs`
- `CreateOrganizationDto.cs`
- `UpdateOrganizationDto.cs`
- `ReadUserDto.cs`
- `CreateUserDto.cs`
- `UpdateUserDto.cs`

### Passo 2: Ajustar os Testes

Depois de modificar os DTOs, os testes funcionarão com inicializadores de objetos:

```csharp
var organization = new ReadOrganizationDto
{
    Id = 1,
    NameFantasia = "Clínica Saúde",
    RazaoSocial = "Clínica Saúde Ltda",
    Cnpj = "11222333000181",
    State = OrganizationEnum.State.Active
};
```

---

## ?? Cobertura de Testes

### **Cenários Testados**

? **Respostas HTTP**
- 200 OK
- 201 Created  
- 204 No Content
- 400 Bad Request
- 404 Not Found
- 409 Conflict

? **Exceções**
- `DomainValidationException`
- `NotFoundException`
- `ConflictException`
- `ArgumentException`
- `ArgumentNullException`
- `OperationCanceledException`

? **Validações**
- Campos obrigatórios
- Formatos inválidos
- Duplicidades
- Limites de tamanho

? **Edge Cases**
- Listas vazias
- Valores nulos
- IDs inválidos
- Cancelamento de operações

---

## ?? Como Executar os Testes

### Via Visual Studio
1. Abra o **Test Explorer** (Test > Test Explorer)
2. Clique em **Run All** para executar todos os testes
3. Ou clique com botão direito em um teste específico > **Run**

### Via Command Line
```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
dotnet test Jrg.SisMed.Api.Test

# Executar com coverage
dotnet test --collect:"XPlat Code Coverage"

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~UserControllerTests"
```

---

## ?? Próximos Passos

1. ? **Modificar DTOs** para usar `init` ou `set` público
2. ? **Executar testes** e verificar que todos passam
3. ? **Adicionar testes de integração** (opcional)
4. ? **Configurar CI/CD** para executar testes automaticamente
5. ? **Gerar relatório de cobertura** de código

---

## ?? Referências

- [xUnit Documentation](https://xunit.net/)
- [Moq Documentation](https://github.com/moq/moq4)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [ASP.NET Core Testing Best Practices](https://docs.microsoft.com/en-us/aspnet/core/test/)

---

**Status**: ?? DTOs precisam ser ajustados para permitir testes  
**Testes Criados**: 74 (36 User + 38 Organization)  
**Próxima Ação**: Modificar DTOs para usar `init`  
**Data**: 15/01/2024
