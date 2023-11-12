using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.UI.Interactivity;

namespace AutoSplitDebugger.Behaviors;

public class DataGridSelectionBehavior : Behavior<DataGrid>
{
    public static readonly DependencyProperty ClearOnEscapeProperty =
        DependencyProperty.Register(nameof(ClearOnEscape), typeof(bool), typeof(DataGridSelectionBehavior), new (true));

    public bool ClearOnEscape
    {
        get { return (bool)GetValue(ClearOnEscapeProperty); }
        set { SetValue(ClearOnEscapeProperty, value); }
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        AssociatedObject.MouseUp += OnDataGridMouseUp;
        AssociatedObject.KeyUp += OnDataGridKeyUp;
    }

    protected override void OnDetaching()
    {
        AssociatedObject.MouseUp -= OnDataGridMouseUp;
        AssociatedObject.KeyUp -= OnDataGridKeyUp;

        base.OnDetaching();
    }
    
    private void OnDataGridMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (e.OriginalSource is ScrollViewer)
        {
            AssociatedObject.UnselectAll();
        }
    }
    
    private void OnDataGridKeyUp(object sender, KeyEventArgs e)
    {
        if (!ClearOnEscape) return;
        if (e.Key == Key.Escape)
        {
            AssociatedObject.UnselectAll();
        }
    }
}
