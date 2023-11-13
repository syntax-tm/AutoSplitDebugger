using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AutoSplitDebugger.Interfaces;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using Wpf.Ui.Controls;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace AutoSplitDebugger.Services;

public class SnackbarWindowService : WindowAwareServiceBase, ISnackbarWindowService
{
    public static readonly DependencyProperty ClosingCommandProperty =
        DependencyProperty.Register(nameof(ClosingCommand), typeof(ICommand), typeof(CurrentWindowService), new (null));

    public ICommand ClosingCommand
    {
        get { return (ICommand) GetValue(ClosingCommandProperty); }
        set { SetValue(ClosingCommandProperty, value); }
    }

    private ISnackbarControl _snackbar;
    private ISnackbarService _service;

    public ISnackbarControl Snackbar
    {
        get
        {
            if (_snackbar != null) return _snackbar;

            var window = GetActualWindow();
            var snackbar = window.FindChild<Snackbar>();

            // cache the snackbar if we were able to find it
            if (snackbar != null)
            {
                _snackbar = snackbar;
            }

            return snackbar;
        }
    }
    
    public ISnackbarService SnackbarService
    {
        get
        {
            if ( _service != null) return _service;

            var service = ServiceContainer.Default.GetService<ISnackbarService>();
            if (service != null)
            {
                _service = service;
            }

            return service;
        }
    }

    public void SetSnackbarControl()
    {
        var snackbar = Snackbar;

        // ensure that a snackbar control has been created first
        if (snackbar == null)
        {
            throw new InvalidOperationException($"The {nameof(Snackbar)} control has not been created.");
        }

        SnackbarService.SetSnackbarControl(snackbar);
    }

    public DXWindowState WindowState
    {
        get
        {
            var window = GetActualWindow();
            return window.WindowState switch
            {
                System.Windows.WindowState.Maximized => DXWindowState.Maximized,
                System.Windows.WindowState.Minimized => DXWindowState.Minimized,
                System.Windows.WindowState.Normal    => DXWindowState.Normal,
                _                                    => throw new InvalidOperationException()
            };
        }
        set
        {
            var window = GetActualWindow();
            window.WindowState = value switch
            {
                DXWindowState.Maximized => System.Windows.WindowState.Maximized,
                DXWindowState.Minimized => System.Windows.WindowState.Minimized,
                DXWindowState.Normal    => System.Windows.WindowState.Normal,
                _                       => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public void Close()
    {
        ActualWindow.Close();
    }

    public void Activate()
    {
        ActualWindow.Activate();
    }

    public void Hide()
    {
        ActualWindow.Hide();
    }

    public void Show()
    {
        ActualWindow.Show();
    }

    protected Window GetActualWindow()
    {
        if (ActualWindow == null)
        {
            UpdateActualWindow();
        }
        return ActualWindow;
    }

    protected override void OnActualWindowChanged(Window oldWindow)
    {
        oldWindow.Do(w => w.Closing -= OnClosing);
        ActualWindow.Do(w => w.Closing += OnClosing);
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
        ClosingCommand?.Execute(e);
    }
}
