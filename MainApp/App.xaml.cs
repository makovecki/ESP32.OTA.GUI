using OTA.GUI.FindESP;
using OTA.GUI.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UICore.App;
using UICore.App.Themes;
using UICore.IoC;
using UICore.MainWindow;
using UICore.Navigation;

namespace MainApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var mainWindow = new MainWindow();
            IIoCService iocService = new IoCService
            {
                RegisterComponents = (ioc) =>
                {
                    ioc.RegisterType<ESPMessageService>().As<IESPMessageService>(true);
                    ioc.RegisterType<HTTPServer>().As<IHTTPServer>();
                }
            };

            iocService.Build();
            iocService.Resolve<IAppService>().SetTheme(ThemeType.Light);
            mainWindow.DataContext = iocService.Resolve<IMainWindowViewModel>();
            mainWindow.Show();

            iocService.Resolve<INavigationService>().NavigateTo<FindESPViewModel, FindESPView>();
        }
    }
}
