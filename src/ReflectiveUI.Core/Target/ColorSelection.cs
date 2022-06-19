using QuickApp.Support;
using ValuedTime.Quick.Support;

namespace ValuedTime.Quick.Target
{
    public class ColorSelection : SelectionBase<ColorOption>
    {
        protected override List<ColorOption> GetOptions()
        {
            return new List<ColorOption>()
            {
                new("Red", "#ff0000"),
                new("Green", "#00ff00"),
                new("Blue", "#0000ff"),
            };
        }
    }
}
