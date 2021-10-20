using System.Windows.Controls;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class TabItemContent
    {
        public string Header { get; set; }
        public int Index { get; set; }
        public UserControl Control { get; set; }
    }
}
