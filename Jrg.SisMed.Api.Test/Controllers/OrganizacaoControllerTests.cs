using FluentAssertions;
using Jrg.SisMed.Api.Controllers;
using Jrg.SisMed.Application.DTOs.OrganizationDto;
using Jrg.SisMed.Application.UseCases.Organization;
using Jrg.SisMed.Domain.Entities;
using Jrg.SisMed.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Jrg.SisMed.Api.Test.Controllers
{
    /// <summary>
    /// Classe de testes unitários para o OrganizacaoController.
    /// Testa todos os endpoints HTTP (GET, POST, PUT, DELETE) e seus possíveis cenários.
    /// </summary>
    public class OrganizacaoControllerTests
    {
        private readonly Mock<CreateOrganizationUseCase> _mockCreateUseCase;
        private readonly Mock<UpdateOrganizationUseCase> _mockUpdateUseCase;
        private readonly Mock<ReadOrganizationUseCase> _mockReadUseCase;
        private readonly Mock<DeleteOrganizationUseCase> _mockDeleteUseCase;
        private readonly OrganizacaoController _controller;

        public OrganizacaoControllerTests()
        {
            // Cria mocks com MockBehavior.Loose (permite chamadas não configuradas)
            _mockCreateUseCase = new Mock<CreateOrganizationUseCase>(null!, null!);
            _mockUpdateUseCase = new Mock<UpdateOrganizationUseCase>(null!, null!);
            _mockReadUseCase = new Mock<ReadOrganizationUseCase>(null!);
            _mockDeleteUseCase = new Mock<DeleteOrganizationUseCase>(null!, null!);

            _controller = new OrganizacaoController(
                _mockCreateUseCase.Object,
                _mockUpdateUseCase.Object,
                _mockReadUseCase.Object,
                _mockDeleteUseCase.Object
            );
        }

        #region Get (GetAll) Tests

        /// <summary>
        /// Testa se Get retorna 200 OK com lista de organizações quando existem organizações.
        /// </summary>
        [Fact]
        public async Task Get_WithExistingOrganizations_ShouldReturn200OK()
        {
            // Arrange
            var organizations = new List<ReadOrganizationDto>
            {
                new() 
                { 
                    Id = 1, 
                    NameFantasia = "Clínica Saúde", 
                    RazaoSocial = "Clínica Saúde Ltda", 
                    Cnpj = "11222333000181",
                    State = OrganizationEnum.State.Active
                },
                new() 
                { 
                    Id = 2, 
                    NameFantasia = "Hospital Vida", 
                    RazaoSocial = "Hospital Vida S/A", 
                    Cnpj = "12345678000195",
                    State = OrganizationEnum.State.Active
                }
            };

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(organizations);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(organizations);

            _mockReadUseCase.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Get retorna 404 Not Found quando não há organizações.
        /// </summary>
        [Fact]
        public async Task Get_WithNoOrganizations_ShouldReturn404NotFound()
        {
            // Arrange
            var emptyList = new List<ReadOrganizationDto>();

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.Get(CancellationToken.None);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult!.Value.Should().BeEquivalentTo(new { message = "No organizations found." });

            _mockReadUseCase.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Get propaga exceções lançadas pelo use case.
        /// </summary>
        [Fact]
        public async Task Get_WhenUseCaseThrowsException_ShouldPropagateException()
        {
            // Arrange
            _mockReadUseCase
                .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            Func<Task> act = async () => await _controller.Get(CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("Database error");
        }

        #endregion

        #region Get by ID Tests

        /// <summary>
        /// Testa se Get(id) retorna 200 OK com a organização quando encontrada.
        /// </summary>
        [Fact]
        public async Task GetById_WithExistingOrganization_ShouldReturn200OK()
        {
            // Arrange
            var organizationId = 1;
            var organization = new ReadOrganizationDto
            {
                Id = organizationId,
                NameFantasia = "Clínica Saúde",
                RazaoSocial = "Clínica Saúde Ltda",
                Cnpj = "11222333000181",
                State = OrganizationEnum.State.Active
            };

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(organization);

            // Act
            var result = await _controller.Get(organizationId, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(organization);

            _mockReadUseCase.Verify(x => x.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Get(id) lança NotFoundException quando organização não existe.
        /// </summary>
        [Fact]
        public async Task GetById_WithNonExistingOrganization_ShouldThrowNotFoundException()
        {
            // Arrange
            var organizationId = 999;

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Organization", organizationId));

            // Act
            Func<Task> act = async () => await _controller.Get(organizationId, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>()
                .WithMessage("Organization with identifier '999' was not found.");
        }

        /// <summary>
        /// Testa se Get(id) funciona com diferentes IDs válidos.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(25)]
        [InlineData(100)]
        [InlineData(500)]
        public async Task GetById_WithDifferentValidIds_ShouldCallUseCaseWithCorrectId(int organizationId)
        {
            // Arrange
            var organization = new ReadOrganizationDto 
            { 
                Id = organizationId, 
                NameFantasia = "Test", 
                RazaoSocial = "Test Ltda",
                Cnpj = "11222333000181"
            };

            _mockReadUseCase
                .Setup(x => x.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(organization);

            // Act
            await _controller.Get(organizationId, CancellationToken.None);

            // Assert
            _mockReadUseCase.Verify(x => x.GetByIdAsync(organizationId, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Search by CNPJ Tests

        /// <summary>
        /// Testa se Search retorna 200 OK com a organização quando encontrada por CNPJ.
        /// </summary>
        [Fact]
        public async Task Search_WithExistingOrganization_ShouldReturn200OK()
        {
            // Arrange
            var cnpj = "11.222.333/0001-81";
            var organization = new ReadOrganizationDto
            {
                Id = 1,
                NameFantasia = "Clínica Saúde",
                RazaoSocial = "Clínica Saúde Ltda",
                Cnpj = "11222333000181",
                State = OrganizationEnum.State.Active
            };

            _mockReadUseCase
                .Setup(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(organization);

            // Act
            var result = await _controller.Search(cnpj, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(organization);

            _mockReadUseCase.Verify(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Search retorna 400 Bad Request quando CNPJ está vazio.
        /// </summary>
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async Task Search_WithEmptyCnpj_ShouldReturn400BadRequest(string? cnpj)
        {
            // Act
            var result = await _controller.Search(cnpj!, CancellationToken.None);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult!.Value.Should().BeEquivalentTo(new { message = "CNPJ is required" });

            _mockReadUseCase.Verify(x => x.GetByCnpjAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Testa se Search lança NotFoundException quando organização não existe.
        /// </summary>
        [Fact]
        public async Task Search_WithNonExistingOrganization_ShouldThrowNotFoundException()
        {
            // Arrange
            var cnpj = "12.345.678/0001-95";

            _mockReadUseCase
                .Setup(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Organization", cnpj));

            // Act
            Func<Task> act = async () => await _controller.Search(cnpj, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se Search lança ArgumentException quando CNPJ é inválido.
        /// </summary>
        [Theory]
        [InlineData("11111111111111")]  // CNPJ inválido (todos iguais)
        [InlineData("123")]  // CNPJ curto
        [InlineData("invalid-cnpj")]  // CNPJ com caracteres inválidos
        public async Task Search_WithInvalidCnpj_ShouldThrowArgumentException(string cnpj)
        {
            // Arrange
            _mockReadUseCase
                .Setup(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentException("CNPJ is invalid", nameof(cnpj)));

            // Act
            Func<Task> act = async () => await _controller.Search(cnpj, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*CNPJ is invalid*");
        }

        /// <summary>
        /// Testa se Search aceita CNPJ com ou sem formatação.
        /// </summary>
        [Theory]
        [InlineData("11.222.333/0001-81")]
        [InlineData("11222333000181")]
        public async Task Search_WithDifferentCnpjFormats_ShouldAcceptBothFormats(string cnpj)
        {
            // Arrange
            var organization = new ReadOrganizationDto
            {
                Id = 1,
                NameFantasia = "Test",
                RazaoSocial = "Test Ltda",
                Cnpj = "11222333000181"
            };

            _mockReadUseCase
                .Setup(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()))
                .ReturnsAsync(organization);

            // Act
            var result = await _controller.Search(cnpj, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockReadUseCase.Verify(x => x.GetByCnpjAsync(cnpj, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Post (Create) Tests

        /// <summary>
        /// Testa se Post retorna 201 Created com o ID quando organização é criada com sucesso.
        /// </summary>
        [Fact]
        public async Task Post_WithValidData_ShouldReturn201Created()
        {
            // Arrange
            var createDto = new CreateOrganizationDto
            {
                NameFantasia = "Clínica Saúde",
                RazaoSocial = "Clínica Saúde Ltda",
                Cnpj = "11.222.333/0001-81",
                State = OrganizationEnum.State.Active
            };
            var organizationId = 1;

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(organizationId);

            // Act
            var result = await _controller.Post(createDto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result as CreatedAtActionResult;
            createdResult!.ActionName.Should().Be(nameof(OrganizacaoController.Get));
            createdResult.RouteValues!["id"].Should().Be(organizationId);
            createdResult.Value.Should().BeEquivalentTo(new { id = organizationId, message = "Organization created successfully" });

            _mockCreateUseCase.Verify(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Post lança DomainValidationException quando dados são inválidos.
        /// </summary>
        [Fact]
        public async Task Post_WithInvalidData_ShouldThrowDomainValidationException()
        {
            // Arrange
            var createDto = new CreateOrganizationDto
            {
                NameFantasia = "",  // Inválido
                RazaoSocial = "",  // Inválido
                Cnpj = "11111111111111",  // CNPJ inválido
                State = OrganizationEnum.State.Active
            };

            var errors = new List<string> 
            { 
                "Trade name is required", 
                "Legal name is required", 
                "CNPJ is invalid" 
            };

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DomainValidationException(errors));

            // Act
            Func<Task> act = async () => await _controller.Post(createDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>()
                .Where(ex => ex.Errors.Count == 3);
        }

        /// <summary>
        /// Testa se Post lança ConflictException quando CNPJ já existe.
        /// </summary>
        [Fact]
        public async Task Post_WithDuplicateCnpj_ShouldThrowConflictException()
        {
            // Arrange
            var createDto = new CreateOrganizationDto
            {
                NameFantasia = "Clínica Teste",
                RazaoSocial = "Clínica Teste Ltda",
                Cnpj = "11.222.333/0001-81",
                State = OrganizationEnum.State.Active
            };

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("Organization", "CNPJ", "11222333000181"));

            // Act
            Func<Task> act = async () => await _controller.Post(createDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Organization with CNPJ '11222333000181' already exists.");
        }

        /// <summary>
        /// Testa se Post lança ConflictException quando Razão Social já existe.
        /// </summary>
        [Fact]
        public async Task Post_WithDuplicateRazaoSocial_ShouldThrowConflictException()
        {
            // Arrange
            var createDto = new CreateOrganizationDto
            {
                NameFantasia = "Clínica Nova",
                RazaoSocial = "Clínica Existente Ltda",
                Cnpj = "12.345.678/0001-95",
                State = OrganizationEnum.State.Active
            };

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ConflictException("Organization", "RazaoSocial", createDto.RazaoSocial));

            // Act
            Func<Task> act = async () => await _controller.Post(createDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ConflictException>()
                .WithMessage("Organization with RazaoSocial 'Clínica Existente Ltda' already exists.");
        }

        /// <summary>
        /// Testa se Post lança ArgumentNullException quando DTO é null.
        /// </summary>
        [Fact]
        public async Task Post_WithNullDto_ShouldThrowArgumentNullException()
        {
            // Arrange
            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(null!, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ArgumentNullException("createOrganizationDto"));

            // Act
            Func<Task> act = async () => await _controller.Post(null!, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        #endregion

        #region Put (Update) Tests

        /// <summary>
        /// Testa se Put retorna 200 OK quando organização é atualizada com sucesso.
        /// </summary>
        [Fact]
        public async Task Put_WithValidData_ShouldReturn200OK()
        {
            // Arrange
            var organizationId = 1;
            var updateDto = new UpdateOrganizationDto
            {
                NameFantasia = "Clínica Saúde Atualizada",
                RazaoSocial = "Clínica Saúde Ltda",
                Cnpj = "11.222.333/0001-81",
                State = OrganizationEnum.State.Active
            };

            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Put(organizationId, updateDto, CancellationToken.None);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(new { message = "Organization updated successfully" });

            _mockUpdateUseCase.Verify(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Testa se Put lança NotFoundException quando organização não existe.
        /// </summary>
        [Fact]
        public async Task Put_WithNonExistingOrganization_ShouldThrowNotFoundException()
        {
            // Arrange
            var organizationId = 999;
            var updateDto = new UpdateOrganizationDto
            {
                NameFantasia = "Test",
                RazaoSocial = "Test Ltda",
                Cnpj = "11.222.333/0001-81",
                State = OrganizationEnum.State.Active
            };

            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotFoundException("Organization", organizationId));

            // Act
            Func<Task> act = async () => await _controller.Put(organizationId, updateDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se Put lança DomainValidationException quando dados são inválidos.
        /// </summary>
        [Fact]
        public async Task Put_WithInvalidData_ShouldThrowDomainValidationException()
        {
            // Arrange
            var organizationId = 1;
            var updateDto = new UpdateOrganizationDto
            {
                NameFantasia = "",
                RazaoSocial = "",
                Cnpj = "invalid",
                State = OrganizationEnum.State.Active
            };

            var errors = new List<string> { "Trade name is required", "Legal name is required", "CNPJ is invalid" };
            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DomainValidationException(errors));

            // Act
            Func<Task> act = async () => await _controller.Put(organizationId, updateDto, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainValidationException>();
        }

        /// <summary>
        /// Testa se Put permite atualizar para diferentes estados.
        /// </summary>
        [Theory]
        [InlineData(OrganizationEnum.State.Active)]
        [InlineData(OrganizationEnum.State.Inactive)]
        [InlineData(OrganizationEnum.State.Suspended)]
        public async Task Put_WithDifferentStates_ShouldAcceptAllValidStates(OrganizationEnum.State state)
        {
            // Arrange
            var organizationId = 1;
            var updateDto = new UpdateOrganizationDto
            {
                NameFantasia = "Test",
                RazaoSocial = "Test Ltda",
                Cnpj = "11.222.333/0001-81",
                State = state
            };

            _mockUpdateUseCase
                .Setup(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.Put(organizationId, updateDto, CancellationToken.None);

            // Assert
            _mockUpdateUseCase.Verify(x => x.ExecuteAsync(organizationId, updateDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Delete Tests

        /// <summary>
        /// Testa se Delete retorna 204 No Content quando organização é deletada com sucesso.
        /// </summary>
        [Fact]
        public async Task Delete_WithExistingOrganization_ShouldReturn204NoContent()
        {
            // Arrange
            var organizationId = 1;

            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(organizationId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(organizationId);

            // Assert
            result.Should().BeOfType<NoContentResult>();

            _mockDeleteUseCase.Verify(x => x.ExecuteAsync(organizationId), Times.Once);
        }

        /// <summary>
        /// Testa se Delete lança NotFoundException quando organização não existe.
        /// </summary>
        [Fact]
        public async Task Delete_WithNonExistingOrganization_ShouldThrowNotFoundException()
        {
            // Arrange
            var organizationId = 999;

            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(organizationId))
                .ThrowsAsync(new NotFoundException("Organization", organizationId));

            // Act
            Func<Task> act = async () => await _controller.Delete(organizationId);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>();
        }

        /// <summary>
        /// Testa se Delete funciona com diferentes IDs válidos.
        /// </summary>
        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task Delete_WithDifferentValidIds_ShouldCallUseCaseWithCorrectId(int organizationId)
        {
            // Arrange
            _mockDeleteUseCase
                .Setup(x => x.ExecuteAsync(organizationId))
                .Returns(Task.CompletedTask);

            // Act
            await _controller.Delete(organizationId);

            // Assert
            _mockDeleteUseCase.Verify(x => x.ExecuteAsync(organizationId), Times.Once);
        }

        #endregion

        #region CancellationToken Tests

        /// <summary>
        /// Testa se Get propaga o CancellationToken corretamente.
        /// </summary>
        [Fact]
        public async Task Get_ShouldPropagateCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var organizations = new List<ReadOrganizationDto> 
            { 
                new() { Id = 1, NameFantasia = "Test", RazaoSocial = "Test Ltda", Cnpj = "11222333000181" } 
            };

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(cts.Token))
                .ReturnsAsync(organizations);

            // Act
            await _controller.Get(cts.Token);

            // Assert
            _mockReadUseCase.Verify(x => x.GetAllAsync(cts.Token), Times.Once);
        }

        /// <summary>
        /// Testa se operações são canceladas quando CancellationToken é acionado.
        /// </summary>
        [Fact]
        public async Task Get_WhenCancelled_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            cts.Cancel();

            _mockReadUseCase
                .Setup(x => x.GetAllAsync(cts.Token))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            Func<Task> act = async () => await _controller.Get(cts.Token);

            // Assert
            await act.Should().ThrowAsync<OperationCanceledException>();
        }

        /// <summary>
        /// Testa se Post propaga o CancellationToken.
        /// </summary>
        [Fact]
        public async Task Post_ShouldPropagateCancellationToken()
        {
            // Arrange
            var cts = new CancellationTokenSource();
            var createDto = new CreateOrganizationDto
            {
                NameFantasia = "Test",
                RazaoSocial = "Test Ltda",
                Cnpj = "11.222.333/0001-81",
                State = OrganizationEnum.State.Active
            };

            _mockCreateUseCase
                .Setup(x => x.ExecuteAsync(createDto, cts.Token))
                .ReturnsAsync(1);

            // Act
            await _controller.Post(createDto, cts.Token);

            // Assert
            _mockCreateUseCase.Verify(x => x.ExecuteAsync(createDto, cts.Token), Times.Once);
        }

        #endregion
    }
}
