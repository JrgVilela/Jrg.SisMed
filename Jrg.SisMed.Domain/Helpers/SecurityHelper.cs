using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Helpers
{
    /// <summary>
    /// Tipos de algoritmos de hash disponíveis.
    /// </summary>
    public enum HashAlgorithmType
    {
        /// <summary>
        /// MD5 - OBSOLETO: Use apenas para compatibilidade legada. NÃO use para senhas ou segurança.
        /// </summary>
        [Obsolete("MD5 is cryptographically broken and should not be used for security purposes. Use SHA256 or higher.")]
        MD5 = 0,

        /// <summary>
        /// SHA1 - OBSOLETO: Use apenas para compatibilidade legada. NÃO use para senhas ou segurança.
        /// </summary>
        [Obsolete("SHA1 is cryptographically broken and should not be used for security purposes. Use SHA256 or higher.")]
        SHA1 = 1,

        /// <summary>
        /// SHA256 - Algoritmo seguro recomendado para checksums e integridade de dados.
        /// </summary>
        SHA256 = 2,

        /// <summary>
        /// SHA384 - Algoritmo seguro com maior resistência a colisões.
        /// </summary>
        SHA384 = 3,

        /// <summary>
        /// SHA512 - Algoritmo mais seguro da família SHA-2.
        /// </summary>
        SHA512 = 4
    }

    /// <summary>
    /// Classe estática com métodos auxiliares para operações de segurança e criptografia.
    /// </summary>
    /// <remarks>
    /// <para><strong>AVISO DE SEGURANÇA:</strong></para>
    /// <para>- MD5 e SHA1 estão criptograficamente quebrados e NÃO devem ser usados para segurança.</para>
    /// <para>- Para senhas, use <see cref="HashPasswordPbkdf2"/> que implementa PBKDF2 com salt.</para>
    /// <para>- Para checksums e integridade, use SHA256 ou superior.</para>
    /// </remarks>
    public static class SecurityHelper
    {
        private const int DefaultPbkdf2Iterations = 100000; // OWASP recomenda 100k+ iterações
        private const int DefaultSaltSize = 16; // 128 bits
        private const int DefaultHashSize = 32; // 256 bits

        /// <summary>
        /// Gera um hash a partir de uma string usando o algoritmo especificado.
        /// </summary>
        /// <param name="input">Texto a ser transformado em hash.</param>
        /// <param name="hashType">Tipo de algoritmo de hash a ser utilizado (padrão: SHA256).</param>
        /// <returns>String hexadecimal representando o hash gerado.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando input é null.</exception>
        /// <exception cref="ArgumentException">Lançado quando input está vazio.</exception>
        /// <example>
        /// <code>
        /// string text = "Hello World";
        /// string hash = SecurityHelper.GenerateHash(text, HashAlgorithmType.SHA256);
        /// </code>
        /// </example>
        /// <remarks>
        /// <para><strong>IMPORTANTE:</strong> Este método NÃO deve ser usado para hash de senhas!</para>
        /// <para>Para senhas, use <see cref="HashPasswordPbkdf2"/> que é mais seguro.</para>
        /// </remarks>
        public static string GenerateHash(string input, HashAlgorithmType hashType = HashAlgorithmType.SHA256)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Length == 0)
                throw new ArgumentException("Input cannot be empty.", nameof(input));

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = ComputeHash(inputBytes, hashType);

            // .NET 5+ tem Convert.ToHexString que é ~3x mais rápido que StringBuilder
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>
        /// Computa o hash de bytes usando o algoritmo especificado.
        /// </summary>
        private static byte[] ComputeHash(byte[] input, HashAlgorithmType hashType)
        {
            return hashType switch
            {
#pragma warning disable SYSLIB0021 // MD5 is obsolete
                HashAlgorithmType.MD5 => MD5.HashData(input),
#pragma warning restore SYSLIB0021

#pragma warning disable SYSLIB0021 // SHA1 is obsolete
                HashAlgorithmType.SHA1 => SHA1.HashData(input),
#pragma warning restore SYSLIB0021

                HashAlgorithmType.SHA256 => SHA256.HashData(input),
                HashAlgorithmType.SHA384 => SHA384.HashData(input),
                HashAlgorithmType.SHA512 => SHA512.HashData(input),
                _ => throw new ArgumentException($"Unsupported hash type: {hashType}", nameof(hashType))
            };
        }

        /// <summary>
        /// Gera um hash seguro de senha usando PBKDF2 (Password-Based Key Derivation Function 2).
        /// </summary>
        /// <param name="password">Senha a ser hasheada.</param>
        /// <param name="iterations">Número de iterações (padrão: 100.000 conforme OWASP).</param>
        /// <returns>String no formato "$pbkdf2$iterations$salt$hash" para armazenamento seguro.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando password é null.</exception>
        /// <exception cref="ArgumentException">Lançado quando password está vazio ou iterations é menor que 10.000.</exception>
        /// <example>
        /// <code>
        /// string password = "MySecureP@ssw0rd!";
        /// string hashedPassword = SecurityHelper.HashPasswordPbkdf2(password);
        /// // Armazene hashedPassword no banco de dados
        /// 
        /// // Para verificar:
        /// bool isValid = SecurityHelper.VerifyPasswordPbkdf2(password, hashedPassword);
        /// </code>
        /// </example>
        /// <remarks>
        /// <para><strong>Este é o método RECOMENDADO para hash de senhas!</strong></para>
        /// <para>- Usa salt aleatório único para cada senha</para>
        /// <para>- Usa PBKDF2 com HMAC-SHA256</para>
        /// <para>- Iterações configuráveis (padrão: 100.000)</para>
        /// <para>- Segue recomendações OWASP</para>
        /// </remarks>
        public static string HashPasswordPbkdf2(string password, int iterations = DefaultPbkdf2Iterations)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

            if (iterations < 10000)
                throw new ArgumentException("Iterations must be at least 10,000 for security.", nameof(iterations));

            // Gera salt aleatório
            byte[] salt = RandomNumberGenerator.GetBytes(DefaultSaltSize);

            // Computa hash usando PBKDF2
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: Encoding.UTF8.GetBytes(password),
                salt: salt,
                iterations: iterations,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: DefaultHashSize);

            // Retorna no formato: $pbkdf2$iterations$salt$hash
            return $"$pbkdf2${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// Verifica se uma senha corresponde a um hash PBKDF2 gerado anteriormente.
        /// </summary>
        /// <param name="password">Senha a ser verificada.</param>
        /// <param name="hashedPassword">Hash gerado pelo método <see cref="HashPasswordPbkdf2"/>.</param>
        /// <returns>True se a senha corresponder ao hash, false caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando password ou hashedPassword é null.</exception>
        /// <exception cref="ArgumentException">Lançado quando o formato do hash é inválido.</exception>
        /// <example>
        /// <code>
        /// string password = "MySecureP@ssw0rd!";
        /// string storedHash = "$pbkdf2$100000$..."; // Hash armazenado no banco
        /// 
        /// bool isValid = SecurityHelper.VerifyPasswordPbkdf2(password, storedHash);
        /// if (isValid)
        /// {
        ///     Console.WriteLine("Senha correta!");
        /// }
        /// </code>
        /// </example>
        public static bool VerifyPasswordPbkdf2(string password, string hashedPassword)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            if (hashedPassword == null)
                throw new ArgumentNullException(nameof(hashedPassword));

            try
            {
                // Parse do formato: $pbkdf2$iterations$salt$hash
                string[] parts = hashedPassword.Split('$');
                if (parts.Length != 5 || parts[0] != string.Empty || parts[1] != "pbkdf2")
                    throw new ArgumentException("Invalid hash format.", nameof(hashedPassword));

                int iterations = int.Parse(parts[2]);
                byte[] salt = Convert.FromBase64String(parts[3]);
                byte[] originalHash = Convert.FromBase64String(parts[4]);

                // Computa o hash da senha fornecida com o mesmo salt e iterações
                byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(
                    password: Encoding.UTF8.GetBytes(password),
                    salt: salt,
                    iterations: iterations,
                    hashAlgorithm: HashAlgorithmName.SHA256,
                    outputLength: originalHash.Length);

                // Comparação segura contra timing attacks
                return CryptographicOperations.FixedTimeEquals(originalHash, testHash);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Invalid hash format.", nameof(hashedPassword));
            }
        }

        /// <summary>
        /// Gera uma string aleatória criptograficamente segura.
        /// </summary>
        /// <param name="length">Comprimento da string a ser gerada.</param>
        /// <param name="includeSpecialChars">Se true, inclui caracteres especiais.</param>
        /// <returns>String aleatória segura.</returns>
        /// <exception cref="ArgumentException">Lançado quando length é menor que 1.</exception>
        /// <example>
        /// <code>
        /// string randomToken = SecurityHelper.GenerateSecureRandomString(32);
        /// string randomPassword = SecurityHelper.GenerateSecureRandomString(16, includeSpecialChars: true);
        /// </code>
        /// </example>
        public static string GenerateSecureRandomString(int length, bool includeSpecialChars = false)
        {
            if (length < 1)
                throw new ArgumentException("Length must be at least 1.", nameof(length));

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            string availableChars = includeSpecialChars ? chars + specialChars : chars;

            byte[] randomBytes = RandomNumberGenerator.GetBytes(length);
            char[] result = new char[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = availableChars[randomBytes[i] % availableChars.Length];
            }

            return new string(result);
        }

        /// <summary>
        /// Valida a força de uma senha com base em critérios de segurança.
        /// </summary>
        /// <param name="password">Senha a ser validada.</param>
        /// <param name="minLength">Comprimento mínimo (padrão: 8).</param>
        /// <param name="requireUppercase">Requer letra maiúscula (padrão: true).</param>
        /// <param name="requireLowercase">Requer letra minúscula (padrão: true).</param>
        /// <param name="requireDigit">Requer número (padrão: true).</param>
        /// <param name="requireSpecialChar">Requer caractere especial (padrão: true).</param>
        /// <returns>True se a senha atende todos os critérios, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// bool isStrong = SecurityHelper.IsPasswordStrong("MyP@ssw0rd!");
        /// </code>
        /// </example>
        public static bool IsPasswordStrong(
            string password,
            int minLength = 8,
            bool requireUppercase = true,
            bool requireLowercase = true,
            bool requireDigit = true,
            bool requireSpecialChar = true)
        {
            if (password.IsNullOrWhiteSpace())
                return false;

            if (password.Length < minLength)
                return false;

            if (requireUppercase && !password.ContainsUpperCase())
                return false;

            if (requireLowercase && !password.Any(char.IsLower))
                return false;

            if (requireDigit && !password.ContainsNumber())
                return false;

            if (requireSpecialChar && !password.ContainsSpecialCharacter())
                return false;

            return true;
        }

        /// <summary>
        /// Gera um token seguro para reset de senha, verificação de e-mail, etc.
        /// </summary>
        /// <returns>Token base64url-encoded de 32 bytes (256 bits).</returns>
        /// <example>
        /// <code>
        /// string resetToken = SecurityHelper.GenerateSecurityToken();
        /// // Envie por e-mail e armazene hash no banco
        /// </code>
        /// </example>
        public static string GenerateSecurityToken()
        {
            byte[] tokenBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .TrimEnd('='); // Base64Url encoding
        }
    }
}
