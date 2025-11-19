using FluentAssertions;
using Jrg.SisMed.Api.Controllers;
using Jrg.SisMed.Application.DTOs.UserDto;
using Jrg.SisMed.Application.UseCases.User;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Jrg.SisMed.Api.Test.Controllers
{
    /// <summary>
    /// Classe de testes unitários para o UserController.
    /// Testa todos os endpoints HTTP (GET, POST, PUT, DELETE) e seus possíveis cenários.
    /// </summary>
    public class UserControllerTests
    {
        private readonly Mock<CreateUserUseCase> _mockCreateUseCase;
        private readonly Mock<UpdateUserUseCase> _mockUpdateUseCase;
        private readonly Mock<ReadUserUseCase> _mockReadUseCase;
        private readonly Mock<DeleteUserUseCase> _mockDeleteUseCase;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            // Cria mocks com MockBehavior.Loose (permite chamadas não configuradas)
            _mockCreateUseCase = new Mock<CreateUserUseCase>(null!, null!);
            _mockUpdateUseCase = new Mock<UpdateUserUseCase>(null!, null!);
            _mockReadUseCase = new Mock<ReadUserUseCase>(null!);
            _mockDeleteUseCase = new Mock<DeleteUserUseCase>(null!, null!);

            _controller = new UserController(
                _mockCreateUseCase.Object,
                _mockUpdateUseCase.Object,
                _mockReadUseCase.Object,
                _mockDeleteUseCase.Object
            );
        }

        #region GetAll Tests

        /// <summary>
        /// Testa se GetAll retorna 200 OK com lista de usuários quando existem usuários.
        /// </summary>
        [Fact]
        public async Task GetAll_WithExistingUsers_ShouldReturn200OK()
        {
            // Arrange
            var users = new List<ReadUserDto>
            {
                new() { Id = 1, Name = "João Silva", Email = "joao@email.com", State = UserEnum.State.Active },
                new() { Id = 2, Name = "Maria Santos", Email = "maria@email.com", State = UserEnum.State.Active }
            };

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(users);

            _mockReadUseCase.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se GetAll retorna 404 Not Found quando não há usuários.
        /// </summary>
        [Fact]
        public async Task GetAll_WithNoUsers_ShouldReturn404NotFound()
        {
            // Arrange
            var emptyList = new List<ReadUserDto>();

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetAll(CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().BeEquivalentTo(new { message = "No users found." });

            _mockReadUseCase.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se GetAll propaga exceções lançadas pelo use case.
        /// </summary>
        [Fact]
        public async Task GetAll_WhenUseCaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            Func<Task> act = async () => await _controller.GetAll(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        #endregion

        #region GetById Tests

        /// <summary>
        /// Testa se GetById retorna 200 OK com o usuário quando encontrado.
        /// </summary>
        [Fact]
        public async Task GetById_WithExistingUser_ShouldReturn200OK()
        {
            // Arrange
            var userId = 1;
            var user = new ReadUserDto 
            { 
                Id = userId, 
                Name = "João Silva", 
                Email = "joao@email.com", 
                State = UserEnum.State.Active 
            };

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.GetById(userId, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(user);

            _mockReadUseCase.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se GetById lança NotFoundException quando usuário não existe.
        /// O middleware irá capturar e retornar 404.
        /// </summary>
        [Fact]
        public async Task GetById_WithNonExistingUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = 999;

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("User", userId));

            // Act
            Func<Task> act = async () => await _controller.GetById(userId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("User with identifier '999' was not found.");
        }

        /// <summary>
        /// Testa se GetById funciona com diferentes IDs válidos.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task GetById_WithDifferentValidIds_ShouldCallUseCaseWithCorrectId(int userId)
        {
            // Arrange
            var user = new ReadUserDto { Id = userId, Name = "Test User", Email = "test@email.com" };

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            await _controller.GetById(userId, CancellationToken.None);

            // Assert
            _mockReadUseCase.Verify(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region SearchByEmail Tests

        /// <summary>
        /// Testa se SearchByEmail retorna 200 OK com o usuário quando encontrado.
        /// </summary>
        [Fact]
        public async Task SearchByEmail_WithExistingUser_ShouldReturn200OK()
        {
            // Arrange
            var email = "joao@email.com";
            var user = new ReadUserDto 
            { 
                Id = 1, 
                Name = "João Silva", 
                Email = email, 
                State = UserEnum.State.Active 
            };

            _mockReadUseCase
                .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.SearchByEmail(email, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(user);

            _mockReadUseCase.Verify(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se SearchByEmail retorna 400 Bad Request quando email está vazio.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task SearchByEmail_WithEmptyEmail_ShouldReturn400BadRequest(string? email)
        {
            // Act
            var result = await _controller.SearchByEmail(email!, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "Email is required" });

            _mockReadUseCase.Verify(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Testa se SearchByEmail lança NotFoundException quando usuário não existe.
        /// </summary>
        [Fact]
        public async Task SearchByEmail_WithNonExistingUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var email = "notfound@email.com";

            _mockReadUseCase
                .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("User", email));

            // Act
            Func<Task> act = async () => await _controller.SearchByEmail(email, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se SearchByEmail lança ArgumentException quando email é inválido.
        /// </summary>
        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@email.com")]
        [InlineData("user@")]
        public async Task SearchByEmail_WithInvalidEmail_ShouldThrowArgumentException(string email)
        {
            // Arrange
            _mockReadUseCase
                .Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("Email is invalid", nameof(email)));

            // Act
            Func<Task> act = async () => await _controller.SearchByEmail(email, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email is invalid*");
        }

        #endregion

        #region Create Tests

        /// <summary>
        /// Testa se Create retorna 201 Created com o ID quando usuário é criado com sucesso.
        /// </summary>
        [Fact]
        public async Task Create_WithValidData_ShouldReturn201Created()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Name = "João Silva",
                Email = "joao@email.com",
                Password = "SenhaForte@123",
                State = UserEnum.State.Active
            };
            var userId = 1;

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(userId);

            // Act
            var result = await _controller.Create(createDto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(UserController.GetById));
            createdResult.RouteValues!["id"].Should().Be(userId);
            createdResult.Value.Should().BeEquivalentTo(new { id = userId, message = "User created successfully" });

            _mockCreateUseCase.Verify(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Create lança DomainValidationException quando dados são inválidos.
        /// </summary>
        [Fact]
        public async Task Create_WithInvalidData_ShouldThrowDomainValidationException()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Name = "",  // Inválido
                Email = "invalid-email",  // Inválido
                Password = "weak",  // Inválido
                State = UserEnum.State.Active
            };

            var errors = new List<string> { "Name is required", "Email is invalid", "Password must be between 8 and 25 characters" };
            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DomainValidationException(errors));

            // Act
            Func<Task> act = async () => await _controller.Create(createDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>()
                .Where(ex => ex.Errors.Count == 3);
        }

        /// <summary>
        /// Testa se Create lança ConflictException quando email já existe.
        /// </summary>
        [Fact]
        public async Task Create_WithDuplicateEmail_ShouldThrowConflictException()
        {
            // Arrange
            var createDto = new CreateUserDto
            {
                Name = "João Silva",
                Email = "existing@email.com",
                Password = "SenhaForte@123",
                State = UserEnum.State.Active
            };

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("User", "Email", createDto.Email));

            // Act
            Func<Task> act = async () => await _controller.Create(createDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("User with Email 'existing@email.com' already exists.");
        }

        /// <summary>
        /// Testa se Create lança ArgumentNullException quando DTO é null.
        /// </summary>
        [Fact]
        public async Task Create_WithNullDto_ShouldThrowArgumentNullException()
        {
            // Arrange
            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(null!, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException("createUserDto"));

            // Act
            Func<Task> act = async () => await _controller.Create(null!, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Update Tests

        /// <summary>
        /// Testa se Update retorna 200 OK quando usuário é atualizado com sucesso.
        /// </summary>
        [Fact]
        public async Task Update_WithValidData_ShouldReturn200OK()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UpdateUserDto
            {
                Name = "João Silva Atualizado",
                Email = "joao.updated@email.com",
                Password = "NovaSenha@456",
                State = UserEnum.State.Active
            };

            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(userId, updateDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update(userId, updateDto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "User updated successfully" });

            _mockUpdateUseCase.Verify(x => x.ExecuteAsync(userId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Update lança NotFoundException quando usuário não existe.
        /// </summary>
        [Fact]
        public async Task Update_WithNonExistingUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = 999;
            var updateDto = new UpdateUserDto
            {
                Name = "Test User",
                Email = "test@email.com",
                Password = "Password@123",
                State = UserEnum.State.Active
            };

            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(userId, updateDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("User", userId));

            // Act
            Func<Task> act = async () => await _controller.Update(userId, updateDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se Update lança DomainValidationException quando dados são inválidos.
        /// </summary>
        [Fact]
        public async Task Update_WithInvalidData_ShouldThrowDomainValidationException()
        {
            // Arrange
            var userId = 1;
            var updateDto = new UpdateUserDto
            {
                Name = "",
                Email = "invalid",
                Password = "weak",
                State = UserEnum.State.Active
            };

            var errors = new List<string> { "Name is required", "Email is invalid" };
            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(userId, updateDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DomainValidationException(errors));

            // Act
            Func<Task> act = async () => await _controller.Update(userId, updateDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>();
        }

        #endregion

        #region Delete Tests

        /// <summary>
        /// Testa se Delete retorna 204 No Content quando usuário é deletado com sucesso.
        /// </summary>
        [Fact]
        public async Task Delete_WithExistingUser_ShouldReturn204NoContent()
        {
            // Arrange
            var userId = 1;

            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _mockDeleteUseCase.Verify(x => x.ExecuteAsync(userId), Times.Once);
        }

        /// <summary>
        /// Testa se Delete lança NotFoundException quando usuário não existe.
        /// </summary>
        [Fact]
        public async Task Delete_WithNonExistingUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = 999;

            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(userId))
                .ThrowsAsync(new NotFoundException("User", userId));

            // Act
            Func<Task> act = async () => await _controller.Delete(userId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se Delete funciona com diferentes IDs válidos.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(100)]
        public async Task Delete_WithDifferentValidIds_ShouldCallUseCaseWithCorrectId(int userId)
        {
            // Arrange
            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(userId))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.Delete(userId);

            // Assert
            _mockDeleteUseCase.Verify(x => x.ExecuteAsync(userId), Times.Once);
        }

        #endregion

        #region CancellationToken Tests

        /// <summary>
        /// Testa se GetAll propaga o CancellationToken corretamente.
        /// </summary>
        [Fact]
        public async Task GetAll_ShouldPropagateCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var users = new List<ReadUserDto> { new() { Id = 1, Name = "Test", Email = "test@email.com" } };

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(cts.Token))
                .ReturnsAsync(users);

            // Act
            await _controller.GetAll(cts.Token);

            // Assert
            _mockReadUseCase.Verify(x => x.GetAllAsync(cts.Token), Times.Once);
        }

        /// <summary>
        /// Testa se operações são canceladas quando CancellationToken é acionado.
        /// </summary>
        [Fact]
        public async Task GetAll_WhenCancelled_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(cts.Token))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> act = async () => await _controller.GetAll(cts.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        #endregion
    }
}
