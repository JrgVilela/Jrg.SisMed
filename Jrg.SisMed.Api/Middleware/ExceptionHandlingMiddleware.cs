using Jrg.SisMed.Api.Models;
using Jrg.SisMed.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Jrg.SisMed.Api.Middleware
{
    /// <summary>
    /// Middleware para tratamento global de exceções na API.
    /// Captura todas as exceções não tratadas e retorna respostas HTTP padronizadas.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var errorResponse = exception switch
            {
                DomainValidationException domainEx => HandleDomainValidationException(context, domainEx),
                NotFoundException notFoundEx => HandleNotFoundException(context, notFoundEx),
                ConflictException conflictEx => HandleConflictException(context, conflictEx),
                InvalidOperationException invalidOpEx => HandleInvalidOperationException(context, invalidOpEx),
                ArgumentNullException argNullEx => HandleArgumentNullException(context, argNullEx),
                ArgumentException argEx => HandleArgumentException(context, argEx),
                _ => HandleGenericException(context, exception)
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = _environment.IsDevelopment()
            };

            var json = JsonSerializer.Serialize(errorResponse, jsonOptions);
            await context.Response.WriteAsync(json);
        }

        /// <summary>
        /// Trata exceções de validação de domínio (regras de negócio).
        /// </summary>
        private ErrorResponse HandleDomainValidationException(HttpContext context, DomainValidationException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return ErrorResponse.CreateValidationError(
                exception.Errors,
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções de recurso não encontrado.
        /// </summary>
        private ErrorResponse HandleNotFoundException(HttpContext context, NotFoundException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            return ErrorResponse.CreateNotFoundError(
                exception.Message,
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções de conflito (recurso já existe).
        /// </summary>
        private ErrorResponse HandleConflictException(HttpContext context, ConflictException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            return ErrorResponse.CreateConflictError(
                exception.Message,
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções de operação inválida (ex: regras de negócio violadas).
        /// </summary>
        private ErrorResponse HandleInvalidOperationException(HttpContext context, InvalidOperationException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Conflict;

            return ErrorResponse.CreateConflictError(
                exception.Message,
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções de argumento inválido.
        /// </summary>
        private ErrorResponse HandleArgumentException(HttpContext context, ArgumentException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return ErrorResponse.CreateBadRequestError(
                exception.Message,
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções de argumento nulo.
        /// </summary>
        private ErrorResponse HandleArgumentNullException(HttpContext context, ArgumentNullException exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            return ErrorResponse.CreateBadRequestError(
                $"Required parameter '{exception.ParamName}' was not provided.",
                context.Request.Path
            );
        }

        /// <summary>
        /// Trata exceções genéricas não mapeadas.
        /// </summary>
        private ErrorResponse HandleGenericException(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var message = _environment.IsDevelopment()
                ? exception.Message
                : "An internal server error occurred. Please try again later.";

            var details = _environment.IsDevelopment()
                ? exception.StackTrace
                : null;

            return ErrorResponse.CreateInternalServerError(
                message,
                context.Request.Path,
                details
            );
        }
    }

    /// <summary>
    /// Classe de extensão para facilitar o registro do middleware.
    /// </summary>
    public static class ExceptionHandlingMiddlewareExtensions
    {
        /// <summary>
        /// Adiciona o middleware de tratamento de exceções ao pipeline da aplicação.
        /// </summary>
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
