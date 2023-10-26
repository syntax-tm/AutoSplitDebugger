using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using AutoSplitDebugger.Config;
using AutoSplitDebugger.Factories;
using AutoSplitDebugger.Interfaces;
using AutoSplitDebugger.Models;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using log4net;
using Newtonsoft.Json;
using Wpf.Ui.Common;
using Wpf.Ui.Mvvm.Contracts;
using Clipboard = System.Windows.Clipboard;

namespace AutoSplitDebugger.ViewModels;

[POCOViewModel]
public class AutoSplitViewModel : ViewModelBase
{
    private static readonly ILog log = LogManager.GetLogger(typeof(AutoSplitViewModel));
    
    protected ISnackbarService SnackbarService { get { return GetService<ISnackbarService>(); } }

    private readonly BackgroundWorker _refreshWorker;
    private Memory _memory;
    
    protected delegate void RefreshPointers(IList<IPointerViewModel> pointers);

    private readonly RefreshPointers _synchronousRefresh = SynchronousRefresh;
    private readonly RefreshPointers _parallelRefresh = ParallelRefresh;

    public virtual bool IsParallel { get; set; }
    public virtual bool IsSuspended { get; set; }
    public virtual bool IsAttached { get; set; }
    public virtual bool IsRunning { get; set; }
    public virtual bool HasSelectedPointer { get; set; }

    public virtual string Title { get; set; }

    public virtual AutoSplitConfig Config { get; protected set; }
    
    public virtual IPointerViewModel SelectedPointer { get; set; }
    public virtual List<IPointerViewModel> Pointers { get; protected set; }

    protected AutoSplitViewModel()
    {
        _refreshWorker = new () { WorkerSupportsCancellation = true };
        _refreshWorker.DoWork += RefreshWorkerOnDoWork;
        _refreshWorker.RunWorkerCompleted += RefreshWorkerOnRunWorkerCompleted;
    }
    
    protected AutoSplitViewModel(string configPath) : this()
    {
        LoadConfig(configPath);
    }

    protected AutoSplitViewModel(AutoSplitConfig config) : this()
    {
        LoadConfig(config);
    }

    public static AutoSplitViewModel Create()
    {
        return ViewModelSource.Create(() => new AutoSplitViewModel());
    }
    
    public static AutoSplitViewModel Create(string configPath)
    {
        return ViewModelSource.Create(() => new AutoSplitViewModel(configPath));
    }

    public static AutoSplitViewModel Create(AutoSplitConfig config)
    {
        return ViewModelSource.Create(() => new AutoSplitViewModel(config));
    }
    
    [Command(false)]
    public void LoadConfig(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));
        if (!File.Exists(fileName)) throw new FileNotFoundException($"Config file '{fileName}' does not exist.", fileName);

        var configJson = File.ReadAllText(fileName);
        var jsonSettings = new JsonSerializerSettings
        {
            PreserveReferencesHandling = PreserveReferencesHandling.All
        };
        var config = JsonConvert.DeserializeObject<AutoSplitConfig>(configJson, jsonSettings);

        LoadConfig(config);
    }

    [Command(false)]
    public void LoadConfig(AutoSplitConfig config)
    {
        if (config == null) throw new ArgumentNullException(nameof(config));

        // TODO: create and add support for JSON schema validation for config files 
        if (string.IsNullOrEmpty(config.Process)) throw new ArgumentNullException(nameof(config.Process));

        Config = config;
        Title = string.IsNullOrEmpty(config.Title) ? config.Title : config.Process;
        
        _memory = new (config.Process);

        var pointers = PointerFactory.CreatePointerViewModels(_memory, config);
        Pointers = new (pointers);

        Start();
    }

    public void CreateSnapshot()
    {
        // TODO: create and save a snapshot of the current pointer values
        try
        {
            _memory.SuspendProcess();

            var snapshot = new MemorySnapshot(Pointers);
            var json = JsonConvert.SerializeObject(snapshot, Formatting.Indented);

            Clipboard.SetText(json, TextDataFormat.Text);
            Clipboard.Flush();
        }
        catch (Exception e)
        {
            log.Error($"An error occurred attempting to create a snapshot. {e.Message}", e);
            throw;
        }
        finally
        {
            _memory.ResumeProcess();
        }
    }

    public void SuspendOrResumeProcess()
    {
        // this will toggle the suspend or resume of all threads in the target process
        try
        {
            if (IsSuspended)
            {
                _memory.ResumeProcess();
            }
            else
            {
                _memory.SuspendProcess();
            }

            IsSuspended = !IsSuspended;
        }
        catch (Exception e)
        {
            log.Error($"An error occurred attempting to suspend/resume the {Config.Process} process. {e.Message}", e);
        }
    }

    public void ToggleWorker()
    {
        // if worker is already busy, request cancel
        if (_refreshWorker.IsBusy)
        {
            Stop();
            return;
        }

        // worker was not running, so start it
        Start();
    }

    public void Start()
    {
        if (_refreshWorker.IsBusy) return;

        IsRunning = true;

        _refreshWorker.RunWorkerAsync();
    }

    public void Stop()
    {
        if (!_refreshWorker.IsBusy) return;

        _refreshWorker.CancelAsync();

        IsRunning = false;
    }

#region BackgroundWorker

    private async void RefreshWorkerOnDoWork(object sender, DoWorkEventArgs e)
    {
        if (!IsAttached)
        {
            if (_refreshWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            IsAttached = _memory.Attach();

            if (!IsAttached)
            {
                await Task.Delay(TimeSpan.FromSeconds(3));

                e.Cancel = true;
                return;
            }
        }

        while (!_refreshWorker.CancellationPending)
        {
            _parallelRefresh(Pointers);
        }

        e.Cancel = true;
    }

    private void RefreshWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        IsRunning = false;
    }

#endregion

    protected void OnSelectedPointerChanged()
    {
        HasSelectedPointer = SelectedPointer != null;
    }
    
    protected static void SynchronousRefresh(IList<IPointerViewModel> pointers)
    {
        foreach (var pointer in pointers)
        {
            pointer.Refresh();
        }
    }

    protected static void ParallelRefresh(IList<IPointerViewModel> pointers)
    {
        Parallel.ForEach(pointers, pointer =>
        {
            pointer.Refresh();
        });
    }
}
