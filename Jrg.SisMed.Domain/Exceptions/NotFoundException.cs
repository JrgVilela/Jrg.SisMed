namespace Jrg.SisMed.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um recurso não é encontrado no sistema.
    /// </summary>
    public class NotFoundException : Exception
    {
        /// <summary>
        /// Nome do recurso que não foi encontrado.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Identificador do recurso que não foi encontrado.
        /// </summary>
        public object ResourceKey { get; }

        public NotFoundException(string resourceName, object resourceKey)
            : base($"{resourceName} with identifier '{resourceKey}' was not found.")
        {
            ResourceName = resourceName;
            ResourceKey = resourceKey;
        }

        public NotFoundException(string message)
            : base(message)
        {
            ResourceName = string.Empty;
            ResourceKey = string.Empty;
        }
    }
}
