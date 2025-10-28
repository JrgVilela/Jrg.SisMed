using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jrg.SisMed.Domain.Exceptions
{
    public class DomainValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public DomainValidationException(IEnumerable<string> errors)
            : base(BuildMessage(errors))
        {
            Errors = errors.ToArray();
        }

        private static string BuildMessage(IEnumerable<string> errors)
        {
            var list = errors?.Where(e => !string.IsNullOrWhiteSpace(e)).ToList() ?? new List<string>();
            return list.Count == 0
                ? "One or more domain validation errors occurred."
                : $"One or more domain validation errors occurred:{Environment.NewLine}- " + string.Join($"{Environment.NewLine}- ", list);
        }
    }
}
