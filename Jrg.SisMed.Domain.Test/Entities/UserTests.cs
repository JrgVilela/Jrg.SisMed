using FluentAssertions;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;

namespace Jrg.SisMed.Domain.Test.Entities
{
    /// <summary>
    /// Classe de testes unitários para a entidade User.
    /// Testa validações, normalização de dados, hash de senha e mudanças de estado.
    /// </summary>
    public class UserTests
    {
        #region Constructor Tests

        /// <summary>
        /// Testa se o construtor cria um usuário corretamente com dados válidos.
        /// Verifica se o nome, email, estado e data de criação são definidos corretamente.
        /// </summary>
        [Fact]
        public void Constructor_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao.silva@email.com";
            var password = "SenhaForte@123";

            // Act
            var user = new User(name, email, password);

            // Assert
            user.Should().NotBeNull();
            user.Name.Should().Be("João Silva");
            user.Email.Should().Be("joao.silva@email.com");
            user.State.Should().Be(UserEnum.State.Active);
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Testa se o construtor permite criar um usuário com estado inativo.
        /// </summary>
        [Fact]
        public void Constructor_WithInactiveState_ShouldCreateInactiveUser()
        {
            // Arrange
            var name = "Maria Santos";
            var email = "maria@email.com";
            var password = "SenhaForte@456";

            // Act
            var user = new User(name, email, password, UserEnum.State.Inactive);

            // Assert
            user.State.Should().Be(UserEnum.State.Inactive);
        }

        /// <summary>
        /// Testa se o construtor normaliza o nome para TitleCase (primeira letra de cada palavra maiúscula).
        /// </summary>
        [Fact]
        public void Constructor_ShouldNormalizeName_ToTitleCase()
        {
            // Arrange
            var name = "JOÃO SILVA";
            var email = "joao@email.com";
            var password = "SenhaForte@123";

            // Act
            var user = new User(name, email, password);

            // Assert
            user.Name.Should().Be("João Silva");
        }

        /// <summary>
        /// Testa se o construtor normaliza o email para minúsculas.
        /// </summary>
        [Fact]
        public void Constructor_ShouldNormalizeEmail_ToLowerCase()
        {
            // Arrange
            var name = "João Silva";
            var email = "JOAO.SILVA@EMAIL.COM";
            var password = "SenhaForte@123";

            // Act
            var user = new User(name, email, password);

            // Assert
            user.Email.Should().Be("joao.silva@email.com");
        }

        /// <summary>
        /// Testa se o construtor gera o hash da senha ao invés de armazená-la em texto plano.
        /// </summary>
        [Fact]
        public void Constructor_ShouldHashPassword()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@email.com";
            var password = "SenhaForte@123";

            // Act
            var user = new User(name, email, password);

            // Assert
            user.Password.Should().NotBe(password);
            user.Password.Should().NotBeNullOrEmpty();
        }

        #endregion

        #region Validation Tests - Name

        /// <summary>
        /// Testa se o construtor lança exceção quando o nome está vazio.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowValidationException()
        {
            // Arrange
            var email = "joao@email.com";
            var password = "SenhaForte@123";

            // Act
            Action act = () => new User("", email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Name is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando o nome excede o comprimento máximo de 100 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithNameExceedingMaxLength_ShouldThrowValidationException()
        {
            // Arrange
            var name = new string('A', 101);
            var email = "joao@email.com";
            var password = "SenhaForte@123";

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Name must be at most 100 characters*");
        }

        #endregion

        #region Validation Tests - Email

        /// <summary>
        /// Testa se o construtor lança exceção quando o email está vazio.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyEmail_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var password = "SenhaForte@123";

            // Act
            Action act = () => new User(name, "", password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Email is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando o email está em formato inválido.
        /// </summary>
        [Fact]
        public void Constructor_WithInvalidEmail_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = "email-invalido";
            var password = "SenhaForte@123";

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Email is invalid*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando o email excede o comprimento máximo de 100 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithEmailExceedingMaxLength_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = new string('a', 91) + "@email.com"; // 101 caracteres
            var password = "SenhaForte@123";

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Email must be at most 100 characters*");
        }

        #endregion

        #region Validation Tests - Password

        /// <summary>
        /// Testa se o construtor lança exceção quando a senha está vazia.
        /// </summary>
        [Fact]
        public void Constructor_WithEmptyPassword_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@email.com";

            // Act
            Action act = () => new User(name, email, "");

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Password is required*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando a senha tem menos de 8 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithShortPassword_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@email.com";
            var password = "Abc@123"; // 7 caracteres

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Password must be between 8 and 25 characters*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando a senha excede 25 caracteres.
        /// </summary>
        [Fact]
        public void Constructor_WithLongPassword_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@email.com";
            var password = "SenhaForte@" + new string('1', 20); // 31 caracteres

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Password must be between 8 and 25 characters*");
        }

        /// <summary>
        /// Testa se o construtor lança exceção quando a senha não atende aos requisitos de força.
        /// A senha deve conter maiúsculas, minúsculas, números e caracteres especiais.
        /// </summary>
        [Fact]
        public void Constructor_WithWeakPassword_ShouldThrowValidationException()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@email.com";
            var password = "senhafraca123"; // Sem maiúsculas e caracteres especiais

            // Act
            Action act = () => new User(name, email, password);

            // Assert
            act.Should().Throw<DomainValidationException>()
                .WithMessage("*Password must contain uppercase, lowercase, numbers and special characters*");
        }

        #endregion

        #region Update Tests

        /// <summary>
        /// Testa se o método Update atualiza corretamente os dados do usuário com dados válidos.
        /// Verifica se nome, email, estado e senha (hasheada) são atualizados.
        /// </summary>
        [Fact]
        public void Update_WithValidData_ShouldUpdateUser()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123");
            var newName = "João Pedro Silva";
            var newEmail = "joao.pedro@email.com";
            var newPassword = "NovaSenha@456";

            // Act
            user.Update(newName, newEmail, newPassword, UserEnum.State.Inactive);

            // Assert
            user.Name.Should().Be("João Pedro Silva");
            user.Email.Should().Be("joao.pedro@email.com");
            user.State.Should().Be(UserEnum.State.Inactive);
            user.Password.Should().NotBe(newPassword); // Deve estar hasheada
        }

        /// <summary>
        /// Testa se o método Update lança exceção quando dados inválidos são fornecidos.
        /// </summary>
        [Fact]
        public void Update_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123");

            // Act
            Action act = () => user.Update("", "invalid-email", "weak", UserEnum.State.Active);

            // Assert
            act.Should().Throw<DomainValidationException>();
        }

        #endregion

        #region Activate/Deactivate Tests

        /// <summary>
        /// Testa se o método Activate altera o estado do usuário para Active.
        /// Verifica também se UpdatedAt é atualizado.
        /// </summary>
        [Fact]
        public void Activate_ShouldSetStateToActive()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123", UserEnum.State.Inactive);

            // Act
            user.Activate();

            // Assert
            user.State.Should().Be(UserEnum.State.Active);
            user.UpdatedAt.Should().NotBeNull();
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Testa se o método Deactivate altera o estado do usuário para Inactive.
        /// Verifica também se UpdatedAt é atualizado.
        /// </summary>
        [Fact]
        public void Deactivate_ShouldSetStateToInactive()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123");

            // Act
            user.Deactivate();

            // Assert
            user.State.Should().Be(UserEnum.State.Inactive);
            user.UpdatedAt.Should().NotBeNull();
            user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        #endregion

        #region VerifyPassword Tests

        /// <summary>
        /// Testa se o método VerifyPassword retorna true quando a senha correta é fornecida.
        /// </summary>
        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            // Arrange
            var password = "SenhaForte@123";
            var user = new User("João Silva", "joao@email.com", password);

            // Act
            var result = user.VerifyPassword(password);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Testa se o método VerifyPassword retorna false quando uma senha incorreta é fornecida.
        /// </summary>
        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            // Arrange
            var password = "SenhaForte@123";
            var user = new User("João Silva", "joao@email.com", password);

            // Act
            var result = user.VerifyPassword("SenhaErrada@456");

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Testa se o método VerifyPassword retorna false quando uma senha vazia é fornecida.
        /// </summary>
        [Fact]
        public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123");

            // Act
            var result = user.VerifyPassword("");

            // Assert
            result.Should().BeFalse();
        }

        /// <summary>
        /// Testa se o método VerifyPassword retorna false quando uma senha null é fornecida.
        /// </summary>
        [Fact]
        public void VerifyPassword_WithNullPassword_ShouldReturnFalse()
        {
            // Arrange
            var user = new User("João Silva", "joao@email.com", "SenhaForte@123");

            // Act
            var result = user.VerifyPassword(null!);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
