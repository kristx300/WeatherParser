using System;
using Autofac;
using Google.Protobuf.WellKnownTypes;
using WeatherParser.GrpcService.Services;

namespace WeatherParser.ConsolePL;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<WeatherParserConsoleModule>();

        var container = builder.Build();

        var weatherParserService = container.Resolve<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

        try
        {
            weatherParserService.SaveWeatherData(new Empty());
        }
        catch
        {
            Console.WriteLine("Error");
        }

        //var result = weatherParserService.GetAllWeatherData(DateTime.UtcNow.ToTimestamp());

        Console.ReadLine();
    }
}