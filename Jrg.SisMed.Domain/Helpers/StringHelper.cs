using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Helpers
{
    /// <summary>
    /// Classe estática com métodos de extensão para manipulação e validação de strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Verifica se o valor informado não é nulo e não está vazio.
        /// </summary>
        /// <param name="value">Valor que será verificado.</param>
        /// <returns>Retorna verdadeiro se não for nulo e não estiver vazio, falso caso contrário.</returns>
        public static bool IsNotNullOrEmpty(this string value) => !string.IsNullOrEmpty(value);

        /// <summary>
        /// Verifica se o valor informado é nulo ou vazio.
        /// </summary>
        /// <param name="value">Valor que será verificado.</param>
        /// <returns>Retorna verdadeiro se for nulo ou vazio e falso caso contrário.</returns>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>
        /// Verifica se o valor informado não é nulo e não contém apenas espaços em branco.
        /// </summary>
        /// <param name="value">Valor que será verificado.</param>
        /// <returns>Retorna verdadeiro se não for nulo e não contiver apenas espaços em branco, falso caso contrário.</returns>
        public static bool IsNotNullOrWhiteSpace(this string value) => !string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Verifica se o valor informado é nulo ou contém apenas espaços em branco.
        /// </summary>
        /// <param name="value">Valor que será verificado.</param>
        /// <returns>Retorna verdadeiro se for nulo ou se contiver apenas espaços em branco e falso caso contrário.</returns>
        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Verifica se os valores informados são diferentes.
        /// </summary>
        /// <param name="value">Primeiro valor que será verificado.</param>
        /// <param name="newValue">Segundo valor que será comparado ao primeiro valor.</param>
        /// <returns>Retorna verdadeiro se forem diferentes e falso se forem iguais.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        public static bool IsNotEquals(this string value, string newValue) => value != null && !value.Equals(newValue);

        /// <summary>
        /// Converte a primeira letra para maiúscula e as demais para minúscula.
        /// </summary>
        /// <param name="value">Valor que será tratado.</param>
        /// <returns>Retorna o mesmo valor informado, mas com a primeira letra maiúscula e as demais minúsculas.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        /// <exception cref="ArgumentException">Lançado quando value está vazio.</exception>
        public static string ToFirstLetterUpper(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            if (value.Length == 1)
                return value.ToUpper();

            return string.Concat(value[0].ToString().ToUpper(), value.AsSpan(1).ToString().ToLower());
        }

        /// <summary>
        /// Converte a primeira letra de cada palavra da string para maiúscula.
        /// </summary>
        /// <param name="value">Valor que será tratado.</param>
        /// <returns>Retorna o mesmo valor informado, mas com a primeira letra de cada palavra maiúscula e as demais minúsculas.</returns>
        public static string ToTitleCase(this string value)
        {
            if (value.IsNullOrEmpty()) 
                return string.Empty;

            string[] palavras = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (var i = 0; i < palavras.Length; i++)
            {
                if (palavras[i].Length > 0)
                    palavras[i] = palavras[i].ToFirstLetterUpper();
            }

            return string.Join(" ", palavras);
        }

        /// <summary>
        /// Converte a primeira letra após cada ponto (.) para maiúscula.
        /// </summary>
        /// <param name="value">Valor que será tratado.</param>
        /// <returns>Retorna o mesmo valor informado, mas com a primeira letra após cada ponto maiúscula.</returns>
        public static string ToSentenceCase(this string value)
        {
            if (value.IsNullOrEmpty()) 
                return string.Empty;

            string[] frases = value.Split('.', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < frases.Length; i++)
            {
                frases[i] = frases[i].Trim();
                if (frases[i].Length > 0)
                    frases[i] = frases[i].ToFirstLetterUpper();
            }

            return string.Join(". ", frases);
        }

        /// <summary>
        /// Remove espaços duplos de toda a string, substituindo-os por um único espaço.
        /// </summary>
        /// <param name="value">Valor que será tratado.</param>
        /// <returns>String com os espaços duplicados removidos.</returns>
        public static string RemoveDoubleSpaces(this string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return string.Empty;

            while (value.Contains("  "))
            {
                value = value.Replace("  ", " ");
            }

            return value.Trim();
        }

        /// <summary>
        /// Remove todos os espaços da string, não somente das extremidades, como também no meio do texto.
        /// </summary>
        /// <param name="value">Valor que será tratado.</param>
        /// <returns>String sem espaços.</returns>
        public static string RemoveAllSpaces(this string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return string.Empty;

            return value.Replace(" ", string.Empty);
        }

        /// <summary>
        /// Verifica se o comprimento da string está dentro de um intervalo especificado.
        /// </summary>
        /// <param name="value">String que será verificada.</param>
        /// <param name="minValue">Valor mínimo permitido (inclusivo).</param>
        /// <param name="maxValue">Valor máximo permitido (inclusivo).</param>
        /// <returns>Retorna verdadeiro se o comprimento estiver entre minValue e maxValue, falso caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        public static bool IsLengthBetween(this string value, int minValue, int maxValue)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Length >= minValue && value.Length <= maxValue;
        }

        /// <summary>
        /// Extrai apenas os caracteres numéricos da string.
        /// </summary>
        /// <param name="value">String de origem.</param>
        /// <returns>Retorna uma nova string contendo apenas os dígitos numéricos.</returns>
        /// <example>
        /// <code>
        /// string telefone = "(11) 98765-4321";
        /// string numeros = telefone.GetOnlyNumbers(); // Retorna "11987654321"
        /// </code>
        /// </example>
        public static string GetOnlyNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value)) 
                return string.Empty;

            return new string(value.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// Verifica se a string contém pelo menos um caractere numérico.
        /// </summary>
        /// <param name="value">String que será verificada.</param>
        /// <returns>Retorna verdadeiro se contiver pelo menos um número, falso caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        public static bool ContainsNumber(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Any(char.IsDigit);
        }

        /// <summary>
        /// Verifica se a string contém pelo menos uma letra maiúscula.
        /// </summary>
        /// <param name="value">String que será verificada.</param>
        /// <returns>Retorna verdadeiro se contiver pelo menos uma letra maiúscula, falso caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        public static bool ContainsUpperCase(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Any(char.IsUpper);
        }

        /// <summary>
        /// Verifica se a string contém pelo menos uma letra (maiúscula ou minúscula).
        /// </summary>
        /// <param name="value">String que será verificada.</param>
        /// <returns>Retorna verdadeiro se contiver pelo menos uma letra, falso caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        public static bool ContainsLetter(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Any(char.IsLetter);
        }

        /// <summary>
        /// Verifica se a string contém pelo menos um caractere especial (símbolo ou pontuação).
        /// </summary>
        /// <param name="value">String que será verificada.</param>
        /// <returns>Retorna verdadeiro se contiver pelo menos um caractere especial, falso caso contrário.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        /// <remarks>
        /// Considera símbolos (IsSymbol) e pontuação (IsPunctuation) como caracteres especiais.
        /// </remarks>
        public static bool ContainsSpecialCharacter(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return value.Any(c => char.IsSymbol(c) || char.IsPunctuation(c));
        }

        /// <summary>
        /// Inverte a ordem dos caracteres da string.
        /// </summary>
        /// <param name="value">String que será invertida.</param>
        /// <returns>Retorna uma nova string com os caracteres em ordem reversa.</returns>
        /// <exception cref="ArgumentNullException">Lançado quando value é null.</exception>
        /// <example>
        /// <code>
        /// string texto = "Hello";
        /// string invertido = texto.Reverse(); // Retorna "olleH"
        /// </code>
        /// </example>
        public static string Reverse(this string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            if (value.Length <= 1)
                return value;

            char[] arr = value.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}
