using Autofac;
using Grpc.Core;
using Grpc.Net.Client;
using TimerSaveDataService;
using WeatherParser.GrpcService.Services;

namespace WeatherParser;

public class WeatherParserConsoleModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.Register(c => GrpcChannel.ForAddress("http://localhost:5000")).As<ChannelBase>().SingleInstance();

        builder.RegisterType<WeatherDataProtoGismeteo.WeatherDataProtoGismeteoClient>();

        builder.RegisterType<ITimerSaveData>().As<TimerSaveData>().SingleInstance();
    }
}