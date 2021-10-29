﻿using System;

namespace MemorieDeFleurs.UI.WPF.ViewModels.Bases

{
    public class DetailViewModelBase : TabItemControlViewModelBase
    {
        protected DetailViewModelBase(string header) : base(header) { }

        public virtual void Validate() { throw new NotImplementedException($"{GetType().Name}.{nameof(Validate)}()"); }
    }
}