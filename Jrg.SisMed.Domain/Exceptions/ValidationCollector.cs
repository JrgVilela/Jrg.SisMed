using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Exceptions
{
    public sealed class ValidationCollector
    {
        private readonly List<string> _errors = new();

        public IReadOnlyList<string> Errors => _errors;

        public void When(bool hasError, string message)
        {
            if (hasError && !string.IsNullOrWhiteSpace(message))
                _errors.Add(message);
        }

        public void ThrowIfAny()
        {
            if (_errors.Count > 0)
                throw new DomainValidationException(_errors);
        }
    }
}
