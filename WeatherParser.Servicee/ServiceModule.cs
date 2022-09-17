﻿using Autofac;
using WeatherParser.Repository;
using WeatherParser.Servicee.Contract.Graphics;

namespace WeatherParser.Service
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataHtmlAgilityPackServiceGismeteo>().As<IWeatherParserServiceGismeteo>();

            builder.RegisterModule<RepositoryModule>();
        }
    }
}
