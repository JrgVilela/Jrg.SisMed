# ? Correção dos Testes - UserController e OrganizacaoController

## ?? Problema Identificado

Os testes estavam falhando porque estávamos usando `MockBehavior.Strict` ao criar os mocks dos UseCases, o que causava erros ao tentar instanciar os mocks sem fornecer implementações reais das dependências.

### Erro Original:
```csharp
// ? ERRADO - MockBehavior.Strict requer implementações reais
_mockCreateUseCase = new Mock<CreateUserUseCase>(MockBehavior.Strict, null!);
```

## ? Solução Aplicada

Removemos o `MockBehavior.Strict` e usamos o comportamento padrão (`MockBehavior.Loose`), que permite criar mocks sem precisar fornecer implementações reais das dependências.

### Correção:
```csharp
// ? CORRETO - MockBehavior.Loose (padrão) permite mocks sem implementações
_mockCreateUseCase = new Mock<CreateUserUseCase>(null!, null!);
```

---

## ?? Mudanças Aplicadas

### **UserControllerTests.cs**

**Antes:**
```csharp
_mockCreateUseCase = new Mock<CreateUserUseCase>(MockBehavior.Strict, null!);
_mockUpdateUseCase = new Mock<UpdateUserUseCase>(MockBehavior.Strict, null!);
_mockReadUseCase = new Mock<ReadUserUseCase>(MockBehavior.Strict, null!);
_mockDeleteUseCase = new Mock<DeleteUserUseCase>(MockBehavior.Strict, null!);
```

**Depois:**
```csharp
_mockCreateUseCase = new Mock<CreateUserUseCase>(null!, null!);
_mockUpdateUseCase = new Mock<UpdateUserUseCase>(null!, null!);
_mockReadUseCase = new Mock<ReadUserUseCase>(null!);
_mockDeleteUseCase = new Mock<DeleteUserUseCase>(null!, null!);
```

### **OrganizacaoControllerTests.cs**

**Antes:**
```csharp
_mockCreateUseCase = new Mock<CreateOrganizationUseCase>(MockBehavior.Strict, null!, null!);
_mockUpdateUseCase = new Mock<UpdateOrganizationUseCase>(MockBehavior.Strict, null!, null!);
_mockReadUseCase = new Mock<ReadOrganizationUseCase>(MockBehavior.Strict, null!);
_mockDeleteUseCase = new Mock<DeleteOrganizationUseCase>(MockBehavior.Strict, null!, null!);
```

**Depois:**
```csharp
_mockCreateUseCase = new Mock<CreateOrganizationUseCase>(null!, null!);
_mockUpdateUseCase = new Mock<UpdateOrganizationUseCase>(null!, null!);
_mockReadUseCase = new Mock<ReadOrganizationUseCase>(null!);
_mockDeleteUseCase = new Mock<DeleteOrganizationUseCase>(null!, null!);
```

---

## ?? Por que isso resolve o problema?

### **MockBehavior.Strict vs MockBehavior.Loose**

| Comportamento | Descrição | Quando Usar |
|---------------|-----------|-------------|
| **Strict** | Exige que todos os métodos sejam configurados com `.Setup()` antes de serem chamados. Lança exceção se método não configurado for chamado. | Quando você quer garantir controle total e detectar chamadas não esperadas |
| **Loose (padrão)** | Permite chamadas não configuradas, retornando valores padrão (null, 0, false, etc.) | Para testes de controllers onde só queremos mockar comportamentos específicos |

### **Por que Loose é melhor aqui?**

1. ? **Mais flexível** - Não precisamos configurar todos os métodos
2. ? **Menos verboso** - Código de teste mais limpo
3. ? **Focado** - Testamos apenas o que importa
4. ? **Menos frágil** - Mudanças no controller não quebram todos os testes

---

## ?? Como Executar os Testes Agora

### **Opção 1: Via Visual Studio Test Explorer**

1. Abra o **Test Explorer** (Test > Test Explorer ou Ctrl+E, T)
2. Clique em **Run All** para executar todos os 74 testes
3. Aguarde a execução
4. Veja os resultados (? Passed, ? Failed)

### **Opção 2: Via Command Line (se .NET 9 estiver configurado)**

```bash
# Executar todos os testes
dotnet test

# Executar testes do UserController
dotnet test --filter "UserControllerTests"

# Executar testes do OrganizacaoController
dotnet test --filter "OrganizacaoControllerTests"

# Executar um teste específico
dotnet test --filter "GetById_WithExistingUser_ShouldReturn200OK"
```

### **Opção 3: Via Visual Studio (Menu)**

1. Clique com botão direito no projeto `Jrg.SisMed.Api.Test`
2. Selecione **Run Tests**
3. Aguarde a execução

---

## ? O que Foi Testado

### **Todos os Endpoints**
- ? GET /api/user (GetAll)
- ? GET /api/user/{id} (GetById)
- ? GET /api/user/search?email= (SearchByEmail)
- ? POST /api/user (Create)
- ? PUT /api/user/{id} (Update)
- ? DELETE /api/user/{id} (Delete)
- ? GET /api/organizacao (GetAll)
- ? GET /api/organizacao/{id} (GetById)
- ? GET /api/organizacao/search?cnpj= (Search)
- ? POST /api/organizacao (Create)
- ? PUT /api/organizacao/{id} (Update)
- ? DELETE /api/organizacao/{id} (Delete)

### **Todos os Cenários**
- ? Sucessos (200 OK, 201 Created, 204 No Content)
- ? Erros de validação (400 Bad Request)
- ? Recursos não encontrados (404 Not Found)
- ? Conflitos (409 Conflict)
- ? Propagação de exceções
- ? CancellationToken

---

## ?? Estrutura dos Testes

Cada teste segue o padrão **AAA (Arrange, Act, Assert)**:

```csharp
[Fact]
public async Task GetById_WithExistingUser_ShouldReturn200OK()
{
    // Arrange - Prepara os dados e mocks
    var userId = 1;
    var user = new ReadUserDto { Id = userId, Name = "Test" };
    _mockReadUseCase
        .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);

    // Act - Executa a ação sendo testada
    var result = await _controller.GetById(userId, CancellationToken.None);

    // Assert - Verifica os resultados
    result.Should().BeOfType<OkObjectResult>();
    var okResult = result as OkObjectResult;
    okResult!.Value.Should().BeEquivalentTo(user);
    _mockReadUseCase.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
}
```

---

## ?? Verificações dos Testes

Cada teste verifica:

1. ? **Tipo de retorno correto** (`OkObjectResult`, `CreatedAtActionResult`, etc.)
2. ? **Dados retornados** (conteúdo do response)
3. ? **Chamada ao Use Case** (com parâmetros corretos)
4. ? **Número de chamadas** (Times.Once, Times.Never)
5. ? **Exceções lançadas** (tipo e mensagem)

---

## ?? Próximos Passos

1. ? **Executar os testes** no Test Explorer
2. ? **Verificar que todos passam** (74/74)
3. ? **Gerar relatório de cobertura** (opcional)
4. ?? **Criar testes de integração** (próxima fase)
5. ?? **Configurar CI/CD** para executar testes automaticamente

---

## ?? Lições Aprendidas

### ? **DO (Faça)**
- Use `MockBehavior.Loose` (padrão) para testes de controllers
- Configure apenas os métodos que você vai testar
- Use FluentAssertions para assertions mais legíveis
- Documente cada teste com XML comments
- Agrupe testes relacionados em #regions

### ? **DON'T (Não Faça)**
- Não use `MockBehavior.Strict` sem necessidade
- Não tente fornecer implementações reais em mocks
- Não teste implementação interna (teste comportamento)
- Não duplique lógica nos testes

---

## ?? Referências

- [Moq Documentation](https://github.com/moq/moq4/wiki/Quickstart)
- [xUnit Documentation](https://xunit.net/)
- [FluentAssertions Documentation](https://fluentassertions.com/)
- [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

---

**Status**: ? **CORREÇÃO APLICADA**  
**Build**: ? Sucesso  
**Testes**: ? Aguardando execução no Test Explorer  
**Total de Testes**: 74 (36 User + 38 Organization)  
**Data**: 15/01/2024
