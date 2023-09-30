using System;
using System.IO;
using System.Windows;
using AutoSplitDebugger.ViewModels;
using log4net;
using log4net.Config;

namespace AutoSplitDebugger;

public partial class App
{
    private static readonly ILog log = LogManager.GetLogger(typeof(App));

    private void App_OnStartup(object sender, StartupEventArgs args)
    {
        try
        {
            XmlConfigurator.Configure(new FileInfo("log4net.config"));

            var vm = MainWindowViewModel.Create();
            var window = new MainWindow
            {
                DataContext = vm
            };

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