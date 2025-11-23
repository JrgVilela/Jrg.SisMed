# ?? Configuração OrganizationProfessional - Documentação

## ?? Visão Geral

A classe `OrganizationProfessionalConfiguration` configura o relacionamento **muitos-para-muitos** entre `Organization` e `Professional` no Entity Framework Core.

---

## ??? Estrutura do Relacionamento

```
Organization (1) ?? (N) OrganizationProfessional (N) ?? (1) Professional
```

### **Tipo de Relacionamento:**
- **Muitos-para-Muitos com Entidade Intermediária**
- Um `Organization` pode ter vários `Professionals`
- Um `Professional` pode pertencer a várias `Organizations`
- A entidade `OrganizationProfessional` armazena metadados adicionais (State, CreatedBy, UpdatedBy)

---

## ?? Estrutura da Tabela

### **Tabela:** `OrganizationProfessionals`

| Coluna | Tipo | Nullable | Descrição |
|--------|------|----------|-----------|
| `Id` | int | NOT NULL | Chave primária (Identity) |
| `OrganizationId` | Guid | NOT NULL | FK para Organizations |
| `ProfessionalId` | Guid | NOT NULL | FK para Professionals |
| `State` | int | NOT NULL | Estado: 1=Active, 2=Inactive, 3=Blocked |
| `CreatedAt` | datetime | NOT NULL | Data de criação (default: GETUTCDATE()) |
| `UpdatedAt` | datetime | NULL | Data de última atualização |
| `CreatedById` | int | NULL | ID do profissional que criou |
| `UpdatedById` | int | NULL | ID do profissional que atualizou |

---

## ?? Relacionamentos Configurados

### **1. Organization ? OrganizationProfessional** (1:N)
```csharp
builder.HasOne(op => op.Organization)
    .WithMany(o => o.Professionals)
    .HasForeignKey(op => op.OrganizationId)
    .OnDelete(DeleteBehavior.Cascade)  // Deleta associações se organização for deletada
    .HasConstraintName("FK_OrganizationProfessionals_Organizations");
```

### **2. Professional ? OrganizationProfessional** (1:N)
```csharp
builder.HasOne(op => op.Professional)
    .WithMany(p => p.Organizations)
    .HasForeignKey(op => op.ProfessionalId)
    .OnDelete(DeleteBehavior.Cascade)  // Deleta associações se profissional for deletado
    .HasConstraintName("FK_OrganizationProfessionals_Professionals");
```

### **3. CreatedBy ? Professional** (N:1)
```csharp
builder.HasOne(op => op.CreatedBy)
    .WithMany()
    .HasForeignKey(op => op.CreatedById)
    .OnDelete(DeleteBehavior.Restrict)  // Não permite deletar profissional que criou associações
    .HasConstraintName("FK_OrganizationProfessionals_CreatedBy");
```

### **4. UpdatedBy ? Professional** (N:1)
```csharp
builder.HasOne(op => op.UpdatedBy)
    .WithMany()
    .HasForeignKey(op => op.UpdatedById)
    .OnDelete(DeleteBehavior.Restrict)  // Não permite deletar profissional que atualizou associações
    .HasConstraintName("FK_OrganizationProfessionals_UpdatedBy");
```

---

## ?? Índices Criados

### **Índice Único (Garante Unicidade)**
```csharp
IX_OrganizationProfessionals_OrgId_ProfId (OrganizationId, ProfessionalId) UNIQUE
```
- **Propósito**: Impede que um profissional seja associado mais de uma vez à mesma organização
- **Exemplo**: Não permite dois registros com `OrganizationId = 1` e `ProfessionalId = 1`

### **Índices de Performance**
```csharp
IX_OrganizationProfessionals_OrganizationId (OrganizationId)
IX_OrganizationProfessionals_ProfessionalId (ProfessionalId)
IX_OrganizationProfessionals_State (State)
IX_OrganizationProfessionals_CreatedAt (CreatedAt)
IX_OrganizationProfessionals_CreatedById (CreatedById)
IX_OrganizationProfessionals_UpdatedById (UpdatedById)
```

---

## ??? Comportamentos de Deleção

| Relacionamento | Comportamento | Descrição |
|----------------|---------------|-----------|
| **Organization ? OrganizationProfessional** | `CASCADE` | Deleta todas as associações quando organização é deletada |
| **Professional ? OrganizationProfessional** | `CASCADE` | Deleta todas as associações quando profissional é deletado |
| **CreatedBy ? OrganizationProfessional** | `RESTRICT` | Impede deleção de profissional que criou associações |
| **UpdatedBy ? OrganizationProfessional** | `RESTRICT` | Impede deleção de profissional que atualizou associações |

---

## ?? Exemplos de Uso

### **Criar Associação**
```csharp
var orgProf = new OrganizationProfessional(
    organizationId: organizationGuid,
    professionalId: professionalGuid
);

orgProf.SetCreatedBy(currentProfessional);

await context.OrganizationProfessionals.AddAsync(orgProf);
await context.SaveChangesAsync();
```

### **Buscar Profissionais de uma Organização**
```csharp
var professionals = await context.OrganizationProfessionals
    .Where(op => op.OrganizationId == orgId && op.State == OrganizationProfessionalEnum.State.Active)
    .Include(op => op.Professional)
    .Select(op => op.Professional)
    .ToListAsync();
```

### **Buscar Organizações de um Profissional**
```csharp
var organizations = await context.OrganizationProfessionals
    .Where(op => op.ProfessionalId == profId && op.State == OrganizationProfessionalEnum.State.Active)
    .Include(op => op.Organization)
    .Select(op => op.Organization)
    .ToListAsync();
```

### **Desativar Associação**
```csharp
var orgProf = await context.OrganizationProfessionals
    .FirstOrDefaultAsync(op => op.OrganizationId == orgId && op.ProfessionalId == profId);

if (orgProf != null)
{
    // Assumindo que existe um método SetState na entidade
    orgProf.State = OrganizationProfessionalEnum.State.Inactive;
    orgProf.SetUpdatedBy(currentProfessional);
    await context.SaveChangesAsync();
}
```

---

## ?? Considerações Importantes

### **1. Índice Único**
O índice único `IX_OrganizationProfessionals_OrgId_ProfId` garante que:
- ? Um profissional não pode ser associado duas vezes à mesma organização
- ? Tentativas de duplicação gerarão exceção `DbUpdateException`

### **2. Cascade Delete**
- ?? **Cuidado**: Se deletar uma `Organization` ou `Professional`, **todas as associações serão deletadas automaticamente**
- ?? **Recomendação**: Considere usar "soft delete" (marcar como inativo) ao invés de deletar

### **3. Restrict em Auditoria**
- Os campos `CreatedBy` e `UpdatedBy` usam `DeleteBehavior.Restrict`
- Isso significa que **não é possível deletar um profissional** que tenha criado ou atualizado associações
- ?? **Solução**: Antes de deletar, você precisa:
  1. Setar `CreatedById` e `UpdatedById` para NULL, ou
  2. Transferir para outro profissional, ou
  3. Deletar as associações primeiro

### **4. Performance**
Os índices criados otimizam as seguintes queries:
- ? Buscar profissionais de uma organização
- ? Buscar organizações de um profissional
- ? Filtrar por estado (Active/Inactive/Blocked)
- ? Ordenar por data de criação
- ? Buscar quem criou/atualizou associações

---

## ?? Migration Recomendada

Para criar a tabela no banco de dados:

```bash
dotnet ef migrations add AddOrganizationProfessionalRelationship -p Jrg.SisMed.Infra.Data -s Jrg.SisMed.Api
dotnet ef database update -p Jrg.SisMed.Infra.Data -s Jrg.SisMed.Api
```

---

## ?? Checklist de Validação

- [x] ? Tabela configurada (`OrganizationProfessionals`)
- [x] ? Chave primária definida (`Id`)
- [x] ? Foreign Keys configuradas (4 relacionamentos)
- [x] ? Índice único para evitar duplicação
- [x] ? Índices de performance criados
- [x] ? Propriedades de auditoria configuradas
- [x] ? Comportamentos de deleção definidos
- [x] ? Comentários no banco de dados
- [x] ? Build bem-sucedido

---

## ?? Referências

- [EF Core - Many-to-Many Relationships](https://learn.microsoft.com/en-us/ef/core/modeling/relationships/many-to-many)
- [EF Core - Cascade Delete](https://learn.microsoft.com/en-us/ef/core/saving/cascade-delete)
- [EF Core - Indexes](https://learn.microsoft.com/en-us/ef/core/modeling/indexes)

---

**? Configuração Completa e Pronta para Uso!**
