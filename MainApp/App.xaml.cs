using OTA.GUI.FindESP;
using OTA.GUI.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UI.Core.App;
using UI.Core.App.Themes;
using UI.Core.IoC;
using UI.Core.MainWindow;
using UI.Core.Navigation;

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
