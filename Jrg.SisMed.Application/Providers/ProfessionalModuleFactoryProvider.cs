using Jrg.SisMed.Application.Factories;
using Jrg.SisMed.Domain.Attributes;
using Jrg.SisMed.Domain.Enumerators;
using Jrg.SisMed.Domain.Interfaces.Factories;
using Jrg.SisMed.Domain.Interfaces.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jrg.SisMed.Application.Providers
{
    /// <summary>
    /// Provider responsável por fornecer a factory apropriada baseada no tipo de profissional.
    /// Usa reflexão e atributos para resolver factories dinamicamente.
    /// </summary>
    public class ProfessionalModuleFactoryProvider : IProfessionalFactoryProvider
    {
        private readonly Dictionary<ProfessionalType, IProfessionalModuleFactory> _factories;

        /// <summary>
        /// Inicializa uma nova instância do provider com as factories disponíveis.
        /// </summary>
        /// <param name="factories">Coleção de factories registradas via Dependency Injection.</param>
        /// <exception cref="ArgumentException">Quando uma factory não possui o atributo ProfessionalType.</exception>
        public ProfessionalModuleFactoryProvider(IEnumerable<IProfessionalModuleFactory> factories)
        {
            _factories = factories.ToDictionary(f => GetTypeFromFactory(f));
        }

        /// <summary>
        /// Extrai o tipo de profissional de uma factory usando o atributo ProfessionalType.
        /// </summary>
        /// <param name="factory">Factory a ser analisada.</param>
        /// <returns>Tipo de profissional que a factory cria.</returns>
        /// <exception cref="ArgumentException">Quando a factory não possui o atributo ProfessionalType.</exception>
        private static ProfessionalType GetTypeFromFactory(IProfessionalModuleFactory factory)
        {
            var attribute = factory.GetType()
                .GetCustomAttribute<ProfessionalTypeAttribute>();

            return attribute?.Type
                ?? throw new ArgumentException(
                    $"Factory {factory.GetType().Name} não possui o atributo [ProfessionalType].");
        }

        /// <summary>
        /// Obtém a factory apropriada para o tipo de profissional especificado.
        /// </summary>
        /// <param name="type">Tipo de profissional.</param>
        /// <returns>Factory correspondente ao tipo.</returns>
        /// <exception cref="InvalidOperationException">Quando não há factory registrada para o tipo.</exception>
        public IProfessionalModuleFactory GetFactory(ProfessionalType type)
        {
            if (_factories.TryGetValue(type, out var factory))
                return factory;

            throw new InvalidOperationException(
                $"Nenhuma factory registrada para o tipo de profissional: {type}");
        }
    }
}
