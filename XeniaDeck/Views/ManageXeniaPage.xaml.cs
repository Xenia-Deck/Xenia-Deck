using Microsoft.UI.Xaml.Controls;
using XeniaDeck.Core.ViewModels;

namespace XeniaDeck.Views
{
    public sealed partial class ManageXeniaPage : Page
    {
        private readonly ManageXeniaViewModel _viewModel;

        public ManageXeniaPage()
        {
            _viewModel = App.GetService<ManageXeniaViewModel>();
            InitializeComponent();
        }
    }
}
