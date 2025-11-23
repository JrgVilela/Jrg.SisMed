# ?? RegisterProfessionalValidation - Documentação Completa

## ?? Visão Geral

A classe `RegisterProfessionalValidation` implementa todas as validações necessárias para o cadastro completo de um profissional no sistema, incluindo:
- Dados do Profissional
- Dados do Usuário
- Dados de Contato (Telefone)
- Dados de Endereço
- Dados da Organização

---

## ?? Estrutura de Validações

### **1. Validações do Profissional** ?????

| Campo | Validações | Mensagem de Erro |
|-------|-----------|------------------|
| `Name` | • Not Empty<br>• Min: 3 caracteres<br>• Max: 150 caracteres | "O nome do profissional é obrigatório."<br>"O nome deve conter no mínimo 3 caracteres."<br>"O nome deve conter no máximo 150 caracteres." |
| `Cpf` | • Not Empty<br>• Algoritmo de validação CPF | "O CPF é obrigatório."<br>"O CPF informado é inválido." |
| `RegisterNumber` | • Not Empty<br>• Max: 20 caracteres | "O número de registro profissional é obrigatório."<br>"O número de registro deve conter no máximo 20 caracteres." |
| `ProfessionalType` | • Is In Enum<br>• Not Default | "O tipo de profissional é inválido."<br>"O tipo de profissional é obrigatório." |

### **2. Validações do Usuário** ??

| Campo | Validações | Mensagem de Erro |
|-------|-----------|------------------|
| `Email` | • Not Empty<br>• Email Address<br>• Max: 100 caracteres | "O e-mail é obrigatório."<br>"O e-mail informado é inválido."<br>"O e-mail deve conter no máximo 100 caracteres." |
| `Password` | • Not Empty<br>• Min: 8 caracteres<br>• Max: 25 caracteres<br>• Senha forte (maiúsculas, minúsculas, números, especiais) | "A senha é obrigatória."<br>"A senha deve conter no mínimo 8 caracteres."<br>"A senha deve conter no máximo 25 caracteres."<br>"A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais." |

### **3. Validações de Contato** ??

| Campo | Validações | Mensagem de Erro |
|-------|-----------|------------------|
| `Phone` | • Not Empty<br>• Formato: 'DDI DDD NUMERO'<br>• DDI: 1-3 dígitos<br>• DDD: 2 dígitos<br>• NUMERO: 8 ou 9 dígitos | "O telefone é obrigatório."<br>"O telefone informado é inválido. Formato esperado: 'DDI DDD NUMERO' (ex: '55 11 987654321')." |

### **4. Validações de Endereço** ??

| Campo | Validações | Mensagem de Erro |
|-------|-----------|------------------|
| `Street` | • Not Empty<br>• Max: 200 caracteres | "A rua é obrigatória."<br>"A rua deve conter no máximo 200 caracteres." |
| `Number` | • Not Empty<br>• Max: 10 caracteres | "O número do endereço é obrigatório."<br>"O número deve conter no máximo 10 caracteres." |
| `Complement` | • Max: 100 caracteres (opcional) | "O complemento deve conter no máximo 100 caracteres." |
| `Neighborhood` | • Not Empty<br>• Max: 100 caracteres | "O bairro é obrigatório."<br>"O bairro deve conter no máximo 100 caracteres." |
| `ZipCode` | • Not Empty<br>• Formato CEP (8 dígitos) | "O CEP é obrigatório."<br>"O CEP informado é inválido." |
| `City` | • Not Empty<br>• Max: 100 caracteres | "A cidade é obrigatória."<br>"A cidade deve conter no máximo 100 caracteres." |
| `State` | • Not Empty<br>• Exactly 2 caracteres<br>• UF brasileira válida | "O estado é obrigatório."<br>"O estado deve conter exatamente 2 caracteres (sigla UF)."<br>"O estado informado é inválido." |

### **5. Validações da Organização** ??

| Campo | Validações | Mensagem de Erro |
|-------|-----------|------------------|
| `RazaoSocial` | • Not Empty<br>• Max: 200 caracteres | "A razão social é obrigatória."<br>"A razão social deve conter no máximo 200 caracteres." |
| `NomeFantasia` | • Not Empty<br>• Max: 200 caracteres | "O nome fantasia é obrigatório."<br>"O nome fantasia deve conter no máximo 200 caracteres." |
| `Cnpj` | • Not Empty<br>• Algoritmo de validação CNPJ | "O CNPJ é obrigatório."<br>"O CNPJ informado é inválido." |

---

## ?? Métodos de Validação Customizados

### **BeValidCpf(string cpf)**
```csharp
private bool BeValidCpf(string cpf)
{
    if (string.IsNullOrWhiteSpace(cpf))
        return false;

    return cpf.IsCpf();
}
```
- Usa o helper `IsCpf()` que implementa o algoritmo oficial da Receita Federal
- Valida dígitos verificadores
- Rejeita CPFs conhecidos como inválidos (ex: 11111111111)

### **BeValidCnpj(string cnpj)**
```csharp
private bool BeValidCnpj(string cnpj)
{
    if (string.IsNullOrWhiteSpace(cnpj))
        return false;

    return cnpj.IsCnpj();
}
```
- Usa o helper `IsCnpj()` que implementa o algoritmo oficial da Receita Federal
- Valida dígitos verificadores
- Rejeita CNPJs conhecidos como inválidos (ex: 11111111111111)

### **BeStrongPassword(string password)**
```csharp
private bool BeStrongPassword(string password)
{
    if (string.IsNullOrWhiteSpace(password))
        return false;

    return SecurityHelper.IsPasswordStrong(
        password,
        minLength: 8,
        requireUppercase: true,
        requireLowercase: true,
        requireDigit: true,
        requireSpecialChar: true
    );
}
```
**Critérios de senha forte:**
- ? Mínimo 8 caracteres
- ? Pelo menos 1 letra maiúscula
- ? Pelo menos 1 letra minúscula
- ? Pelo menos 1 número
- ? Pelo menos 1 caractere especial

### **BeValidPhone(string phone)**
```csharp
private bool BeValidPhone(string phone)
{
    var parts = phone.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    
    // Deve ter exatamente 3 partes: DDI, DDD, NUMERO
    if (parts.Length != 3)
        return false;

    string ddi = parts[0];   // 1-3 dígitos
    string ddd = parts[1];   // 2 dígitos
    string number = parts[2]; // 8 ou 9 dígitos

    // Validações...
}
```
**Formato esperado:** `DDI DDD NUMERO`

**Exemplos válidos:**
- `55 11 987654321` (celular brasileiro)
- `55 11 12345678` (fixo brasileiro)
- `1 212 5551234` (número EUA)

### **BeValidZipCode(string zipCode)**
```csharp
private bool BeValidZipCode(string zipCode)
{
    if (string.IsNullOrWhiteSpace(zipCode))
        return false;

    return zipCode.IsCep();
}
```
- Valida CEP brasileiro (8 dígitos)
- Aceita com ou sem formatação

### **BeValidBrazilianState(string state)**
```csharp
private bool BeValidBrazilianState(string state)
{
    var validStates = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
        "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
        "RS", "RO", "RR", "SC", "SP", "SE", "TO"
    };

    return validStates.Contains(state.ToUpperInvariant());
}
```
- Valida se é uma UF brasileira válida
- Case-insensitive
- 27 estados válidos

---

## ?? Exemplos de Uso

### **Exemplo de DTO Válido**
```csharp
var dto = new RegisterDto
{
    // Profissional
    Name = "Dr. João Silva",
    Cpf = "12345678900",
    RegisterNumber = "06/12345",
    ProfessionalType = ProfessionalType.Psychologist,
    
    // Usuário
    Email = "joao.silva@exemplo.com",
    Password = "SenhaForte@123",
    
    // Contato
    Phone = "55 11 987654321",
    
    // Endereço
    Street = "Rua das Flores",
    Number = "123",
    Complement = "Apto 45",
    Neighborhood = "Centro",
    ZipCode = "01234567",
    City = "São Paulo",
    State = "SP",
    
    // Organização
    RazaoSocial = "Clínica de Psicologia LTDA",
    NomeFantasia = "Clínica Mente Sã",
    Cnpj = "12345678000195"
};
```

### **Validando o DTO**
```csharp
var validator = new RegisterProfessionalValidation();
var result = await validator.ValidateAsync(dto);

if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine($"{error.PropertyName}: {error.ErrorMessage}");
    }
}
```

### **Exemplo de Resposta de Erro**
```json
{
    "errors": {
        "Name": ["O nome do profissional é obrigatório."],
        "Cpf": ["O CPF informado é inválido."],
        "Email": ["O e-mail informado é inválido."],
        "Password": ["A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais."],
        "Phone": ["O telefone informado é inválido. Formato esperado: 'DDI DDD NUMERO' (ex: '55 11 987654321')."],
        "ZipCode": ["O CEP informado é inválido."],
        "State": ["O estado informado é inválido."],
        "Cnpj": ["O CNPJ informado é inválido."]
    }
}
```

---

## ?? Casos de Teste Recomendados

### **Dados do Profissional**
- ? Nome válido (3-150 caracteres)
- ? Nome vazio
- ? Nome com 2 caracteres
- ? Nome com 151 caracteres
- ? CPF válido
- ? CPF inválido (dígitos errados)
- ? CPF inválido (todos iguais)

### **Dados do Usuário**
- ? Email válido
- ? Email sem @
- ? Email sem domínio
- ? Senha forte
- ? Senha sem maiúscula
- ? Senha sem número
- ? Senha sem caractere especial
- ? Senha com menos de 8 caracteres

### **Dados de Contato**
- ? Telefone válido: "55 11 987654321"
- ? Telefone sem espaços: "5511987654321"
- ? Telefone com 2 partes: "11 987654321"
- ? DDD com 3 dígitos: "55 111 987654321"
- ? Número com 7 dígitos: "55 11 1234567"

### **Dados de Endereço**
- ? CEP válido: "01234567"
- ? CEP com máscara: "01234-567"
- ? CEP com 7 dígitos
- ? Estado válido: "SP"
- ? Estado inválido: "XX"
- ? Estado com 3 letras: "SPA"

### **Dados da Organização**
- ? CNPJ válido
- ? CNPJ inválido (dígitos errados)
- ? CNPJ inválido (todos iguais)

---

## ?? Segurança

### **Validação de CPF/CNPJ**
- ? Usa algoritmo oficial da Receita Federal
- ? Valida dígitos verificadores
- ? Rejeita documentos conhecidos como inválidos
- ? Remove formatação automaticamente

### **Validação de Senha**
- ? Requer complexidade mínima
- ? Previne senhas fracas
- ? Compatível com OWASP guidelines

### **Validação de Telefone**
- ? Formato padronizado
- ? Suporta DDI internacional
- ? Valida quantidade de dígitos

---

## ?? Dependências

### **Helpers Utilizados:**
- `IsCpf()` - Domain.Helpers.ValidateHelper
- `IsCnpj()` - Domain.Helpers.ValidateHelper
- `IsCep()` - Domain.Helpers.ValidateHelper
- `IsPasswordStrong()` - Domain.Helpers.SecurityHelper

### **Pacotes:**
- FluentValidation (versão compatível com .NET 9)

---

## ? Checklist de Implementação

- [x] ? Validações do Profissional (Name, CPF, RegisterNumber, Type)
- [x] ? Validações do Usuário (Email, Password)
- [x] ? Validações de Contato (Phone)
- [x] ? Validações de Endereço (Street, Number, Complement, Neighborhood, ZipCode, City, State)
- [x] ? Validações da Organização (RazaoSocial, NomeFantasia, CNPJ)
- [x] ? Métodos customizados de validação
- [x] ? Mensagens de erro em português
- [x] ? Documentação XML
- [x] ? Build bem-sucedido
- [x] ? Testes de validação recomendados
- [x] ? Conformidade com SOLID principles

---

## ?? Status Final

**? Validação Completa e Pronta para Uso!**

A validação cobre **todos os campos** do `RegisterDto` e garante que apenas dados válidos e seguros sejam processados pelo sistema.

**Próximos Passos:**
1. Criar testes unitários para cada regra de validação
2. Integrar com controller/use case
3. Adicionar tratamento de erros customizado
4. Implementar logging de tentativas de validação falhadas

---

**?? Nota:** Todas as validações seguem as melhores práticas de segurança e usabilidade, garantindo uma experiência robusta para o usuário final.
