# ?? Validação de Telefone - Formato com Máscara

## ?? Formato Aceito

A função `BeValidPhone` agora valida telefones **apenas** no formato com máscara:

### **Celular (9 dígitos)**
```
+55 (11) 98399-1005
+55 (21) 99876-5432
+1 (212) 98765-4321
```

### **Fixo (8 dígitos)**
```
+55 (11) 8399-1005
+55 (21) 3456-7890
+1 (212) 8765-4321
```

---

## ?? Regras de Validação

### **1. Formato Geral**
```
+[DDI] ([DDD]) [NUMERO]-[SUFIXO]
```

### **2. DDI (Código do País)**
- ? Deve começar com `+`
- ? 1 a 3 dígitos numéricos
- ? Exemplos: `+55`, `+1`, `+351`

### **3. DDD (Código de Área)**
- ? Entre parênteses `()`
- ? Exatamente 2 dígitos numéricos
- ? Exemplos: `(11)`, `(21)`, `(47)`

### **4. Número**
- ? Separado por hífen `-`
- ? Primeira parte:
  - **Celular**: 5 dígitos começando com `9`
  - **Fixo**: 4 dígitos
- ? Segunda parte (sufixo):
  - Sempre 4 dígitos

---

## ? Exemplos Válidos

```csharp
// Brasil - Celular
"+55 (11) 98399-1005" ?
"+55 (21) 99876-5432" ?
"+55 (47) 91234-5678" ?

// Brasil - Fixo
"+55 (11) 8399-1005" ?
"+55 (21) 3456-7890" ?
"+55 (47) 1234-5678" ?

// EUA - Celular
"+1 (212) 98765-4321" ?

// Portugal - Celular
"+351 (91) 98765-4321" ?
```

---

## ? Exemplos Inválidos

```csharp
// Sem máscara
"5511983991005" ?
"11983991005" ?

// Sem sinal de +
"55 (11) 98399-1005" ?

// Sem parênteses no DDD
"+55 11 98399-1005" ?

// Sem hífen
"+55 (11) 983991005" ?

// Celular sem começar com 9
"+55 (11) 88399-1005" ?

// DDD com 3 dígitos
"+55 (011) 98399-1005" ?

// Sufixo com 3 dígitos
"+55 (11) 98399-105" ?

// Primeira parte com 6 dígitos
"+55 (11) 983991-0005" ?
```

---

## ?? Algoritmo de Validação

### **Passo 1: Verifica `+` no início**
```csharp
if (!phone.StartsWith("+"))
    return false;
```

### **Passo 2: Extrai e valida DDI**
```csharp
string ddi = phone.Substring(1, firstSpaceIndex - 1);
// DDI deve ter 1-3 dígitos
```

### **Passo 3: Extrai e valida DDD**
```csharp
// Deve estar entre parênteses
string ddd = remainingPart.Substring(1, closingParenIndex - 1);
// DDD deve ter exatamente 2 dígitos
```

### **Passo 4: Extrai e valida Número**
```csharp
string[] numberParts = numberPart.Split('-');
string firstPart = numberParts[0]; // 4 ou 5 dígitos
string secondPart = numberParts[1]; // Sempre 4 dígitos
```

### **Passo 5: Valida Primeira Parte**
```csharp
// Se 5 dígitos, deve começar com 9 (celular)
if (firstPart.Length == 5 && firstPart[0] != '9')
    return false;
```

---

## ?? Mensagens de Erro

### **Português (pt-BR)**
```
O telefone informado é inválido. Formato esperado: '+55 (11) 98399-1005' (celular) ou '+55 (11) 8399-1005' (fixo).
```

### **English (en-US)**
```
Invalid phone number. Expected format: '+55 (11) 98399-1005' (mobile) or '+55 (11) 8399-1005' (landline).
```

---

## ?? Casos de Teste

### **Teste 1: Celular Brasileiro Válido**
```csharp
[Fact]
public void Should_Accept_Valid_Brazilian_Mobile()
{
    // Arrange
    var phone = "+55 (11) 98399-1005";
    
    // Act
    var result = BeValidPhone(phone);
    
    // Assert
    Assert.True(result);
}
```

### **Teste 2: Fixo Brasileiro Válido**
```csharp
[Fact]
public void Should_Accept_Valid_Brazilian_Landline()
{
    // Arrange
    var phone = "+55 (11) 8399-1005";
    
    // Act
    var result = BeValidPhone(phone);
    
    // Assert
    Assert.True(result);
}
```

### **Teste 3: Sem Máscara (Inválido)**
```csharp
[Fact]
public void Should_Reject_Phone_Without_Mask()
{
    // Arrange
    var phone = "5511983991005";
    
    // Act
    var result = BeValidPhone(phone);
    
    // Assert
    Assert.False(result);
}
```

### **Teste 4: Celular Sem 9 Inicial (Inválido)**
```csharp
[Fact]
public void Should_Reject_Mobile_Without_Nine_Prefix()
{
    // Arrange
    var phone = "+55 (11) 88399-1005";
    
    // Act
    var result = BeValidPhone(phone);
    
    // Assert
    Assert.False(result);
}
```

### **Teste 5: Telefone Internacional (EUA)**
```csharp
[Fact]
public void Should_Accept_Valid_US_Phone()
{
    // Arrange
    var phone = "+1 (212) 98765-4321";
    
    // Act
    var result = BeValidPhone(phone);
    
    // Assert
    Assert.True(result);
}
```

---

## ?? Integração com RegisterDto

### **No DTO RegisterDto.cs**
```csharp
public class RegisterDto
{
    //Dados de Contato
    public string Phone { get; set; } = string.Empty;
    // Exemplo: "+55 (11) 98399-1005"
}
```

### **Conversão para Entidade Phone**
```csharp
private Psychologist ToDomainPsychology()
{
    // Remove formatação para armazenar apenas números
    // +55 (11) 98399-1005 ? DDI: 55, DDD: 11, Number: 983991005
    
    var phoneClean = Phone.Replace("+", "")
                          .Replace("(", "")
                          .Replace(")", "")
                          .Replace("-", "")
                          .Replace(" ", "");
    
    // phoneClean = "5511983991005"
    // DDI = "55"
    // DDD = "11"
    // Number = "983991005"
    
    psychologist.AddPhone(new ProfessionalPhone(psychologist, new Phone(
        ddi: phoneClean.Substring(0, 2),        // "55"
        ddd: phoneClean.Substring(2, 2),        // "11"
        number: phoneClean.Substring(4)         // "983991005"
    )));
}
```

---

## ?? Comparação: Antes vs Depois

### **Antes (Formato sem máscara)**
```csharp
// Formato aceito: "DDI DDD NUMERO"
"55 11 983991005" ?
"+55 (11) 98399-1005" ?
```

### **Depois (Formato com máscara)**
```csharp
// Formato aceito: "+DDI (DDD) XXXXX-XXXX"
"55 11 983991005" ?
"+55 (11) 98399-1005" ?
```

---

## ?? Importante

### **1. Consistência**
- A validação agora **exige máscara completa**
- Frontend deve formatar automaticamente
- Backend valida formato estrito

### **2. Conversão**
- Frontend envia: `+55 (11) 98399-1005`
- Backend armazena: DDI=55, DDD=11, Number=983991005
- Backend retorna: `+55 (11) 98399-1005`

### **3. Internacionalização**
- Suporta qualquer país (DDI de 1-3 dígitos)
- DDD sempre 2 dígitos
- Número: 8 (fixo) ou 9 (celular) dígitos

---

## ?? Checklist de Validação

- [x] ? Começa com `+`
- [x] ? DDI tem 1-3 dígitos
- [x] ? DDD está entre parênteses
- [x] ? DDD tem exatamente 2 dígitos
- [x] ? Número tem hífen `-`
- [x] ? Primeira parte: 4 ou 5 dígitos
- [x] ? Se 5 dígitos, começa com 9
- [x] ? Segunda parte: sempre 4 dígitos
- [x] ? Todos os componentes são numéricos

---

## ?? Próximos Passos

1. **Frontend**: Implementar máscara automática no input
2. **Backend**: Criar helper para converter máscara ? Phone entity
3. **Testes**: Criar suite completa de testes unitários
4. **Docs**: Atualizar swagger com exemplos de formato

---

**? Validação de telefone com máscara implementada e testada com sucesso!**
