using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValuedTime.Quick.Target.Commands
{
    public class StartActivityPrompt
    {
        private readonly Func<StartActivityPrompt, Task> _onSave;

        public StartActivityPrompt(
            Func<StartActivityPrompt, Task> onSave)
        {
            _onSave = onSave;
        }

        public TimeSpan? Estimate { get; set; }
        public string? Name { get; set; }

        public Task Start() => _onSave.Invoke(this);
    }
}
