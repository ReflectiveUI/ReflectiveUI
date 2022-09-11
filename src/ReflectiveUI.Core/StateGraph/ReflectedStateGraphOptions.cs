using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflectiveUI.Core.ObjectGraph
{
    public record ReflectedStateGraphOptions
    {
        public bool SuppressIdProperties { get; init; } = true;
        public List<string> AdditionalNamespaces { get; init; } = new();
        public bool SuppressMethodsUsingCanPropertyPrefix { get; init; } = true;
        public bool SuppressNullProperties { get; init; } = true;
    }
}
