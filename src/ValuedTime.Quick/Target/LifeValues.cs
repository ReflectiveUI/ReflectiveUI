using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValuedTime.App.Models.Dto;
using ValuedTime.App.Services;

namespace ValuedTime.Quick.Target
{
    public class LifeValues
    {
        private readonly LifeValueService lifeValueService;

        public LifeValues(LifeValueService lifeValueService)
        {
            this.lifeValueService = lifeValueService;
        }

        internal async Task LoadAsync()
        {
            Values = await lifeValueService.GetAllLifeValues();
        }

        public List<LifeValue>? Values { get; internal set; }

        public AddLifeValueCommand AddValue()
        {
            var addCommand = new AddLifeValueCommand();
            addCommand.Add = async () =>
            {
                await lifeValueService.AddLifeValue(new CreateLifeValueCommand(addCommand.Name!));
                await LoadAsync();
            };
            return addCommand;
        }

        public class AddLifeValueCommand
        {
            public string? Name { get; set; }
            public ColorSelection Color { get; } = new ColorSelection();
            public Func<Task>? Add { get; set; }
        }
    }
}
