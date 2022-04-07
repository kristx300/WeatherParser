﻿using Autofac;
using WeatherParser.Repository;
using WeatherParser.Service.Contract;

namespace WeatherParser.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataHtmlAgilityPackService>().As<IWeatherParserService>();

            builder.RegisterModule<RepositoryModule>();
        }
    }
}
