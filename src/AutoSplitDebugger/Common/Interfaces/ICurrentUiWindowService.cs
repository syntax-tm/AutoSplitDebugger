using System.Windows;
using DevExpress.Mvvm;
using Wpf.Ui.Controls.Interfaces;
using Wpf.Ui.Mvvm.Contracts;

namespace AutoSplitDebugger.Interfaces;

public interface ISnackbarWindowService : ICurrentWindowService
{
    Window ActualWindow { get; }
    ISnackbarControl Snackbar { get; }
    ISnackbarService SnackbarService { get; }

    void SetSnackbarControl();
}
