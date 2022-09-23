﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Google.Protobuf.WellKnownTypes;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands;

internal class GetHumidityCommand : CommandBase, ICommand
{
    private readonly ILogger _logger;

    public GetHumidityCommand(ILogger logger)
    {
        _logger = logger;
    }

    public void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
        DateTime? selectedDate,
        ObservableCollection<ISeries> series,
        SitePresentation selectedSite,
        ObservableCollection<TimeViewModel> times,
        ObservableCollection<Axis> xAxes)
    {
        series.Clear();

        List<WeatherDataPresentation> weatherData = null;

        try
        {
            weatherData = GetLabelsAndResponse(
                weatherParserService.GetAllWeatherData(new WeatherDataRequest
                {
                    Date = DateTime.SpecifyKind((DateTime)selectedDate, DateTimeKind.Utc).ToTimestamp(),
                    SiteID = selectedSite.ID.ToString()
                }),
                xAxes,
                (DateTime)selectedDate);
        }
        catch (Exception ex)
        {
            _logger.Error($"{GetType().Name} have an exception with message: {ex.Message}");
        }

        if (weatherData != null)
            for (var i = 0; i < times.Count; ++i)
                if (times[i].IsChecked)
                {
                    var humValues = new List<double>();

                    foreach (var weather in weatherData)
                    foreach (var hum in weather.Weather)
                        humValues.Add(hum.Humidity[i]);
                    series.Add(new LineSeries<double> { Values = humValues, Name = $"{times[i].CurrentTime}.00" });
                }
    }
}