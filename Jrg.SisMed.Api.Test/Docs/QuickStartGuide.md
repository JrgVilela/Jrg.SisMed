# ?? Guia Rápido - Sistema de Mensagens Internacionalizadas

## ? Quick Start

### **1. Usando em Validações**

```csharp
public class MyValidation : AbstractValidator<MyDto>
{
    private readonly IStringLocalizer<Messages> _localizer;

    public MyValidation(IStringLocalizer<Messages> localizer)
    {
        _localizer = localizer;
        
        // Mensagem simples
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(_localizer.For(ProfessionalMessage.NameRequired).Value);
        
        // Mensagem com parâmetros
        RuleFor(x => x.Name)
            .MaximumLength(150)
            .WithMessage(_localizer.For(ProfessionalMessage.NameMaxLength, 150).Value);
    }
}
```

### **2. Registrando no DI**

```csharp
// Program.cs ou Startup.cs
services.AddScoped<IValidator<RegisterDto>, RegisterProfessionalValidation>();
```

### **3. Configurando Culturas**

```csharp
// Program.cs
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "pt-BR", "en-US" };
    options.SetDefaultCulture("pt-BR")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

// Middleware
app.UseRequestLocalization();
```

---

## ?? Enums Disponíveis

```csharp
// Professional
ProfessionalMessage.NameRequired
ProfessionalMessage.CpfInvalid
ProfessionalMessage.RegisterNumberRequired

// Address
AddressMessage.StreetRequired
AddressMessage.ZipCodeInvalid
AddressMessage.StateInvalid

// Phone
PhoneMessage.PhoneRequired
PhoneMessage.PhoneInvalid

// User
UserMessage.EmailRequired
UserMessage.PasswordRequired

// Organization
OrganizationMessage.CnpjRequired
OrganizationMessage.TradeNameRequired
```

---

## ?? Testando Idiomas

### **Via Header**
```http
GET /api/endpoint
Accept-Language: pt-BR  // Português
Accept-Language: en-US  // English
```

### **Via Query String**
```
https://api.exemplo.com/endpoint?culture=en-US
```

---

## ? Adicionando Nova Mensagem

### **Passo 1: Adicionar no Enum**
```csharp
// Messages.cs
public enum ProfessionalMessage
{
    // ...existing...
    DocumentRequired,  // Nova mensagem
}
```

### **Passo 2: Adicionar no .resx (PT)**
```xml
<!-- Messages.pt.resx -->
<data name="ProfessionalMessage_DocumentRequired" xml:space="preserve">
  <value>O documento é obrigatório.</value>
</data>
```

### **Passo 3: Adicionar no .resx (EN)**
```xml
<!-- Messages.en.resx -->
<data name="ProfessionalMessage_DocumentRequired" xml:space="preserve">
  <value>Document is required.</value>
</data>
```

### **Passo 4: Usar na Validação**
```csharp
RuleFor(x => x.Document)
    .NotEmpty()
    .WithMessage(_localizer.For(ProfessionalMessage.DocumentRequired).Value);
```

---

## ? Checklist de Implementação

- [ ] Criar enum no Messages.cs
- [ ] Adicionar strings no Messages.pt.resx
- [ ] Adicionar strings no Messages.en.resx
- [ ] Injetar IStringLocalizer no construtor
- [ ] Usar _localizer.For(Enum).Value nas validações
- [ ] Testar com Accept-Language: pt-BR
- [ ] Testar com Accept-Language: en-US
- [ ] Compilar sem erros

---

**? Pronto para usar! Sistema de mensagens internacionalizado e operacional.**
