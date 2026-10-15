using Microsoft.UI.Xaml.Controls;
using XeniaDeck.Core.ViewModels;

namespace XeniaDeck.Views
{
    public sealed partial class XeniaSettingsPage : Page
    {
        private readonly XeniaSettingsViewModel _viewModel;

        public XeniaSettingsPage()
        {
            _viewModel = App.GetService<XeniaSettingsViewModel>();
            InitializeComponent();
        }
    }
}
