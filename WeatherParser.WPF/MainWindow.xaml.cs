using System.Windows;
using Autofac;
using WeatherParser.WPF.ViewModels;

namespace WeatherParser.WPF;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IContainer _container;

    public MainWindow()
    {
        InitializeComponent();

        var builder = new ContainerBuilder();
        builder.RegisterModule<WPFModule>();
        _container = builder.Build();

        DataContext = _container.Resolve<MainWindowViewModel>();
    }
}