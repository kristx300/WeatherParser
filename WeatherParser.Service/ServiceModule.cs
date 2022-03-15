﻿using Autofac;
using WeatherParser.Contract;
using WeatherParser.Repository;

namespace WeatherParser.Service
{
    public class ServiceModulee : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<WeatherDataService>().As<IWeatherParserService>();

            builder.RegisterModule<RepositoryModule>();
        }
    }
}