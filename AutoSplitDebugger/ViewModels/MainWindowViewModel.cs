using DevExpress.Mvvm.POCO;
using log4net;
using System;
using System.IO;
using AutoSplitDebugger.Interfaces;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;

namespace AutoSplitDebugger.ViewModels;

[POCOViewModel]
public class MainWindowViewModel : ViewModelBase
{
    private const string DEFAULT_CONFIG_FILE = @"config.json";
    private const string DEFAULT_FILE_FILTER = @"Config files (*.json)|*.json|All files (*.*)|*.*";
    private const string WINDOW_TITLE_BASE = @"AutoSplitter Debugger";

    private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowViewModel));
    
    protected ISnackbarWindowService WindowService => GetService<ISnackbarWindowService>();
    protected ISnackbarService SnackbarService => GetService<ISnackbarService>();
    protected IOpenFileDialogService OpenFileDialogService => GetService<IOpenFileDialogService>();

    public virtual string WindowTitle { get; protected set; }
    public virtual AutoSplitViewModel AutoSplitVm { get; set; }

    protected MainWindowViewModel()
    {
        OpenFileDialogService.Title = $"Open Config | {WINDOW_TITLE_BASE}";
        OpenFileDialogService.AddExtension = true;
        OpenFileDialogService.Multiselect = false;
        OpenFileDialogService.CheckFileExists = true;
        OpenFileDialogService.CheckPathExists = true;
        OpenFileDialogService.Filter = DEFAULT_FILE_FILTER;

        LoadConfig(DEFAULT_CONFIG_FILE);
        
        WindowTitle = $"{AutoSplitVm.Config.Title} | {WINDOW_TITLE_BASE}";
    }

    public static MainWindowViewModel Create()
    {
        return ViewModelSource.Create(() => new MainWindowViewModel());
    }

    public void Open()
    {
        var result = OpenFileDialogService.ShowDialog();

        if (!result)
        {
            log.Info("User cancelled config open.");
            return;
        }

        LoadConfig(OpenFileDialogService.File.GetFullName());
    }

    public void LoadConfig(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (!File.Exists(fileName)) throw new FileNotFoundException($"Config file '{fileName}' not found.", fileName);

        AutoSplitVm = AutoSplitViewModel.Create(fileName);
    }

    public void OnLoaded()
    {
        WindowService.SetSnackbarControl();
    }
}