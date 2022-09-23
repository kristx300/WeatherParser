using Autofac;
using WeatherParser.Service.GismeteoService.Contract;

namespace WeatherParser.Service.GismeteoService;

public class GismeteoServiceModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<WeatherDataHtmlAgilityPackServiceGismeteo>().As<IWeatherParserServiceGismeteo>();
    }
}