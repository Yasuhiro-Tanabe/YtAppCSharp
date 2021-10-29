using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels
{
    public class ListViewModelBase : TabItemControlViewModelBase
    {
        public event EventHandler DetailViewOpening;

        protected ListViewModelBase(string header) : base(header) { }

        #region コマンド
        public ICommand Reload { get; } = new ReloadListCommand();
        public ICommand Selected { get; } = new SelectionChangedEventCommand();
        #endregion // コマンド

        private void OpenDetailView(object sender, EventArgs unused)
        {
            LogUtil.DEBULOG_MethodCalled(sender.GetType().Name);
            DetailViewOpening?.Invoke(this, null);
        }

        protected void RemoveSelectedItem(object sender, EventArgs unused)
        {
            LogUtil.DEBULOG_MethodCalled(sender.GetType().Name);
            RemoveSelectedItem(sender);
        }

        protected void Subscribe(ListItemViewModelBase view)
        {
            view.DetailViewOpening += OpenDetailView;
            view.SelectedItemRemoving += RemoveSelectedItem;
        }

        protected void Unsubscribe(ListItemViewModelBase view)
        {
            view.DetailViewOpening -= OpenDetailView;
            view.SelectedItemRemoving -= RemoveSelectedItem;
        }

        public virtual void LoadItems() { throw new NotImplementedException($"{this.GetType().Name}.{nameof(LoadItems)}()"); }
        protected virtual void RemoveSelectedItem(object sender) { throw new NotImplementedException($"{this.GetType().Name}.{nameof(RemoveSelectedItem)}({sender?.GetType()?.Name})"); }

        public virtual DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM) { throw new NotImplementedException($"{GetType().Name}.{nameof(OpenDetailView)}({mainVM.GetType().Name})"); }
    }
}
