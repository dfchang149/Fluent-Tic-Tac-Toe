using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    bool buttonsEnabled = true;

    public void MoveToGameSelection(object sender, RoutedEventArgs e)
    {
    }

    public void PlaySingleplayerClick(object sender, RoutedEventArgs e)
    {
        if (buttonsEnabled)
        {
            buttonsEnabled = false;
            PageContent.Visibility = Visibility.Collapsed; //remove this
            Game.Gamemode = (Game.gamemodes[0]);
            this.Frame.Navigate(typeof(Pages.PlayingPage), null, new DrillInNavigationTransitionInfo());
        }
    }

    private void PlayMultiplayerClick(object sender, RoutedEventArgs e)
    {
        if (buttonsEnabled)
        {
            buttonsEnabled = false;
            PageContent.Visibility = Visibility.Collapsed;
            PlayButton.IsEnabled = false;
            Game.Gamemode = (Game.gamemodes[1]);
            this.Frame.Navigate(typeof(Pages.PlayingPage), null, new DrillInNavigationTransitionInfo());
        }
    }

    private void SettingsButtonClick(object sender, RoutedEventArgs e)
    {
        if (buttonsEnabled)
        {
            buttonsEnabled = false;
            this.Frame.Navigate(typeof(Pages.GameSelectionPage), null, new DrillInNavigationTransitionInfo());
        }
    }
}
