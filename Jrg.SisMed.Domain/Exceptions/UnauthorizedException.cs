namespace Jrg.SisMed.Domain.Exceptions
{
    /// <summary>
    /// Exceção lançada quando há falha de autenticação ou autorização.
    /// </summary>
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message)
        {
        }

        public UnauthorizedException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
