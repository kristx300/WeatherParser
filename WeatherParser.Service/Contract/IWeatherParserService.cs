﻿using System;
using System.Collections.Generic;
using WeatherParser.Entities;

namespace WeatherParser.Contract
{
    public interface IWeatherParserService
    {
        bool SaveWeatherData(string url);
        Dictionary<DateTime, List<WeatherData>> GetWeatherData(DateTime targetDate);

    }
}
