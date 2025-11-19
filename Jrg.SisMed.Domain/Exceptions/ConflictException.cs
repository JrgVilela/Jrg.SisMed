namespace Jrg.SisMed.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando há uma tentativa de criar um recurso que já existe.
    /// </summary>
    public class ConflictException : Exception
    {
        /// <summary>
        /// Nome do recurso em conflito.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Campo/propriedade que está em conflito.
        /// </summary>
        public string ConflictField { get; }

        /// <summary>
        /// Valor que está em conflito.
        /// </summary>
        public object ConflictValue { get; }

        public ConflictException(string resourceName, string conflictField, object conflictValue)
            : base($"{resourceName} with {conflictField} '{conflictValue}' already exists.")
        {
            ResourceName = resourceName;
            ConflictField = conflictField;
            ConflictValue = conflictValue;
        }

        public ConflictException(string message)
            : base(message)
        {
            ResourceName = string.Empty;
            ConflictField = string.Empty;
            ConflictValue = string.Empty;
        }
    }
}
