using Autofac;

namespace TimerSaveDataService;

public class TimerSaveDataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<TimerSaveData>().As<ITimerSaveData>();
    }
}