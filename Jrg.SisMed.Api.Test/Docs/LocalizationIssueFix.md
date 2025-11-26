# ?? Solução: Mensagens de Validação Retornando Chaves

## ?? Problema

As mensagens de erro estavam retornando as **chaves** ao invés dos **valores traduzidos**.

### **Erro:**
```json
{
  "errors": [
    "ProfessionalType: ProfessionalMessage_ProfessionalTypeRequired"
  ]
}
```

### **Esperado:**
```json
{
  "errors": [
    "ProfessionalType: O tipo de profissional é obrigatório."
  ]
}
```

---

## ? Solução: Configurar UseRequestLocalization

### **Program.cs:**

```csharp
// Configuração de Localização
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

var supportedCultures = new[]
{
    new CultureInfo("pt-BR"),
    new CultureInfo("en-US")
};

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

var app = builder.Build();

// IMPORTANTE: Adicionar middleware
app.UseRequestLocalization();
```

---

## ?? Como Testar

### **Português:**
```http
POST /api/professional/register
Accept-Language: pt-BR
```

**Response:**
```json
{
  "errors": ["Nome: O nome é obrigatório."]
}
```

### **Inglês:**
```http
POST /api/professional/register
Accept-Language: en-US
```

**Response:**
```json
{
  "errors": ["Name: Name is required."]
}
```

---

**?? Localização funcionando!**
