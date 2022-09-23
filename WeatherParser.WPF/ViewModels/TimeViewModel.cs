namespace WeatherParser.WPF.ViewModels;

internal class TimeViewModel : NotifyPropertyChangedBase
{
    private int _currentTime;

    private bool _isChecked;

    private bool _isDateChecked;

    public int CurrentTime
    {
        get => _currentTime;
        set => OnPropertyChanged(value, ref _currentTime);
    }

    public bool IsDateChecked
    {
        get => _isDateChecked;
        set => OnPropertyChanged(value, ref _isDateChecked);
    }

    public bool IsChecked
    {
        get => _isChecked;
        set => OnPropertyChanged(value, ref _isChecked);
    }
}