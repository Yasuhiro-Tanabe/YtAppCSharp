using System;
using System.Windows;

namespace SVGEditor
{
    internal class LoadedEventHandler : CommandBase
    {
        private MainWindowViewModel _viewModel;

        public LoadedEventHandler(MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public override void Execute(object parameter)
        {
            var view = (parameter as RoutedEventArgs).Source as MainWindow;

            _viewModel.LoadFileNameSelected += view.LoadFile;
            _viewModel.SaveFileNameSelected += view.SaveFile;
            _viewModel.RenderCalled += view.UpdateImage;
        }
    }
}
