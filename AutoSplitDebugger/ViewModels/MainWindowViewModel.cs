using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using AutoSplitDebugger.Config;
using AutoSplitDebugger.Factories;
using AutoSplitDebugger.Interfaces;
using DevExpress.Mvvm.POCO;
using log4net;
using Newtonsoft.Json;

namespace AutoSplitDebugger.ViewModels;

public class MainWindowViewModel
{
    private const string PROCESS_NAME = @"re5dx9";

    private static readonly ILog log = LogManager.GetLogger(typeof(App));
    
    private readonly BackgroundWorker _refreshWorker;
    private readonly Memory _memory;
    
    public virtual bool IsSuspended { get; set; }
    public virtual bool IsAttached { get; set; }
    public virtual bool IsRunning { get; set; }
    public virtual string WindowTitle { get; protected set; }

    public virtual AutoSplitConfig Config { get; set; }

    public virtual IPointerViewModel SelectedPointer { get; set; }
    public virtual List<IPointerViewModel> Pointers { get; }

    protected MainWindowViewModel()
    {
        WindowTitle = $"{PROCESS_NAME} | AutoSplitter Debugger";
        
        _memory = new (PROCESS_NAME);

        var configJson = File.ReadAllText("config.json");
        var jsonSettings = new JsonSerializerSettings();
        jsonSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
        var config = JsonConvert.DeserializeObject<AutoSplitConfig>(configJson);

        Config = config;

        var pointers = PointerFactory.CreatePointerViewModels(_memory, config);
        Pointers = new (pointers);

        _refreshWorker = new () { WorkerSupportsCancellation = true };
        _refreshWorker.DoWork += RefreshWorkerOnDoWork;
        _refreshWorker.RunWorkerCompleted += RefreshWorkerOnRunWorkerCompleted;

        Start();
    }

    public static MainWindowViewModel Create()
    {
        return ViewModelSource.Create(() => new MainWindowViewModel());
    }

    public void SuspendOrResumeProcess()
    {
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
            log.Error($"An error occurred attempting to suspend/resume the {PROCESS_NAME} process. {e.Message}", e);
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
            foreach (var pointer in Pointers)
            {
                if (_refreshWorker.CancellationPending) break;

                pointer.Refresh();
            }
        }

        e.Cancel = true;
    }

    private void RefreshWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        IsRunning = false;
    }
}