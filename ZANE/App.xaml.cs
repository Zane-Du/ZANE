using Autofac;
using Autofac;
using AutoMapper;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using ZANE.ViewModels;
using SQL;
using SQL.Profiles;


using ZANE.Views;
using static Org.BouncyCastle.Math.EC.ECCurve;
using Application = System.Windows.Application;

namespace ZANE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        //DI(依赖注入)和IOC(控制反转)
        public static IContainer container { get; private set; }
        //暴露容器，供全局使用



        public App()
        {
             var builder = new ContainerBuilder();
             ConfigureServices(builder);
             container = builder.Build();



             var mainWindow = container.Resolve<MainWindow>();
             mainWindow?.Show();







        }

        public void ConfigureServices(ContainerBuilder builder)
        {
            builder.RegisterType<MainWindowViewModel>().SingleInstance();


            builder.RegisterType<MainWindow>();
            builder.RegisterType<SearchView>();
            builder.RegisterType<SqliteHelper>();
            //builder.RegisterType<SearchViewModel>();





            var config = new MapperConfiguration(profiles =>
            {
                profiles.AddProfile(new PumpTestProfile());
            });
            var mapper = config.CreateMapper();
            builder.RegisterInstance(mapper);
        }


    }

}
