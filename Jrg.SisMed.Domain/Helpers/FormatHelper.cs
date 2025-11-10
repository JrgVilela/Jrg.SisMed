using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Helpers
{
    /// <summary>
    /// Classe estática com métodos de extensão para formatação de strings em padrões brasileiros.
    /// </summary>
    public static class FormatHelper
    {
        /// <summary>
        /// Formata uma string de CEP no padrão brasileiro (00000-000).
        /// </summary>
        /// <param name="cep">CEP a ser formatado (pode conter caracteres não numéricos).</param>
        /// <returns>CEP formatado no padrão 00000-000 ou string vazia se inválido.</returns>
        /// <example>
        /// <code>
        /// string cep = "01310100";
        /// string formatado = cep.FormatCep(); // Retorna "01310-100"
        /// </code>
        /// </example>
        public static string FormatCep(this string cep)
        {
            if (cep.IsNullOrEmpty()) 
                return string.Empty;

            cep = cep.GetOnlyNumbers();

            if (cep.Length != 8)
                return string.Empty;

            return $"{cep.Substring(0, 5)}-{cep.Substring(5, 3)}";
        }

        /// <summary>
        /// Formata uma string de Cnpj no padrão brasileiro (00.000.000/0000-00).
        /// </summary>
        /// <param name="cnpj">Cnpj a ser formatado (pode conter caracteres não numéricos).</param>
        /// <returns>Cnpj formatado no padrão 00.000.000/0000-00 ou string vazia se inválido.</returns>
        /// <example>
        /// <code>
        /// string cnpj = "12345678000195";
        /// string formatado = cnpj.FormatCnpj(); // Retorna "12.345.678/0001-95"
        /// </code>
        /// </example>
        public static string FormatCnpj(this string cnpj)
        {
            if (cnpj.IsNullOrEmpty()) 
                return string.Empty;

            cnpj = cnpj.GetOnlyNumbers();

            if (cnpj.Length != 14)
                return string.Empty;

            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        /// <summary>
        /// Formata uma string de CPF no padrão brasileiro (000.000.000-00).
        /// </summary>
        /// <param name="cpf">CPF a ser formatado (pode conter caracteres não numéricos).</param>
        /// <returns>CPF formatado no padrão 000.000.000-00 ou string vazia se inválido.</returns>
        /// <example>
        /// <code>
        /// string cpf = "12345678901";
        /// string formatado = cpf.FormatCpf(); // Retorna "123.456.789-01"
        /// </code>
        /// </example>
        public static string FormatCpf(this string cpf)
        {
            if (cpf.IsNullOrEmpty()) 
                return string.Empty;

            cpf = cpf.GetOnlyNumbers();

            if (cpf.Length != 11)
                return string.Empty;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        /// <summary>
        /// Formata uma string de CPF ou Cnpj automaticamente, detectando qual é baseado no comprimento.
        /// </summary>
        /// <param name="cpfCnpj">CPF ou Cnpj a ser formatado.</param>
        /// <returns>CPF ou Cnpj formatado ou string vazia se inválido.</returns>
        /// <remarks>
        /// Usa os métodos de validação IsCpf() e IsCnpj() para determinar o tipo do documento.
        /// </remarks>
        public static string FormatCpfOrCnpj(this string cpfCnpj)
        {
            if (cpfCnpj.IsNullOrEmpty())
                return string.Empty;

            if (!cpfCnpj.IsCpf() && !cpfCnpj.IsCnpj()) 
                return string.Empty;

            return cpfCnpj.IsCpf() ? cpfCnpj.FormatCpf() : cpfCnpj.FormatCnpj();
        }

        /// <summary>
        /// Formata um número de telefone brasileiro.
        /// Suporta telefones fixos (8 dígitos) e celulares (9 dígitos).
        /// </summary>
        /// <param name="phone">Número de telefone a ser formatado.</param>
        /// <returns>Telefone formatado (0000-0000 ou 00000-0000) ou string vazia se inválido.</returns>
        /// <example>
        /// <code>
        /// string phone1 = "987654321";
        /// string formatted1 = phone1.FormatPhone(); // Retorna "98765-4321"
        /// 
        /// string phone2 = "12345678";
        /// string formatted2 = phone2.FormatPhone(); // Retorna "1234-5678"
        /// </code>
        /// </example>
        public static string FormatPhone(this string phone)
        {
            if (phone.IsNullOrEmpty()) 
                return string.Empty;

            phone = phone.GetOnlyNumbers();

            return phone.Length switch
            {
                8 => $"{phone.Substring(0, 4)}-{phone.Substring(4, 4)}",
                9 => $"{phone.Substring(0, 5)}-{phone.Substring(5, 4)}",
                10 => $"({phone.Substring(0, 2)}) {phone.Substring(2, 4)}-{phone.Substring(6, 4)}",
                11 => $"({phone.Substring(0, 2)}) {phone.Substring(2, 5)}-{phone.Substring(7, 4)}",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Formata um número de telefone brasileiro com DDD.
        /// </summary>
        /// <param name="phone">Número de telefone (8 ou 9 dígitos).</param>
        /// <param name="ddd">Código DDD (2 dígitos).</param>
        /// <returns>Telefone formatado com DDD no formato (00) 0000-0000 ou (00) 00000-0000.</returns>
        /// <example>
        /// <code>
        /// string phone = "987654321";
        /// string formatted = phone.FormatPhone("11"); // Retorna "(11) 98765-4321"
        /// </code>
        /// </example>
        public static string FormatPhoneWithDdd(this string phone, string ddd)
        {
            if (phone.IsNullOrEmpty() || ddd.IsNullOrEmpty()) 
                return string.Empty;

            phone = phone.GetOnlyNumbers();
            ddd = ddd.GetOnlyNumbers();

            if (ddd.Length != 2)
                return string.Empty;

            return phone.Length switch
            {
                8 => $"({ddd}) {phone.Substring(0, 4)}-{phone.Substring(4, 4)}",
                9 => $"({ddd}) {phone.Substring(0, 5)}-{phone.Substring(5, 4)}",
                _ => string.Empty
            };
        }

        /// <summary>
        /// Formata um número de telefone internacional com DDI e DDD.
        /// </summary>
        /// <param name="phone">Número de telefone (8 ou 9 dígitos).</param>
        /// <param name="ddi">Código DDI (código do país).</param>
        /// <param name="ddd">Código DDD (2 dígitos).</param>
        /// <returns>Telefone formatado com DDI e DDD no formato +00 (00) 0000-0000 ou +00 (00) 00000-0000.</returns>
        /// <example>
        /// <code>
        /// string phone = "987654321";
        /// string formatted = phone.FormatPhoneWithDdiAndDdd("55", "11"); // Retorna "+55 (11) 98765-4321"
        /// </code>
        /// </example>
        public static string FormatPhoneWithDdiAndDdd(this string phone, string ddi, string ddd)
        {
            if (phone.IsNullOrEmpty() || ddd.IsNullOrEmpty() || ddi.IsNullOrEmpty()) 
                return string.Empty;

            phone = phone.GetOnlyNumbers();
            ddd = ddd.GetOnlyNumbers();
            ddi = ddi.GetOnlyNumbers();

            if (ddd.Length != 2 || ddi.IsNullOrEmpty())
                return string.Empty;

            return phone.Length switch
            {
                8 => $"+{ddi} ({ddd}) {phone.Substring(0, 4)}-{phone.Substring(4, 4)}",
                9 => $"+{ddi} ({ddd}) {phone.Substring(0, 5)}-{phone.Substring(5, 4)}",
                _ => string.Empty
            };
        }
    }
}
