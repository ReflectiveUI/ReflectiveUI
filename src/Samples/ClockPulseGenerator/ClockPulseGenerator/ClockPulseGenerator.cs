namespace ClockPulseGenerator;

public class ClockGenerator
{
    private readonly System.Timers.Timer _timer;

    private int _bpm;
    private int _baseAccumulater;
    private int _multiplierAccumulater;
    private const double BASE_MULTIPLIER = 8.0;

    public event EventHandler? BeatUpdated;

    public ClockGenerator()
    {
        _timer = new();
        _timer.AutoReset = true;
        ClockCyclesPerMinute = 60;
        _timer.Elapsed += TimerElapsed;
        Start = DoStart;
        ClockMultiplier.TrySelect(1);
        ClockMultiplier.SelectionChanged += (s, e) =>
        {
            // Reset the state to keep the clocks in sync
            Outputs.MultipliedClockIsHigh = false;
            Outputs.ClockIsHigh = false;
            _baseAccumulater = 0;
            _multiplierAccumulater = 0;
        };
    }

    public Outputs Outputs { get; } = new();
    public ClockMultiplierSelection ClockMultiplier { get; } = new();

    public Action? Start { get; private set; }
    public Action? Stop { get; private set; }

    private void DoStart()
    {
        _timer.Start();
        Start = null;
        Stop = DoStop;
    }

    private void DoStop()
    {
        _timer.Stop();
        Stop = null;
        Start = DoStart;
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs args)
    {
        ++_baseAccumulater;
        ++_multiplierAccumulater;

        if (_baseAccumulater >= BASE_MULTIPLIER)
        {
            _baseAccumulater = 0;
            Outputs.ClockIsHigh = !Outputs.ClockIsHigh;
        }

        if (ClockMultiplier.TryGetSelected(out double selectedMultiplier))
            selectedMultiplier = 1.0 / selectedMultiplier * BASE_MULTIPLIER;
        else
            selectedMultiplier = BASE_MULTIPLIER;

        if (_multiplierAccumulater >= selectedMultiplier)
        {
            _multiplierAccumulater = 0;
            Outputs.MultipliedClockIsHigh = !Outputs.MultipliedClockIsHigh;
        }

        BeatUpdated?.Invoke(this, EventArgs.Empty);
    }

    public int ClockCyclesPerMinute
    {
        get
        {
            return _bpm;
        }
        set
        {
            if (_bpm != value)
                _bpm = value;
            _timer.Interval = 60 * 1000 / _bpm / BASE_MULTIPLIER;
        }
    }
}

public class ClockMultiplierSelection : SelectionBase<double>
{
    protected override List<double> GetOptions()
    {
        return new()
        {
            0.25,
            0.5,
            //0.97,
            1,
            2,
            3,
            4,
        };
    }
}


public class Outputs
{
    internal bool ClockIsHigh { get; set; }
    public string ClockSignalOutput => ClockIsHigh ? "High" : "Low";
    internal bool MultipliedClockIsHigh { get; set; }
    public string MultipliedClockSignalOutput => MultipliedClockIsHigh ? "High" : "Low";
}

