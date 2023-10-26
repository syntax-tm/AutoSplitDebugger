using System.Windows.Controls;
using System.Windows.Input;

namespace AutoSplitDebugger.Views
{
    public partial class PointerGridView
    {
        public PointerGridView()
        {
            InitializeComponent();
        }

        // TODO: create a behavior to deselect rows when click is not a row or cell
        private void OnDataGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is ScrollViewer)
            {
                ((DataGrid) sender).UnselectAll();
            }
        }
        
        // TODO: create a behavior to deselect rows on ESC
        private void OnDataGridKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                ((DataGrid) sender).UnselectAll();
            }
        }
    }
}
