using Jrg.SisMed.Domain.Exceptions;
using Jrg.SisMed.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Entities
{
    /// <summary>
    /// Representa um número de telefone no sistema.
    /// </summary>
    public class Phone : EntityBase
    {
        private const int MinNumberLength = 8;
        private const int MaxNumberLength = 9;
        private const int DddLength = 2;
        private const int MinDdiLength = 1;
        private const int MaxDdiLength = 3;

        /// <summary>
        /// Código DDI (código internacional do país).
        /// </summary>
        public string Ddi { get; private set; } = string.Empty;
        
        /// <summary>
        /// Código DDD (código de área).
        /// </summary>
        public string Ddd { get; private set; } = string.Empty;
        
        /// <summary>
        /// Número do telefone (sem DDD e DDI).
        /// </summary>
        public string Number { get; private set; } = string.Empty;

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal Phone() { }

        /// <summary>
        /// Cria uma nova instância de Phone com validação completa.
        /// </summary>
        /// <param name="ddi">Código DDI (código do país, ex: "55" para Brasil).</param>
        /// <param name="ddd">Código DDD (código de área, ex: "11" para São Paulo).</param>
        /// <param name="number">Número do telefone (8 ou 9 dígitos).</param>
        /// <example>
        /// <code>
        /// // Celular brasileiro
        /// var phone = new Phone("55", "11", "987654321");
        /// 
        /// // Telefone fixo brasileiro
        /// var phone2 = new Phone("55", "11", "12345678");
        /// </code>
        /// </example>
        public Phone(string ddi, string ddd, string number)
        {
            Update(ddi, ddd, number);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Atualiza os dados do telefone com validação completa.
        /// </summary>
        /// <param name="ddi">Código DDI (código do país).</param>
        /// <param name="ddd">Código DDD (código de área).</param>
        /// <param name="number">Número do telefone.</param>
        public void Update(string ddi, string ddd, string number)
        {
            // Atribui valores para validação
            Ddi = ddi;
            Ddd = ddd;
            Number = number;
            
            // Normaliza e valida
            Normalize();
            Validate();
            
            // Atualiza timestamp se já existe
            if (Id > 0)
                UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Retorna o número formatado no padrão internacional.
        /// </summary>
        /// <returns>Número formatado como +DDI (DDD) XXXXX-XXXX</returns>
        /// <example>
        /// <code>
        /// var phone = new Phone("55", "11", "987654321");
        /// string formatted = phone.ToString(); // "+55 (11) 98765-4321"
        /// </code>
        /// </example>
        public override string ToString()
        {
            return Number.FormatPhoneWithDdiAndDdd(Ddi, Ddd);
        }

        /// <summary>
        /// Retorna o número formatado apenas com DDD (formato nacional).
        /// </summary>
        /// <returns>Número formatado como (DDD) XXXXX-XXXX</returns>
        /// <example>
        /// <code>
        /// var phone = new Phone("55", "11", "987654321");
        /// string formatted = phone.GetFormattedNumber(); // "(11) 98765-4321"
        /// </code>
        /// </example>
        public string GetFormattedNumber()
        {
            return Number.FormatPhoneWithDdd(Ddd);
        }

        /// <summary>
        /// Verifica se é um telefone celular (9 dígitos).
        /// </summary>
        public bool IsMobile()
        {
            return Number.Length == MaxNumberLength;
        }

        /// <summary>
        /// Verifica se é um telefone fixo (8 dígitos).
        /// </summary>
        public bool IsLandline()
        {
            return Number.Length == MinNumberLength;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida todos os dados do telefone.
        /// </summary>
        private void Validate()
        {
            var v = new ValidationCollector();

            v.When(Ddi.IsNullOrWhiteSpace(), "O código do país (DDI) é obrigatório.");
            v.When(Ddi.Length < MinDdiLength || Ddi.Length > MaxDdiLength, 
                $"O código do país (DDI) deve conter entre {MinDdiLength} e {MaxDdiLength} dígitos.");
            
            v.When(Ddd.IsNullOrWhiteSpace(), "O código de área (DDD) é obrigatório.");
            v.When(Ddd.Length != DddLength, 
                $"O código de área (DDD) deve conter exatamente {DddLength} dígitos.");
            
            v.When(Number.IsNullOrWhiteSpace(), "O número do telefone é obrigatório.");
            v.When(Number.Length < MinNumberLength || Number.Length > MaxNumberLength, 
                $"O número de telefone deve conter entre {MinNumberLength} e {MaxNumberLength} dígitos.");
            
            // Valida se são apenas números
            v.When(!Ddi.ContainsNumber() || Ddi.ContainsLetter(), 
                "O DDI deve conter apenas números.");
            v.When(!Ddd.ContainsNumber() || Ddd.ContainsLetter(), 
                "O DDD deve conter apenas números.");
            v.When(!Number.ContainsNumber() || Number.ContainsLetter(), 
                "O número do telefone deve conter apenas números.");

            v.ThrowIfAny();
        }

        /// <summary>
        /// Normaliza os dados do telefone removendo formatação.
        /// </summary>
        private void Normalize()
        {
            Ddi = Ddi.GetOnlyNumbers();
            Ddd = Ddd.GetOnlyNumbers();
            Number = Number.GetOnlyNumbers();
        }
        #endregion
    }
}
