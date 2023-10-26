using System;
using System.IO;
using System.Windows;
using DevExpress.Mvvm;
using log4net;
using log4net.Config;
using Wpf.Ui.Mvvm.Services;

namespace AutoSplitDebugger;

public partial class App
{
    private static readonly ILog log = LogManager.GetLogger(typeof(App));

    private void App_OnStartup(object sender, StartupEventArgs args)
    {
        try
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            ServiceContainer.Default.RegisterService("SnackbarService", new SnackbarService());

            var window = new MainWindow();

            MainWindow = window;

            window.Show();

            log.Info("Application startup complete.");
        }
        catch (Exception e)
        {
            log.Fatal($"An error occurred during application startup. {e.Message}", e);

            Environment.Exit(-1);
        }
    }
}