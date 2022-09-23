using System;
using System.Windows.Threading;
using Google.Protobuf.WellKnownTypes;
using Serilog;
using WeatherParser.GrpcService.Services;

namespace WeatherParser.TimerSaveDataService;

public class TimerSaveData : ITimerSaveData
{
    private readonly ILogger _logger;
    private readonly WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient _weatherParserService;

    public TimerSaveData(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService, ILogger logger)
    {
        _weatherParserService = weatherParserService;
        _logger = logger;
    }

    public void SaveData()
    {
        var timer = new DispatcherTimer();

        SaveWeather();

        timer.Interval = TimeSpan.FromDays(1);
        timer.Tick += timer_Tick;
        timer.Start();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        SaveWeather();
    }

    private void SaveWeather()
    {
        try
        {
            _weatherParserService.SaveWeatherData(new Empty());
        }
        catch (Exception e)
        {
            _logger.Error($"SaveData have an error: {e.Message} StackTrace: {e.StackTrace}");
        }
    }
}