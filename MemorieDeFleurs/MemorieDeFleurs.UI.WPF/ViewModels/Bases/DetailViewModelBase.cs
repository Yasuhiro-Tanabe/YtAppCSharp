using MemorieDeFleurs.UI.WPF.Commands;

using System;
using System.Windows.Input;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases

{
    public class DetailViewModelBase : TabItemControlViewModelBase
    {
        protected DetailViewModelBase(string header) : base(header) { }

        #region コマンド
        public ICommand Clear { get; } = new ClearDetailCommand();
        #endregion // コマンド

        public virtual void Validate() { throw new NotImplementedException($"{GetType().Name}.{nameof(Validate)}()"); }
        public virtual void SaveToDatabase() { throw new NotImplementedException($"{GetType().Name}.{nameof(SaveToDatabase)}()"); }
        public virtual void ClearProperties() { throw new NotImplementedException($"{GetType().Name}.{nameof(ClearProperties)}()"); }
    }
}
