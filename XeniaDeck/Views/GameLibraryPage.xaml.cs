using Microsoft.UI.Xaml.Controls;
using XeniaDeck.Core.ViewModels;

namespace XeniaDeck.Views
{
    public sealed partial class GameLibraryPage : Page
    {
        private readonly GameLibraryViewModel _viewModel;

        public GameLibraryPage()
        {
            _viewModel = App.GetService<GameLibraryViewModel>();
            InitializeComponent();
        }
    }
}
