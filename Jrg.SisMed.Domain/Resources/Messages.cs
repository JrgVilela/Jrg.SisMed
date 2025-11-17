using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Resources
{
    public class Messages
    {
    }

    public static class MessageExtensions
    {
        /// <summary>
        /// Obtém a mensagem localizada com base em um enum de mensagem.
        /// </summary>
        public static LocalizedString For(this IStringLocalizer<Messages> localizer, Enum messageKey, params object[] args)
        {
            if (localizer == null)
                throw new ArgumentNullException(nameof(localizer));

            if (messageKey == null)
                throw new ArgumentNullException(nameof(messageKey));

            // Monta chave padrão: Organization_AlreadyExistsByCnpj
            var key = $"{messageKey.GetType().Name}_{messageKey}";

            var localized = args.Length > 0
            ? localizer[key, args]
            : localizer[key];

            // remove placeholders literais {0}, {1}, {2} se args não foram passados
            if (args.Length == 0 && localized.Value.Contains("{0}"))
                return new LocalizedString(localized.Name, localized.Value.Replace("{0}", string.Empty));

            return localized;
        }
    }

    public enum OrganizationMessage
    {
        AlreadyExistsByCnpj,
        AlreadyExistsByRazaoSocial,

        NotFound,

        TradeNameRequired,
        TradeNameMaxLength,

        LegalNameRequired,
        LegalNameMaxLength,

        CnpjRequired,
        CnpjInvalid,

        StateInvalid,
    }

    public enum CommonMessage
    {
        ArgumentNull_Generic,
        ArgumentNull,
        InvalidArgument,
        RequiredValidation,
        MaxLengthValidation,
        NotFound
    }

    public enum UserMessage
    {
        AlreadyExistsByEmail,
        EmailAlreadyExists,
        NotFound,
        NameRequired,
        NameMaxLength,
        EmailRequired,
        EmailInvalid,
        EmailMaxLength,
        PasswordRequired,
        PasswordMinLength,
        PasswordMaxLength,
        StateInvalid,
        MultipleUsersFound
    }
}
