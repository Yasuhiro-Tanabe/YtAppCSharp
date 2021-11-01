using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases

{
    public class DetailViewModelBase : TabItemControlViewModelBase
    {
        protected DetailViewModelBase(string header) : base(header) { }

        #region コマンド
        public ICommand Reload { get; } = new ReloadDetailCommand();
        #endregion // コマンド

        public virtual void Validate() { throw new NotImplementedException($"{GetType().Name}.{nameof(Validate)}()"); }
        public virtual void Update() { throw new NotImplementedException($"{this.GetType().Name}.{nameof(Update)}()"); }
    }
}
