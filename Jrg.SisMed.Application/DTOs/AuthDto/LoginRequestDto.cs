using System.ComponentModel.DataAnnotations;

namespace Jrg.SisMed.Application.DTOs.AuthDto
{
    /// <summary>
    /// DTO para requisição de login.
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        /// <example>joao@exemplo.com</example>
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Senha do usuário.
        /// </summary>
        /// <example>SenhaForte@123</example>
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(8, ErrorMessage = "A senha deve ter no mínimo 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO para resposta de login bem-sucedido.
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// Token JWT gerado.
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do token (sempre "Bearer").
        /// </summary>
        public string TokenType { get; set; } = "Bearer";

        /// <summary>
        /// Tempo de expiração do token em minutos.
        /// </summary>
        public int ExpiresIn { get; set; }

        /// <summary>
        /// Dados do usuário autenticado.
        /// </summary>
        public UserInfoDto User { get; set; } = new();
    }

    /// <summary>
    /// Informações básicas do usuário autenticado.
    /// </summary>
    public class UserInfoDto
    {
        /// <summary>
        /// ID do usuário.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome do usuário.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// E-mail do usuário.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Estado do usuário (Active, Inactive, Blocked).
        /// </summary>
        public string State { get; set; } = string.Empty;
    }
}
