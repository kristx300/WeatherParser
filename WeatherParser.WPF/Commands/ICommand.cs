﻿using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using WeatherParser.GrpcService.Services;
using WeatherParser.Presentation.Entities;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF.Commands;

internal interface ICommand
{
    void Execute(WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient weatherParserService,
        DateTime? selectedDate,
        ObservableCollection<ISeries> series,
        SitePresentation selectedSite,
        ObservableCollection<TimeViewModel> times,
        ObservableCollection<Axis> xAxes);
}