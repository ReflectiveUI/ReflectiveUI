
using ValuedTime.App.Repository;
using ValuedTime.App.Services;
using ValuedTime.Domain.Aggregates;

namespace ValuedTime.Quick.Target;

[Display(Name = "Valued Time")]
public class ValuedTimeApp
{
    private Func<Today> _todayFactory;
    private readonly History _history;
    private readonly DataSeedService dataSeedService;

    public ValuedTimeApp(Func<Today> today, History history, DataSeedService dataSeedService, LifeValues lifeValues)
    {
        _todayFactory = today;
        _history = history;
        this.dataSeedService = dataSeedService;
        LifeValues = lifeValues;
    }

    [Display(AutoGenerateField = false)]
    public async Task LoadAsync()
    {
        var today = _todayFactory();
        await today.Init();
        Today = _todayFactory();
        await LifeValues.LoadAsync();
    }

    public async Task SeedData()
    {
        await dataSeedService.ResetAsync();
        await LoadAsync();
    }
    
    public async Task Clear()
    {
        await dataSeedService.ClearAsync();
        await LoadAsync();
    }

    public Today? Today { get; private set; }

    private History? History { get; set; }

    public LifeValues LifeValues { get; }
}
