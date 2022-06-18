using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.Domain.Abstractions
{
    public class DefaultGuidFactory : IGuidFactory
    {
        public Guid NewGuid()
        {
            return Guid.NewGuid();
        }
    }
}
