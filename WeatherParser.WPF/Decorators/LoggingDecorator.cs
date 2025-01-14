﻿using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Serilog;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.Commands;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Decorators;

internal class LoggingDecorator : BaseCommandDecorator
{
    public LoggingDecorator(ILogger logger, ICommand command) : base(logger, command)
    {
    }

    public override void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
        DateTime? selectedDate,
        ObservableCollection<ISeries> series,
        SitePresentation selectedSite,
        ObservableCollection<TimeViewModel> times,
        ObservableCollection<Axis> xAxes)
    {
        _logger.Information($"{_command.GetType().Name} started");

        _command.Execute(weatherParserService, selectedDate, series, selectedSite, times, xAxes);

        _logger.Information($"{_command.GetType().Name} finished");
    }
}