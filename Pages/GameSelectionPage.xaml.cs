using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class GameSelectionPage : Page
{
    public GameSelectionPage()
    {
        this.InitializeComponent();
        GamemodeExpander.Content = null;
    }

    private void UpdateGamemodeExanderContent()
    {
        if (GamemodeSelectionBox.SelectedIndex == 0)
        {
            //MultiplayerContent.Visibility = Visibility.Collapsed;
            //SingleplayerContent.Visibility = Visibility.Visible;
            GamemodeExpander.Content = SingleplayerContent;
        }
        else
        {
            GamemodeExpander.Content = MultiplayerContent;
            //SingleplayerContent.Visibility = Visibility.Collapsed;
            //MultiplayerContent.Visibility = Visibility.Visible;
        }
    }

    private void GamemodeExanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateGamemodeExanderContent();
    }

    private void GamemodeSelected(object sender, RoutedEventArgs e)
    {
        if (GamemodeExpander.IsExpanded)
        {
            UpdateGamemodeExanderContent();
        }
    }

    private void PlayerBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        
    }

    private void StartButtonClick(object sender, RoutedEventArgs e)
    {

    }

    private void TimerToggled(object sender, RoutedEventArgs e)
    {

    }

    private void SquaresInfoToggled(object sender, RoutedEventArgs e)
    {

    }

    private void PlayerCounterToggled(object sender, RoutedEventArgs e)
    {

    }
}
