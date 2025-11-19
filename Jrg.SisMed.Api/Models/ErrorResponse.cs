using System.Net;

namespace Jrg.SisMed.Api.Models
{
    /// <summary>
    /// Modelo de resposta de erro padronizado para a API.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Código de status HTTP.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Mensagem principal do erro.
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Lista de erros detalhados (para validações múltiplas).
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }

        /// <summary>
        /// Detalhes técnicos do erro (apenas em ambiente de desenvolvimento).
        /// </summary>
        public string? Details { get; set; }

        /// <summary>
        /// Timestamp de quando o erro ocorreu.
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Caminho da requisição que gerou o erro.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Cria uma resposta de erro para validações do domínio.
        /// </summary>
        public static ErrorResponse CreateValidationError(IEnumerable<string> errors, string path)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors,
                Path = path
            };
        }

        /// <summary>
        /// Cria uma resposta de erro para recursos não encontrados.
        /// </summary>
        public static ErrorResponse CreateNotFoundError(string message, string path)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = message,
                Path = path
            };
        }

        /// <summary>
        /// Cria uma resposta de erro para conflitos (ex: recurso já existe).
        /// </summary>
        public static ErrorResponse CreateConflictError(string message, string path)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.Conflict,
                Message = message,
                Path = path
            };
        }

        /// <summary>
        /// Cria uma resposta de erro para argumentos inválidos.
        /// </summary>
        public static ErrorResponse CreateBadRequestError(string message, string path)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = message,
                Path = path
            };
        }

        /// <summary>
        /// Cria uma resposta de erro interno do servidor.
        /// </summary>
        public static ErrorResponse CreateInternalServerError(string message, string path, string? details = null)
        {
            return new ErrorResponse
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                Message = message,
                Details = details,
                Path = path
            };
        }
    }
}
