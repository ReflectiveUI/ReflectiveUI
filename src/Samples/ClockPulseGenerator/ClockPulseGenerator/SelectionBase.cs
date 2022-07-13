using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClockPulseGenerator;

public abstract class SelectionBase<T>
{
    public event EventHandler? SelectionChanged;

    public SelectionBase()
    {
    }

    protected abstract List<T> GetOptions();

    private List<SelectionOption<T>> _options = new();
    public List<SelectionOption<T>> Options
    {
        get
        {
            var isExistingSelection = TryGetSelected(out var selected);
            _options = GetOptions()
                .Select(o => new SelectionOption<T>(o)
                {
                    Parent = this,
                    Selected = isExistingSelection && (o?.Equals(selected) ?? false)
                }).ToList();
            return _options;
        }
    }

    internal bool TryGetSelected(out T value)
    {
        var selection = _options.FirstOrDefault(o => o.Selected);
        if (selection == null)
        {
            value = default!;
            return false;
        }

        value = selection.Value;
        return true;
    }

    internal bool TrySelect(SelectionOption<T> selectable)
    {
        foreach (var option in Options)
        {
            if (option == selectable)
                option.Selected = true;
            else
                option.Selected = false;
        }

        SelectionChanged?.Invoke(this, EventArgs.Empty);

        return true;
    }

    internal bool TrySelect(T value)
    {
        foreach (var option in Options)
        {
            if (option.Value?.Equals(value) ?? false)
                option.Selected = true;
            else
                option.Selected = false;
        }
        return true;
    }
}
