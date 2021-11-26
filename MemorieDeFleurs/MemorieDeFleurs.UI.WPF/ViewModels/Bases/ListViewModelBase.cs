using MemorieDeFleurs.Logging;
using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases
{
    public class ListViewModelBase : TabItemControlViewModelBase
    {
        public event EventHandler DetailViewOpening;

        protected ListViewModelBase(string header) : base(header) { }

        #region コマンド
        public ICommand Selected { get; } = new SelectionChangedEventHandler();
        #endregion // コマンド

        private void OpenDetailView(object sender, EventArgs unused)
        {
            LogUtil.DEBUGLOG_MethodCalled(sender.GetType().Name);
            DetailViewOpening?.Invoke(this, null);
        }

        protected void RemoveSelectedItem(object sender, EventArgs unused)
        {
            LogUtil.DEBUGLOG_MethodCalled(sender.GetType().Name);
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

        protected virtual void RemoveSelectedItem(object sender) { throw new NotImplementedException($"{this.GetType().Name}.{nameof(RemoveSelectedItem)}({sender?.GetType()?.Name})"); }

        public virtual DetailViewModelBase OpenDetailTabItem(MainWindowViiewModel mainVM) { throw new NotImplementedException($"{GetType().Name}.{nameof(OpenDetailView)}({mainVM.GetType().Name})"); }
    }
}
