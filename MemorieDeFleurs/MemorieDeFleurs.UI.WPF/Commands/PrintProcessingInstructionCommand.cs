using MemorieDeFleurs.UI.WPF.ViewModels;
using MemorieDeFleurs.UI.WPF.Views;
using MemorieDeFleurs.UI.WPF.Views.Helpers;

namespace MemorieDeFleurs.UI.WPF.Commands
{
    /// <summary>
    /// 加工指示書印刷コマンド
    /// </summary>
    public class PrintProcessingInstructionCommand : PrintCommand<ProcessingInstructionViewModel>
    {
        /// <inheritdoc/>
        protected override void Execute(ProcessingInstructionViewModel parameter)
        {
            UserControlPrinter.PrintDocument<ProcessingInstructionViewModel, ProcessingInstructionControl>(parameter);
        }
    }
}
