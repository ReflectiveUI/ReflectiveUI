using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.Quick.Host
{
    public record AppHostSettings
    {
        public bool SupressIdProperties { get; init; } = true;
        public List<string> AdditionalNamespaces { get; init; } = new();
        public bool SuppressMethodsUsingCanPropertyPrefix { get; init; } = true;
        public bool SuppressNullProperties { get; init; } = true;
    }
}
