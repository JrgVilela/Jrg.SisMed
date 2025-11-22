using FluentValidation;
using Jrg.SisMed.Application.DTOs.ProfessionalDto;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Helpers;
using Jrg.SisMed.Domain.Resources;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Application.Validations.ProfessionalValidations
{
    /// <summary>
    /// Validação do DTO de registro de profissional.
    /// Valida todos os dados necessários para o cadastro completo: profissional, usuário, endereço e organização.
    /// </summary>
    public class RegisterProfessionalValidation : AbstractValidator<RegisterDto>
    {
        private readonly IStringLocalizer<Messages> _localizer;

        public RegisterProfessionalValidation(IStringLocalizer<Messages> localizer)
        {
            _localizer = localizer;

            ConfigureValidationRules();
        }

        private void ConfigureValidationRules()
        {
            // ========================================
            // VALIDAÇÕES DO PROFISSIONAL
            // ========================================
            
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage(_localizer.For(ProfessionalMessage.NameRequired).Value)
                .MinimumLength(3)
                .WithMessage(_localizer.For(ProfessionalMessage.NameMinLength, 3).Value)
                .MaximumLength(150)
                .WithMessage(_localizer.For(ProfessionalMessage.NameMaxLength, 150).Value);

            RuleFor(x => x.Cpf)
                .NotEmpty()
                .WithMessage(_localizer.For(ProfessionalMessage.CpfRequired).Value)
                .Must(BeValidCpf)
                .WithMessage(_localizer.For(ProfessionalMessage.CpfInvalid).Value);

            RuleFor(x => x.RegisterNumber)
                .NotEmpty()
                .WithMessage(_localizer.For(ProfessionalMessage.RegisterNumberRequired).Value)
                .MaximumLength(20)
                .WithMessage(_localizer.For(ProfessionalMessage.RegisterNumberMaxLength, 20).Value);

            RuleFor(x => x.ProfessionalType)
                .IsInEnum()
                .WithMessage(_localizer.For(ProfessionalMessage.ProfessionalTypeInvalid).Value)
                .NotEqual(default(ProfessionalType))
                .WithMessage(_localizer.For(ProfessionalMessage.ProfessionalTypeRequired).Value);

            // ========================================
            // VALIDAÇÕES DO USUÁRIO
            // ========================================

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.EmailRequired).Value)
                .EmailAddress()
                .WithMessage(_localizer.For(UserMessage.EmailInvalid).Value)
                .MaximumLength(100)
                .WithMessage(_localizer.For(UserMessage.EmailMaxLength, 100).Value);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(_localizer.For(UserMessage.PasswordRequired).Value)
                .MinimumLength(8)
                .WithMessage(_localizer.For(UserMessage.PasswordMinLength, 8).Value)
                .MaximumLength(25)
                .WithMessage(_localizer.For(UserMessage.PasswordMaxLength, 25).Value)
                .Must(BeStrongPassword)
                .WithMessage(_localizer.For(ProfessionalMessage.PasswordStrengthRequired).Value);

            // ========================================
            // VALIDAÇÕES DE CONTATO (TELEFONE)
            // ========================================

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage(_localizer.For(PhoneMessage.PhoneRequired).Value)
                .Must(BeValidPhone)
                .WithMessage(_localizer.For(PhoneMessage.PhoneInvalid).Value);

            // ========================================
            // VALIDAÇÕES DE ENDEREÇO
            // ========================================

            RuleFor(x => x.Street)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.StreetRequired).Value)
                .MaximumLength(200)
                .WithMessage(_localizer.For(AddressMessage.StreetMaxLength, 200).Value);

            RuleFor(x => x.Number)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.NumberRequired).Value)
                .MaximumLength(10)
                .WithMessage(_localizer.For(AddressMessage.NumberMaxLength, 10).Value);

            RuleFor(x => x.Complement)
                .MaximumLength(100)
                .WithMessage(_localizer.For(AddressMessage.ComplementMaxLength, 100).Value)
                .When(x => !string.IsNullOrWhiteSpace(x.Complement));

            RuleFor(x => x.Neighborhood)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.NeighborhoodRequired).Value)
                .MaximumLength(100)
                .WithMessage(_localizer.For(AddressMessage.NeighborhoodMaxLength, 100).Value);

            RuleFor(x => x.ZipCode)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.ZipCodeRequired).Value)
                .Must(BeValidZipCode)
                .WithMessage(_localizer.For(AddressMessage.ZipCodeInvalid).Value);

            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.CityRequired).Value)
                .MaximumLength(100)
                .WithMessage(_localizer.For(AddressMessage.CityMaxLength, 100).Value);

            RuleFor(x => x.State)
                .NotEmpty()
                .WithMessage(_localizer.For(AddressMessage.StateRequired).Value)
                .Length(2)
                .WithMessage(_localizer.For(AddressMessage.StateLength, 2).Value)
                .Must(BeValidBrazilianState)
                .WithMessage(_localizer.For(AddressMessage.StateInvalid).Value);

            // ========================================
            // VALIDAÇÕES DA ORGANIZAÇÃO
            // ========================================

            RuleFor(x => x.RazaoSocial)
                .NotEmpty()
                .WithMessage(_localizer.For(OrganizationMessage.LegalNameRequired).Value)
                .MaximumLength(200)
                .WithMessage(_localizer.For(OrganizationMessage.LegalNameMaxLength, 200).Value);

            RuleFor(x => x.NomeFantasia)
                .NotEmpty()
                .WithMessage(_localizer.For(OrganizationMessage.TradeNameRequired).Value)
                .MaximumLength(200)
                .WithMessage(_localizer.For(OrganizationMessage.TradeNameMaxLength, 200).Value);

            RuleFor(x => x.Cnpj)
                .NotEmpty()
                .WithMessage(_localizer.For(OrganizationMessage.CnpjRequired).Value)
                .Must(BeValidCnpj)
                .WithMessage(_localizer.For(OrganizationMessage.CnpjInvalid).Value);
        }

        // ========================================
        // MÉTODOS DE VALIDAÇÃO CUSTOMIZADOS
        // ========================================

        /// <summary>
        /// Valida se o CPF é válido.
        /// </summary>
        private bool BeValidCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            return cpf.IsCpf();
        }

        /// <summary>
        /// Valida se o CNPJ é válido.
        /// </summary>
        private bool BeValidCnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            return cnpj.IsCnpj();
        }

        /// <summary>
        /// Valida se a senha é forte (contém maiúsculas, minúsculas, números e caracteres especiais).
        /// </summary>
        private bool BeStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return SecurityHelper.IsPasswordStrong(
                password,
                minLength: 8,
                requireUppercase: true,
                requireLowercase: true,
                requireDigit: true,
                requireSpecialChar: true
            );
        }

        /// <summary>
        /// Valida se o telefone está no formato correto com máscara:
        /// - Celular: '+55 (11) 98399-1005' (9 dígitos)
        /// - Fixo: '+55 (11) 8399-1005' (8 dígitos)
        /// </summary>
        private bool BeValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Remove espaços extras
            phone = phone.Trim();

            // Verifica formato básico: +XX (XX) XXXXX-XXXX ou +XX (XX) XXXX-XXXX
            // Padrão: +[DDI] ([DDD]) [NUMERO]-[SUFIXO]
            
            // Deve começar com '+'
            if (!phone.StartsWith("+"))
                return false;

            // Encontra a posição do espaço após o DDI
            int firstSpaceIndex = phone.IndexOf(' ');
            if (firstSpaceIndex == -1 || firstSpaceIndex == 1) // Deve ter DDI após o '+'
                return false;

            // Extrai DDI (sem o '+')
            string ddi = phone.Substring(1, firstSpaceIndex - 1);
            
            // DDI deve ter 1-3 dígitos
            if (ddi.Length < 1 || ddi.Length > 3 || !ddi.All(char.IsDigit))
                return false;

            // Extrai parte após DDI: "(XX) XXXXX-XXXX"
            string remainingPart = phone.Substring(firstSpaceIndex + 1).Trim();

            // Deve começar com '('
            if (!remainingPart.StartsWith("("))
                return false;

            // Encontra o ')' que fecha o DDD
            int closingParenIndex = remainingPart.IndexOf(')');
            if (closingParenIndex == -1)
                return false;

            // Extrai DDD (entre parênteses)
            string ddd = remainingPart.Substring(1, closingParenIndex - 1);
            
            // DDD deve ter exatamente 2 dígitos
            if (ddd.Length != 2 || !ddd.All(char.IsDigit))
                return false;

            // Extrai número (após ") ")
            string numberPart = remainingPart.Substring(closingParenIndex + 1).Trim();

            // Remove o hífen para validar os dígitos
            if (!numberPart.Contains('-'))
                return false;

            string[] numberParts = numberPart.Split('-');
            if (numberParts.Length != 2)
                return false;

            string firstPart = numberParts[0].Trim();
            string secondPart = numberParts[1].Trim();

            // Valida que ambas as partes são números
            if (!firstPart.All(char.IsDigit) || !secondPart.All(char.IsDigit))
                return false;

            // Segunda parte deve ter sempre 4 dígitos
            if (secondPart.Length != 4)
                return false;

            // Primeira parte pode ter:
            // - 5 dígitos (celular: 9XXXX)
            // - 4 dígitos (fixo: XXXX)
            if (firstPart.Length != 4 && firstPart.Length != 5)
                return false;

            // Se for celular (5 dígitos), o primeiro dígito deve ser 9
            if (firstPart.Length == 5 && firstPart[0] != '9')
                return false;

            return true;
        }

        /// <summary>
        /// Valida se o CEP é válido (8 dígitos).
        /// </summary>
        private bool BeValidZipCode(string zipCode)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
                return false;

            return zipCode.IsCep();
        }

        /// <summary>
        /// Valida se o estado é uma UF brasileira válida.
        /// </summary>
        private bool BeValidBrazilianState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                return false;

            // Lista de UFs válidas do Brasil
            var validStates = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
                "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
                "RS", "RO", "RR", "SC", "SP", "SE", "TO"
            };

            return validStates.Contains(state.ToUpperInvariant());
        }
    }
}