# ?? Sistema de Mensagens Internacionalizadas - Implementação

## ?? Visão Geral

A validação `RegisterProfessionalValidation` foi **atualizada** para usar o sistema de mensagens internacionalizadas através do `IStringLocalizer<Messages>`, permitindo suporte a múltiplos idiomas.

---

## ?? Mudanças Implementadas

### **1. Novos Enums de Mensagens**

Foram adicionados 3 novos enums ao arquivo `Messages.cs`:

```csharp
public enum ProfessionalMessage  // 13 mensagens
public enum AddressMessage        // 12 mensagens  
public enum PhoneMessage          // 5 mensagens
```

### **2. Arquivos de Recursos Atualizados**

#### **Messages.pt.resx** (Português - Brasil)
- ? 35+ novas mensagens adicionadas
- ? Mensagens claras e objetivas
- ? Formato com placeholders `{0}`, `{1}` para parâmetros

#### **Messages.en.resx** (English - US)
- ? 35+ traduções em inglês
- ? Linguagem profissional e técnica
- ? Formato compatível com o sistema

---

## ?? Como Funciona

### **Injeção de Dependência**

A validação agora recebe `IStringLocalizer<Messages>` via construtor:

```csharp
public class RegisterProfessionalValidation : AbstractValidator<RegisterDto>
{
    private readonly IStringLocalizer<Messages> _localizer;

    public RegisterProfessionalValidation(IStringLocalizer<Messages> localizer)
    {
        _localizer = localizer;
        ConfigureValidationRules();
    }
}
```

### **Uso das Mensagens**

As mensagens são obtidas usando o método de extensão `For()`:

```csharp
// Mensagem simples (sem parâmetros)
.WithMessage(_localizer.For(ProfessionalMessage.NameRequired).Value)

// Mensagem com parâmetros
.WithMessage(_localizer.For(ProfessionalMessage.NameMaxLength, 150).Value)
```

### **Chaves Geradas Automaticamente**

O sistema gera automaticamente as chaves no formato:
```
{EnumName}_{EnumValue}
```

**Exemplos:**
- `ProfessionalMessage.NameRequired` ? `ProfessionalMessage_NameRequired`
- `AddressMessage.ZipCodeInvalid` ? `AddressMessage_ZipCodeInvalid`
- `PhoneMessage.PhoneRequired` ? `PhoneMessage_PhoneRequired`

---

## ?? Mensagens por Categoria

### **Professional (13 mensagens)**

| Enum | Português | English |
|------|-----------|---------|
| `NameRequired` | O nome do profissional é obrigatório. | Professional name is required. |
| `NameMinLength` | O nome deve conter no mínimo {0} caracteres. | Name must be at least {0} characters. |
| `NameMaxLength` | O nome deve conter no máximo {0} caracteres. | Name cannot exceed {0} characters. |
| `CpfRequired` | O CPF é obrigatório. | CPF is required. |
| `CpfInvalid` | O CPF informado é inválido. | The CPF provided is invalid. |
| `RegisterNumberRequired` | O número de registro profissional é obrigatório. | Professional registration number is required. |
| `RegisterNumberMaxLength` | O número de registro deve conter no máximo {0} caracteres. | Registration number cannot exceed {0} characters. |
| `ProfessionalTypeRequired` | O tipo de profissional é obrigatório. | Professional type is required. |
| `ProfessionalTypeInvalid` | O tipo de profissional é inválido. | Professional type is invalid. |
| `PasswordStrengthRequired` | A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais. | Password must contain uppercase letters, lowercase letters, numbers and special characters. |

### **Address (12 mensagens)**

| Enum | Português | English |
|------|-----------|---------|
| `StreetRequired` | A rua é obrigatória. | Street is required. |
| `StreetMaxLength` | A rua deve conter no máximo {0} caracteres. | Street cannot exceed {0} characters. |
| `NumberRequired` | O número do endereço é obrigatório. | Address number is required. |
| `NumberMaxLength` | O número deve conter no máximo {0} caracteres. | Number cannot exceed {0} characters. |
| `ComplementMaxLength` | O complemento deve conter no máximo {0} caracteres. | Complement cannot exceed {0} characters. |
| `NeighborhoodRequired` | O bairro é obrigatório. | Neighborhood is required. |
| `NeighborhoodMaxLength` | O bairro deve conter no máximo {0} caracteres. | Neighborhood cannot exceed {0} characters. |
| `ZipCodeRequired` | O CEP é obrigatório. | ZIP code is required. |
| `ZipCodeInvalid` | O CEP informado é inválido. | The ZIP code provided is invalid. |
| `CityRequired` | A cidade é obrigatória. | City is required. |
| `CityMaxLength` | A cidade deve conter no máximo {0} caracteres. | City cannot exceed {0} characters. |
| `StateRequired` | O estado é obrigatório. | State is required. |
| `StateLength` | O estado deve conter exatamente {0} caracteres (sigla UF). | State must be exactly {0} characters (UF abbreviation). |
| `StateInvalid` | O estado informado é inválido. | The state provided is invalid. |

### **Phone (5 mensagens)**

| Enum | Português | English |
|------|-----------|---------|
| `PhoneRequired` | O telefone é obrigatório. | Phone is required. |
| `PhoneInvalid` | O telefone informado é inválido. Formato esperado: 'DDI DDD NUMERO' (ex: '55 11 987654321'). | Invalid phone number. Expected format: 'DDI DDD NUMBER' (e.g., '55 11 987654321'). |
| `DdiInvalid` | O código do país (DDI) é inválido. | Country code (DDI) is invalid. |
| `DddInvalid` | O código de área (DDD) é inválido. | Area code (DDD) is invalid. |
| `NumberInvalid` | O número do telefone é inválido. | Phone number is invalid. |

---

## ?? Como Mudar o Idioma

### **Método 1: Via Request Header (API)**

```http
GET /api/professional/register
Accept-Language: pt-BR
```

ou

```http
GET /api/professional/register
Accept-Language: en-US
```

### **Método 2: Via Configuração no Startup/Program.cs**

```csharp
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "pt-BR", "en-US" };
    options.SetDefaultCulture("pt-BR")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});
```

### **Método 3: Via Query String**

```
https://api.exemplo.com/api/professional/register?culture=en-US
```

---

## ?? Exemplo de Resposta da API

### **Português (pt-BR)**

```json
{
  "status": 400,
  "title": "Validation Error",
  "errors": {
    "Name": ["O nome do profissional é obrigatório."],
    "Cpf": ["O CPF informado é inválido."],
    "Email": ["O e-mail é obrigatório."],
    "Password": ["A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais."],
    "Phone": ["O telefone informado é inválido. Formato esperado: 'DDI DDD NUMERO' (ex: '55 11 987654321')."],
    "ZipCode": ["O CEP informado é inválido."],
    "Cnpj": ["O CNPJ informado é inválido."]
  }
}
```

### **English (en-US)**

```json
{
  "status": 400,
  "title": "Validation Error",
  "errors": {
    "Name": ["Professional name is required."],
    "Cpf": ["The CPF provided is invalid."],
    "Email": ["Email is required."],
    "Password": ["Password must contain uppercase letters, lowercase letters, numbers and special characters."],
    "Phone": ["Invalid phone number. Expected format: 'DDI DDD NUMBER' (e.g., '55 11 987654321')."],
    "ZipCode": ["The ZIP code provided is invalid."],
    "Cnpj": ["The CNPJ provided is invalid."]
  }
}
```

---

## ?? Registrando a Validação no DI

### **No arquivo ServiceDependencyInjection.cs ou similar:**

```csharp
// Registra validators do FluentValidation
services.AddScoped<IValidator<RegisterDto>, RegisterProfessionalValidation>();

// Ou usando AddValidatorsFromAssemblyContaining
services.AddValidatorsFromAssemblyContaining<RegisterProfessionalValidation>();
```

### **FluentValidation Automatic Registration:**

```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterProfessionalValidation>();
```

---

## ?? Adicionando Novos Idiomas

### **Passo 1: Criar novo arquivo .resx**

Criar `Messages.es.resx` (Espanhol):

```xml
<data name="ProfessionalMessage_NameRequired" xml:space="preserve">
  <value>El nombre del profesional es obligatorio.</value>
</data>
```

### **Passo 2: Adicionar cultura suportada**

```csharp
var supportedCultures = new[] { "pt-BR", "en-US", "es-ES" };
```

### **Passo 3: Testar**

```http
GET /api/professional/register
Accept-Language: es-ES
```

---

## ? Vantagens da Implementação

### **1. Internacionalização Automática** ??
- Suporta múltiplos idiomas sem alterar código
- Fácil adicionar novos idiomas

### **2. Manutenção Centralizada** ??
- Todas as mensagens em um único local
- Fácil atualização e correção

### **3. Tipagem Forte** ??
- Uso de enums previne erros de digitação
- IntelliSense completo

### **4. Reutilização** ??
- Mensagens podem ser usadas em diferentes validações
- Evita duplicação

### **5. Parâmetros Dinâmicos** ??
- Suporte a placeholders `{0}`, `{1}`, etc.
- Mensagens contextualizadas

---

## ?? Boas Práticas

### ? **DO:**
- Use enums para categorizar mensagens
- Mantenha mensagens curtas e objetivas
- Use placeholders para valores dinâmicos
- Teste em todos os idiomas suportados
- Mantenha consistência entre traduções

### ? **DON'T:**
- Não use strings hardcoded nas validações
- Não misture idiomas no mesmo arquivo .resx
- Não esqueça de adicionar traduções para todos os idiomas
- Não use mensagens técnicas demais para o usuário final

---

## ?? Testando as Validações

```csharp
[Fact]
public async Task Should_Return_Localized_Message_In_Portuguese()
{
    // Arrange
    var localizer = CreateLocalizer("pt-BR");
    var validator = new RegisterProfessionalValidation(localizer);
    var dto = new RegisterDto { Name = "" }; // Nome vazio

    // Act
    var result = await validator.ValidateAsync(dto);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains("O nome do profissional é obrigatório.", 
        result.Errors.Select(e => e.ErrorMessage));
}

[Fact]
public async Task Should_Return_Localized_Message_In_English()
{
    // Arrange
    var localizer = CreateLocalizer("en-US");
    var validator = new RegisterProfessionalValidation(localizer);
    var dto = new RegisterDto { Name = "" };

    // Act
    var result = await validator.ValidateAsync(dto);

    // Assert
    Assert.False(result.IsValid);
    Assert.Contains("Professional name is required.", 
        result.Errors.Select(e => e.ErrorMessage));
}
```

---

## ?? Estatísticas

| Categoria | Quantidade de Mensagens |
|-----------|-------------------------|
| **ProfessionalMessage** | 13 |
| **AddressMessage** | 12 |
| **PhoneMessage** | 5 |
| **UserMessage** | 13 |
| **OrganizationMessage** | 10 |
| **CommonMessage** | 6 |
| **TOTAL** | **59** |

| Idioma | Arquivo | Status |
|--------|---------|--------|
| **Português (BR)** | `Messages.pt.resx` | ? Completo |
| **English (US)** | `Messages.en.resx` | ? Completo |
| **Espanhol (ES)** | `Messages.es.resx` | ? Pendente |
| **Francês (FR)** | `Messages.fr.resx` | ? Pendente |

---

## ?? Conclusão

A implementação do sistema de mensagens internacionalizadas torna o sistema:
- ? **Escalável** - Fácil adicionar novos idiomas
- ? **Manutenível** - Mensagens centralizadas
- ? **Profissional** - Suporte multi-idioma
- ? **Tipado** - Erros detectados em tempo de compilação
- ? **Testável** - Fácil criar testes automatizados

**?? Sistema pronto para mercado global!**
