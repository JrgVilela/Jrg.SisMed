using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Helpers
{
    /// <summary>
    /// Classe estática com métodos de extensão para validação de documentos brasileiros e formatos comuns.
    /// </summary>
    public static partial class ValidateHelper
    {
        // Regex compilado para melhor performance em validações repetidas
        // Pattern baseado no padrão RFC 5322 (mais permissivo mas preciso)
        [GeneratedRegex(@"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", 
            RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, matchTimeoutMilliseconds: 1000)]
        private static partial Regex EmailRegex();

        // Multiplicadores para validação de CNPJ
        private static readonly int[] CnpjMultiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        private static readonly int[] CnpjMultiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        // Multiplicadores para validação de CPF
        private static readonly int[] CpfMultiplier1 = [10, 9, 8, 7, 6, 5, 4, 3, 2];
        private static readonly int[] CpfMultiplier2 = [11, 10, 9, 8, 7, 6, 5, 4, 3, 2];

        // CNPJs conhecidos como inválidos (todos os dígitos iguais)
        private static readonly HashSet<string> InvalidCnpjs = new()
        {
            "00000000000000", "11111111111111", "22222222222222", "33333333333333",
            "44444444444444", "55555555555555", "66666666666666", "77777777777777",
            "88888888888888", "99999999999999"
        };

        // CPFs conhecidos como inválidos (todos os dígitos iguais)
        private static readonly HashSet<string> InvalidCpfs = new()
        {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };

        /// <summary>
        /// Valida se um CNPJ é válido de acordo com o algoritmo oficial da Receita Federal.
        /// </summary>
        /// <param name="cnpj">CNPJ a ser validado (pode conter formatação).</param>
        /// <returns>Retorna true se o CNPJ for válido, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string cnpj1 = "12.345.678/0001-95";
        /// bool isValid1 = cnpj1.IsCnpj(); // Valida o CNPJ
        /// 
        /// string cnpj2 = "12345678000195";
        /// bool isValid2 = cnpj2.IsCnpj(); // Também aceita sem formatação
        /// </code>
        /// </example>
        /// <remarks>
        /// Remove automaticamente caracteres não numéricos antes da validação.
        /// Rejeita CNPJs com todos os dígitos iguais (ex: 11111111111111).
        /// </remarks>
        public static bool IsCnpj(this string cnpj)
        {
            if (cnpj.IsNullOrEmpty()) 
                return false;

            cnpj = cnpj.GetOnlyNumbers();

            if (cnpj.Length != 14) 
                return false;

            // Rejeita CNPJs conhecidos como inválidos (todos os dígitos iguais)
            if (InvalidCnpjs.Contains(cnpj))
                return false;

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 12; i++)
            {
                soma += (cnpj[i] - '0') * CnpjMultiplier1[i];
            }

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 13; i++)
            {
                soma += (cnpj[i] - '0') * CnpjMultiplier2[i];
            }

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            // Verifica se os dígitos calculados correspondem aos dígitos informados
            return cnpj[12] - '0' == digito1 && cnpj[13] - '0' == digito2;
        }

        /// <summary>
        /// Valida se um CPF é válido de acordo com o algoritmo oficial da Receita Federal.
        /// </summary>
        /// <param name="cpf">CPF a ser validado (pode conter formatação).</param>
        /// <returns>Retorna true se o CPF for válido, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string cpf1 = "123.456.789-01";
        /// bool isValid1 = cpf1.IsCpf(); // Valida o CPF
        /// 
        /// string cpf2 = "12345678901";
        /// bool isValid2 = cpf2.IsCpf(); // Também aceita sem formatação
        /// </code>
        /// </example>
        /// <remarks>
        /// Remove automaticamente caracteres não numéricos antes da validação.
        /// Rejeita CPFs com todos os dígitos iguais (ex: 11111111111).
        /// </remarks>
        public static bool IsCpf(this string cpf)
        {
            if (cpf.IsNullOrEmpty()) 
                return false;

            cpf = cpf.GetOnlyNumbers();

            if (cpf.Length != 11) 
                return false;

            // Rejeita CPFs conhecidos como inválidos (todos os dígitos iguais)
            if (InvalidCpfs.Contains(cpf))
                return false;

            // Calcula o primeiro dígito verificador
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * CpfMultiplier1[i];
            }

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;

            // Calcula o segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * CpfMultiplier2[i];
            }

            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;

            // Verifica se os dígitos calculados correspondem aos dígitos informados
            return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
        }

        /// <summary>
        /// Valida se um CPF ou CNPJ é válido, detectando automaticamente o tipo baseado no comprimento.
        /// </summary>
        /// <param name="cpfOrCnpj">CPF ou CNPJ a ser validado.</param>
        /// <returns>Retorna true se o documento for válido, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string doc1 = "123.456.789-01";
        /// bool isValid1 = doc1.IsCpfOrCnpj(); // Valida como CPF
        /// 
        /// string doc2 = "12.345.678/0001-95";
        /// bool isValid2 = doc2.IsCpfOrCnpj(); // Valida como CNPJ
        /// </code>
        /// </example>
        public static bool IsCpfOrCnpj(this string cpfOrCnpj)
        {
            if (cpfOrCnpj.IsNullOrEmpty())
                return false;

            string numbersOnly = cpfOrCnpj.GetOnlyNumbers();

            return numbersOnly.Length switch
            {
                11 => cpfOrCnpj.IsCpf(),
                14 => cpfOrCnpj.IsCnpj(),
                _ => false
            };
        }

        /// <summary>
        /// Valida se uma string está no formato de e-mail válido.
        /// </summary>
        /// <param name="email">E-mail a ser validado.</param>
        /// <returns>Retorna true se o e-mail for válido, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string email1 = "usuario@exemplo.com.br";
        /// bool isValid1 = email1.IsEmail(); // Retorna true
        /// 
        /// string email2 = "email.invalido@";
        /// bool isValid2 = email2.IsEmail(); // Retorna false
        /// 
        /// string email3 = "user+tag@domain.co.uk";
        /// bool isValid3 = email3.IsEmail(); // Retorna true
        /// </code>
        /// </example>
        /// <remarks>
        /// <para>Utiliza regex source generator do .NET 7+ para melhor performance.</para>
        /// <para>Baseado no padrão RFC 5322 simplificado.</para>
        /// <para>Valida o formato mas não verifica se o e-mail realmente existe.</para>
        /// <para>Aceita caracteres especiais comuns como +, -, _, . no local part.</para>
        /// <para>Tem timeout de 1 segundo para prevenir ataques de ReDoS.</para>
        /// </remarks>
        public static bool IsEmail(this string email)
        {
            if (email.IsNullOrWhiteSpace())
                return false;

            // Validação básica de comprimento (RFC 5321)
            if (email.Length > 254)
                return false;

            // Deve conter exatamente um @
            int atIndex = email.IndexOf('@');
            if (atIndex <= 0 || atIndex != email.LastIndexOf('@'))
                return false;

            // Valida comprimentos das partes
            string localPart = email.Substring(0, atIndex);
            string domainPart = email.Substring(atIndex + 1);

            if (localPart.Length == 0 || localPart.Length > 64) // RFC 5321: local part max 64 chars
                return false;

            if (domainPart.Length == 0 || domainPart.Length > 253) // RFC 5321: domain max 253 chars
                return false;

            try
            {
                return EmailRegex().IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                // Timeout de regex - possível ataque ReDoS
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida se uma string representa um CEP brasileiro válido.
        /// </summary>
        /// <param name="cep">CEP a ser validado (pode conter formatação).</param>
        /// <returns>Retorna true se o CEP tiver 8 dígitos, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string cep1 = "01310-100";
        /// bool isValid1 = cep1.IsCep(); // Retorna true
        /// 
        /// string cep2 = "01310100";
        /// bool isValid2 = cep2.IsCep(); // Retorna true
        /// </code>
        /// </example>
        /// <remarks>
        /// Valida apenas o formato (8 dígitos), não verifica se o CEP existe.
        /// </remarks>
        public static bool IsCep(this string cep)
        {
            if (cep.IsNullOrEmpty())
                return false;

            cep = cep.GetOnlyNumbers();

            return cep.Length == 8;
        }

        /// <summary>
        /// Valida se uma string representa um telefone brasileiro válido.
        /// </summary>
        /// <param name="phone">Telefone a ser validado (pode conter formatação).</param>
        /// <returns>Retorna true se o telefone tiver 8, 9, 10 ou 11 dígitos, false caso contrário.</returns>
        /// <example>
        /// <code>
        /// string phone1 = "(11) 98765-4321";
        /// bool isValid1 = phone1.IsPhone(); // Retorna true (11 dígitos)
        /// 
        /// string phone2 = "1234-5678";
        /// bool isValid2 = phone2.IsPhone(); // Retorna true (8 dígitos)
        /// </code>
        /// </example>
        /// <remarks>
        /// Aceita telefones com 8 dígitos (fixo sem DDD), 9 dígitos (celular sem DDD),
        /// 10 dígitos (fixo com DDD) ou 11 dígitos (celular com DDD).
        /// </remarks>
        public static bool IsPhone(this string phone)
        {
            if (phone.IsNullOrEmpty())
                return false;

            phone = phone.GetOnlyNumbers();

            return phone.Length is 8 or 9 or 10 or 11;
        }
    }
}
