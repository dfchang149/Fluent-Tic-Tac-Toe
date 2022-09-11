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
using Windows.Security.Isolation;

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
        if (GamemodeExpander.IsExpanded)
        {
            if (GamemodeSelectionBox.SelectedIndex == 0)
            {
                GamemodeExpander.Content = SingleplayerContent;
            }
            else if (GamemodeSelectionBox.SelectedIndex == 1)
            {
                GamemodeExpander.Content = MultiplayerContent;
            }
            else
            {
                GamemodeExpander.Content = SpectatorContent;
            }
        } 
    }

    private void GamemodeExanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateGamemodeExanderContent();
    }

    private void GamemodeSelected(object sender, RoutedEventArgs e)
    {
        UpdateGamemodeExanderContent();
    }

    private void UpdateBoardExanderContent()
    {
        if (BoardExpander.IsExpanded)
        {
            BoardExpander.Content = BoardExpanderContent;
            if (BoardSelectionBox.SelectedIndex == 0)
            {
                BoardRowSelection.IsEnabled = false;
                BoardColumnSelection.IsEnabled = false;
                WinPatternSelectionBox.IsEnabled = false;

                BoardRowSelection.Value = Double.Parse(BoardRowSelection.PlaceholderText);
                BoardColumnSelection.Value = Double.Parse(BoardColumnSelection.PlaceholderText);
                WinPatternSelectionBox.SelectedIndex = 0;
            }
            else
            {
                BoardRowSelection.IsEnabled = true;
                BoardColumnSelection.IsEnabled = true;
                WinPatternSelectionBox.IsEnabled = true;
            }
        }
    }

    private void BoardRowsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {

    }

    private void BoardColumnsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {

    }

    private void WinPatternChanged(object sender, RoutedEventArgs e)
    {

    }

    private void BoardExanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateBoardExanderContent();
    }

    private void BoardSelected(object sender, RoutedEventArgs e)
    {
        UpdateBoardExanderContent();
    }

    private void PlayerBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {

    }

    private void BotsBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {

    }


    private void ResetGameSettingsClick(object sender, RoutedEventArgs e)
    {
        Settings.Reset();
        GamemodeSelectionBox.SelectedIndex = Settings.gamemode;
        BoardSelectionBox.SelectedIndex = Settings.boardMode;
        MultiplayerPlayersBox.Value = Settings.numPlayers;
        MultiplayerBotsBox.Value = Settings.numMultiplayerBots;
        SpectatorBotsBox.Value = Settings.numSpectatorBots;
        DifficultySelectionBox.SelectedIndex = Settings.difficulty;
        BoardRowSelection.Value = Settings.boardSize.Y;
        BoardColumnSelection.Value = Settings.boardSize.X;
        TimerToggleSwitch.IsOn = Settings.matchTimerEnabled;
        SquaresInfoToggleSwitch.IsOn = Settings.boardInfoEnabled;
        PlayerCounterToggleSwitch.IsOn = Settings.playerCounterEnabled;

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
