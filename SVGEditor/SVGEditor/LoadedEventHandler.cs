using System.Windows;

using YasT.Framework.WPF;

namespace SVGEditor
{
    internal class LoadedEventHandler : CommandBase<RoutedEventArgs>
    {
        private MainWindowViewModel _viewModel;

        public LoadedEventHandler(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        protected override void Execute(RoutedEventArgs args)
        {
            var view = (args as RoutedEventArgs).Source as MainWindow;

            _viewModel.LoadFileNameSelected += view.LoadFile;
            _viewModel.SaveFileNameSelected += view.SaveFile;
            _viewModel.RenderCalled += view.UpdateImage;
        }
    }
}
