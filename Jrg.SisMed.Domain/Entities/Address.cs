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
    /// Representa um endereço no sistema.
    /// </summary>
    public class Address : EntityBase
    {
        private const int MaxStreetLength = 200;
        private const int MaxNumberLength = 20;
        private const int MaxComplementLength = 100;
        private const int MaxZipCodeLength = 10;
        private const int MaxCityLength = 100;
        private const int MaxStateLength = 50;

        public string Street { get; private set; } = string.Empty;
        public string Number { get; private set; } = string.Empty;
        public string? Complement { get; private set; }
        public string ZipCode { get; private set; } = string.Empty;
        public string City { get; private set; } = string.Empty;
        public string State { get; private set; } = string.Empty;

        #region Constructors
        /// <summary>
        /// Construtor protegido para uso do Entity Framework.
        /// </summary>
        internal Address() { }
        
        /// <summary>
        /// Cria uma nova instância de Address com validação completa.
        /// </summary>
        /// <param name="street">Nome da rua/logradouro.</param>
        /// <param name="number">Número do endereço.</param>
        /// <param name="complement">Complemento (opcional).</param>
        /// <param name="zipCode">CEP do endereço.</param>
        /// <param name="city">Cidade.</param>
        /// <param name="state">Estado/UF.</param>
        public Address(string street, string number, string? complement, string zipCode, string city, string state)
        {
            Update(street, number, complement, zipCode, city, state);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Atualiza os dados do endereço com validação completa.
        /// </summary>
        public void Update(string street, string number, string? complement, string zipCode, string city, string state)
        {
            // Atribui valores para validação
            Street = street;
            Number = number;
            Complement = complement;
            ZipCode = zipCode;
            City = city;
            State = state;
            
            // Normaliza e valida
            Normalize();
            Validate();
            
            // Atualiza timestamp se já existe
            if (Id > 0)
                UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Valida todos os dados do endereço.
        /// </summary>
        private void Validate()
        {
            var v = new ValidationCollector();

            v.When(Street.IsNullOrWhiteSpace(), "A rua é obrigatória.");
            v.When(Street.Length > MaxStreetLength, $"A rua deve conter no máximo {MaxStreetLength} caracteres.");
            
            v.When(Number.IsNullOrWhiteSpace(), "O número é obrigatório.");
            v.When(Number.Length > MaxNumberLength, $"O número deve conter no máximo {MaxNumberLength} caracteres.");
            
            v.When(Complement != null && Complement.Length > MaxComplementLength, 
                $"O complemento deve conter no máximo {MaxComplementLength} caracteres.");
            
            v.When(ZipCode.IsNullOrWhiteSpace(), "O CEP é obrigatório.");
            v.When(!ZipCode.IsCep(), "O CEP informado é inválido.");
            
            v.When(City.IsNullOrWhiteSpace(), "A cidade é obrigatória.");
            v.When(City.Length > MaxCityLength, $"A cidade deve conter no máximo {MaxCityLength} caracteres.");
            
            v.When(State.IsNullOrWhiteSpace(), "O estado é obrigatório.");
            v.When(State.Length > MaxStateLength, $"O estado deve conter no máximo {MaxStateLength} caracteres.");
            
            v.ThrowIfAny();
        }

        /// <summary>
        /// Normaliza os dados do endereço.
        /// </summary>
        private void Normalize()
        {
            Street = Street.Trim().ToTitleCase();
            Number = Number.Trim();
            Complement = Complement?.Trim();
            ZipCode = ZipCode.GetOnlyNumbers();
            City = City.Trim().ToTitleCase();
            State = State.Trim().ToUpper();
        }
        #endregion
    }
}
