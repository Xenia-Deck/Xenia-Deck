using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using XeniaDeck.Views;

namespace XeniaDeck
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ExtendsContentIntoTitleBar = true;

            SetTitleBar(AppTitleBar);
        }

        private void AppNavigationView_Navigate(Type navigationViewType, NavigationTransitionInfo navigationTransitionInfo)
        {
            var preNavigationViewType = ContentFrame.CurrentSourcePageType;

            if (!(navigationViewType is null || Equals(navigationViewType, preNavigationViewType)))
            {
                ContentFrame.Navigate(navigationViewType, null, navigationTransitionInfo);
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void AppNavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                AppNavigationView_Navigate(typeof(SettingsPage), args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var tag = args.SelectedItemContainer.Tag.ToString();

                if (tag is not null)
                {
                    var navigationPageType = Type.GetType(tag);

                    if (navigationPageType is not null)
                    {
                        AppNavigationView_Navigate(navigationPageType, args.RecommendedNavigationTransitionInfo);
                    }
                    else
                    {
                        throw new Exception("Failed to load page " + tag);
                    }
                }
            }
        }

        private void AppNavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            AppNavigationView.SelectedItem = AppNavigationView.MenuItems[0];
        }
    }
}