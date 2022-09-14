using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        Navigate();
        MainGrid.Children.Remove(BackButton);
        Settings.windowContent = this.WindowContent;

        FrameworkElement windowContent = (FrameworkElement)Settings.windowContent;
    }

    private void BackButtonClick(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.CanGoBack)
        {
            ContentFrame.GoBack();
        }
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        if (ContentFrame.CanGoBack)
        {
            MainGrid.Children.Add(BackButton);
        }
        else
        {
            MainGrid.Children.Remove(BackButton);
        }
    }

    private void WindowActivated(object sender, WindowActivatedEventArgs e)
    {
        if (e.WindowActivationState == WindowActivationState.Deactivated)
        {
            TitleBarFill.Visibility = Visibility.Collapsed;
        }
        else
        {
            TitleBarFill.Visibility = Visibility.Visible;
        }
    }

    public void Navigate()
    {
        ContentFrame.Navigate(typeof(Pages.MainPage), null, new SuppressNavigationTransitionInfo());
    }
}

