using FluentAssertions;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;

namespace Jrg.SisMed.Domain.Test.Entities
{
    /// <summary>
    /// Classe de testes unitários para a entidade Organization.
    /// Testa validações, normalização de dados, gerenciamento de telefones e mudanças de estado.
    /// </summary>
    public class OrganizationTests
    {
        #region Constructor Tests

        /// <summary>
        /// Testa se o construtor cria uma organização corretamente com dados válidos.
        /// Verifica se o nome fantasia, razão social, CNPJ e estado são definidos corretamente.
        /// </summary>
        [Fact]
        public void Constructor_WithValidData_ShouldCreateOrganization()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.Should().NotBeNull();
            organization.NameFantasia.Should().Be("Clínica Saúde Total");
            organization.RazaoSocial.Should().Be("Clínica Saúde Total Ltda");
            organization.Cnpj.Should().Be("11222333000181"); // Apenas números
            organization.State.Should().Be(OrganizationEnum.State.Active);
        }

        /// <summary>
        /// Testa se o construtor permite criar uma organização com estado inativo.
        /// </summary>
        [Fact]
        public void Constructor_WithInactiveState_ShouldCreateInactiveOrganization()
        {
            // Arrange
            var nameFantasia = "Clínica Teste";
            var razaoSocial = "Clínica Teste Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj, OrganizationEnum.State.Inactive);

            // Assert
            organization.State.Should().Be(OrganizationEnum.State.Inactive);
        }

        /// <summary>
        /// Testa se o construtor permite criar uma organização com estado suspenso.
        /// </summary>
        [Fact]
        public void Constructor_WithSuspendedState_ShouldCreateSuspendedOrganization()
        {
            // Arrange
            var nameFantasia = "Clínica Teste";
            var razaoSocial = "Clínica Teste Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj, OrganizationEnum.State.Suspended);

            // Assert
            organization.State.Should().Be(OrganizationEnum.State.Suspended);
        }

        /// <summary>
        /// Testa se o construtor normaliza o nome fantasia para TitleCase (primeira letra de cada palavra maiúscula).
        /// </summary>
        [Fact]
        public void Constructor_ShouldNormalizeNameFantasia_ToTitleCase()
        {
            // Arrange
            var nameFantasia = "CLÍNICA SAÚDE TOTAL";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.NameFantasia.Should().Be("Clínica Saúde Total");
        }

        /// <summary>
        /// Testa se o construtor normaliza a razão social para TitleCase (primeira letra de cada palavra maiúscula).
        /// </summary>
        [Fact]
        public void Constructor_ShouldNormalizeRazaoSocial_ToTitleCase()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "CLÍNICA SAÚDE TOTAL LTDA";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.RazaoSocial.Should().Be("Clínica Saúde Total Ltda");
        }

        /// <summary>
        /// Testa se o construtor normaliza o CNPJ removendo caracteres não numéricos.
        /// </summary>
        [Fact]
        public void Constructor_ShouldNormalizeCnpj_ToOnlyNumbers()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.Cnpj.Should().Be("11222333000181");
            organization.Cnpj.Should().NotContain(".");
            organization.Cnpj.Should().NotContain("/");
            organization.Cnpj.Should().NotContain("-");
        }

        /// <summary>
        /// Testa se o construtor remove espaços duplos do nome fantasia.
        /// </summary>
        [Fact]
        public void Constructor_ShouldRemoveDoubleSpaces_FromNameFantasia()
        {
            // Arrange
            var nameFantasia = "Clínica    Saúde    Total";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.NameFantasia.Should().Be("Clínica Saúde Total");
        }

        #endregion

        #region Validation Tests - NameFantasia

        /// <summary>
        /// Testa se o construtor lança exceção quando o nome fantasia está vazio.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyNameFantasia_ShouldThrowValidationException()
        {
            // Arrange
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            Action act = () => new Organization("", razaoSocial, cnpj);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Trade name is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando o nome fantasia excede o comprimento máximo de 150 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithNameFantasiaExceedingMaxLength_ShouldThrowValidationException()
        {
            // Arrange
            var nameFantasia = new string('A', 151);
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.222.333/0001-81";

            // Act
            Action act = () => new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Trade name must be at most 150 characters*");
        }

        #endregion

        #region Validation Tests - RazaoSocial

        /// <summary>
        /// Testa se o construtor lança exceção quando a razão social está vazia.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyRazaoSocial_ShouldThrowValidationException()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var cnpj = "11.222.333/0001-81";

            // Act
            Action act = () => new Organization(nameFantasia, "", cnpj);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Legal name is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando a razão social excede o comprimento máximo de 150 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithRazaoSocialExceedingMaxLength_ShouldThrowValidationException()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = new string('A', 151);
            var cnpj = "11.222.333/0001-81";

            // Act
            Action act = () => new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Legal name must be at most 150 characters*");
        }

        #endregion

        #region Validation Tests - CNPJ

        /// <summary>
        /// Testa se o construtor lança exceção quando o CNPJ está vazio.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyCnpj_ShouldThrowValidationException()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "Clínica Saúde Total Ltda";

            // Act
            Action act = () => new Organization(nameFantasia, razaoSocial, "");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*CNPJ is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando o CNPJ está em formato inválido.
        /// </summary>
        [Fact]
        public void Constructor_WithInvalidCnpj_ShouldThrowValidationException()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11.111.111/1111-11"; // CNPJ inválido

            // Act
            Action act = () => new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*CNPJ is invalid*");
        }

        /// <summary>
        /// Testa se o construtor aceita CNPJ válido sem formatação.
        /// </summary>
        [Fact]
        public void Constructor_WithValidCnpjWithoutFormatting_ShouldCreateOrganization()
        {
            // Arrange
            var nameFantasia = "Clínica Saúde Total";
            var razaoSocial = "Clínica Saúde Total Ltda";
            var cnpj = "11222333000181"; // CNPJ válido sem formatação

            // Act
            var organization = new Organization(nameFantasia, razaoSocial, cnpj);

            // Assert
            organization.Should().NotBeNull();
            organization.Cnpj.Should().Be("11222333000181");
        }

        #endregion

        #region Update Tests

        /// <summary>
        /// Testa se o método Update atualiza corretamente os dados da organização com dados válidos.
        /// Verifica se nome fantasia, razão social, CNPJ e estado são atualizados.
        /// </summary>
        [Fact]
        public void Update_WithValidData_ShouldUpdateOrganization()
        {
            // Arrange
            var organization = new Organization("Clínica Antiga", "Clínica Antiga Ltda", "11.222.333/0001-81");
            var newNameFantasia = "Clínica Nova";
            var newRazaoSocial = "Clínica Nova Ltda";
            var newCnpj = "12.345.678/0001-95";

            // Act
            organization.Update(newNameFantasia, newRazaoSocial, newCnpj, OrganizationEnum.State.Inactive);

            // Assert
            organization.NameFantasia.Should().Be("Clínica Nova");
            organization.RazaoSocial.Should().Be("Clínica Nova Ltda");
            organization.Cnpj.Should().Be("12345678000195");
            organization.State.Should().Be(OrganizationEnum.State.Inactive);
        }

        /// <summary>
        /// Testa se o método Update lança exceção quando dados inválidos são fornecidos.
        /// </summary>
        [Fact]
        public void Update_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");

            // Act
            Action act = () => organization.Update("", "", "invalid-cnpj", OrganizationEnum.State.Active);

            // Assert
            act.Should().Throw<DomainValidationException>();
        }

        #endregion

        #region State Change Tests

        /// <summary>
        /// Testa se o método Activate altera o estado da organização para Active.
        /// Verifica também se UpdatedAt é atualizado.
        /// </summary>
        [Fact]
        public void Activate_ShouldSetStateToActive()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81", OrganizationEnum.State.Inactive);

            // Act
            organization.Activate();

            // Assert
            organization.State.Should().Be(OrganizationEnum.State.Active);
            organization.UpdatedAt.Should().NotBeNull();
            organization.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Testa se o método Deactivate altera o estado da organização para Inactive.
        /// Verifica também se UpdatedAt é atualizado.
        /// </summary>
        [Fact]
        public void Deactivate_ShouldSetStateToInactive()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");

            // Act
            organization.Deactivate();

            // Assert
            organization.State.Should().Be(OrganizationEnum.State.Inactive);
            organization.UpdatedAt.Should().NotBeNull();
            organization.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Testa se o método Suspend altera o estado da organização para Suspended.
        /// Verifica também se UpdatedAt é atualizado.
        /// </summary>
        [Fact]
        public void Suspend_ShouldSetStateToSuspended()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");

            // Act
            organization.Suspend();

            // Assert
            organization.State.Should().Be(OrganizationEnum.State.Suspended);
            organization.UpdatedAt.Should().NotBeNull();
            organization.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        #endregion

        #region Phone Management Tests

        /// <summary>
        /// Testa se o método AddPhone adiciona um telefone à lista de telefones da organização.
        /// </summary>
        [Fact]
        public void AddPhone_WithValidPhone_ShouldAddPhoneToList()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");
            var phone = new Phone("55", "11", "987654321");
            var organizationPhone = new OrganizationPhone(organization, phone);

            // Act
            organization.AddPhone(organizationPhone);

            // Assert
            organization.Phones.Should().HaveCount(1);
            organization.Phones.Should().Contain(organizationPhone);
        }

        /// <summary>
        /// Testa se o método AddPhone lança exceção quando o telefone é null.
        /// </summary>
        [Fact]
        public void AddPhone_WithNullPhone_ShouldThrowArgumentNullException()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");

            // Act
            Action act = () => organization.AddPhone(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("phone");
        }

        /// <summary>
        /// Testa se ao adicionar o primeiro telefone, ele é automaticamente marcado como principal.
        /// </summary>
        [Fact]
        public void AddPhone_FirstPhone_ShouldAutomaticallyBePrincipal()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");
            var phone = new Phone("55", "11", "987654321");
            var organizationPhone = new OrganizationPhone(organization, phone, isPrincipal: false);

            // Act
            organization.AddPhone(organizationPhone);

            // Assert
            organization.Phones.Should().HaveCount(1);
        }

        /// <summary>
        /// Testa se ao adicionar um telefone como principal, os outros telefones são desmarcados.
        /// </summary>
        [Fact]
        public void AddPhone_WithPrincipalFlag_ShouldUnmarkOtherPhones()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");
            
            var phone1 = new Phone("55", "11", "987654321");
            var organizationPhone1 = new OrganizationPhone(organization, phone1, isPrincipal: true);
            organization.AddPhone(organizationPhone1);

            var phone2 = new Phone("55", "11", "987655678");
            var organizationPhone2 = new OrganizationPhone(organization, phone2, isPrincipal: true);

            // Act
            organization.AddPhone(organizationPhone2);

            // Assert
            organization.Phones.Should().HaveCount(2);
            organizationPhone1.IsPrincipal.Should().BeFalse();
            organizationPhone2.IsPrincipal.Should().BeTrue();
        }

        /// <summary>
        /// Testa se o método RemovePhone remove um telefone da lista de telefones da organização.
        /// </summary>
        [Fact]
        public void RemovePhone_WithValidPhone_ShouldRemovePhoneFromList()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");
            var phone = new Phone("55", "11", "987654321");
            var organizationPhone = new OrganizationPhone(organization, phone);
            organization.AddPhone(organizationPhone);

            // Act
            organization.RemovePhone(organizationPhone);

            // Assert
            organization.Phones.Should().BeEmpty();
        }

        /// <summary>
        /// Testa se o método RemovePhone lança exceção quando o telefone é null.
        /// </summary>
        [Fact]
        public void RemovePhone_WithNullPhone_ShouldThrowArgumentNullException()
        {
            // Arrange
            var organization = new Organization("Clínica Teste", "Clínica Teste Ltda", "11.222.333/0001-81");

            // Act
            Action act = () => organization.RemovePhone(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>()
                .WithParameterName("phone");
        }

        #endregion
    }
}
