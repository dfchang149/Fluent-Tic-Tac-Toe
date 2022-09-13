using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.Graphics.Printing3D;
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
        LoadSettings();
        GamemodeExpander.Content = null;
    }

    /*
    private void ThemeSelected(object sender, RoutedEventArgs e)
    {
        if (!this.IsLoaded)
        {
            return;
        }
        switch (ThemeSelectionBox.SelectedIndex)
        {
            case 0:
                this.RequestedTheme = ElementTheme.Light;
                break;
            case 1:
                this.RequestedTheme = ElementTheme.Dark;
                break;
            case 2:
                this.RequestedTheme = ElementTheme.Default;
                break;
            default:
                ThemeSelectionBox.SelectedIndex = 2;
                this.RequestedTheme = ElementTheme.Default;
                break;
        }
        Settings.theme = ThemeSelectionBox.SelectedIndex;
    }
    */

    private void UpdateGamemodeExanderContent()
    {
        if (!this.IsLoaded)
        {
            return;
        }
        Settings.gamemode = GamemodeSelectionBox.SelectedIndex;
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

    private void UpdatePlayerBoxes()
    {
        if (MultiplayerPlayersBox != null && MultiplayerBotsBox != null && SpectatorBotsBox != null && MaxPlayersMultiplayerText != null && MaxPlayersSpectatorText != null)
        {
            var MaxPlayers = Settings.GetMaxPlayers();
            MaxPlayersMultiplayerText.Text = MaxPlayers.ToString();
            MaxPlayersSpectatorText.Text = MaxPlayers.ToString();
            MultiplayerPlayersBox.Maximum = MaxPlayers;
            MultiplayerBotsBox.Maximum = MaxPlayers - MultiplayerPlayersBox.Value;
            MultiplayerPlayersBox.Maximum = MaxPlayers - MultiplayerBotsBox.Value;
            SpectatorBotsBox.Maximum = MaxPlayers;

            if (this.IsLoaded)
            {
                if (Settings.gamemode == 1)
                {
                    Debug.WriteLine("YES");
                    Settings.numPlayers = (int)MultiplayerPlayersBox.Value;
                    Settings.numMultiplayerBots = (int)MultiplayerBotsBox.Value;
                }
                else if (Settings.gamemode == 2)
                {
                    Settings.numSpectatorBots = (int)SpectatorBotsBox.Value;
                }
            }
            
        }
    }

    private void DifficultySelected(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.difficulty = DifficultySelectionBox.SelectedIndex;
        }
    }

    private void BoardRowsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.boardSize.Y = (float)e.NewValue;
            UpdatePlayerBoxes();
        }
    }

    private void BoardColumnsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.boardSize.X = (float)e.NewValue;
            UpdatePlayerBoxes();
        }
    }

    private void WinPatternChanged(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.winPattern = WinPatternSelectionBox.SelectedIndex;
        }
    }

    private void BoardExanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateBoardExanderContent();
    }

    private void BoardSelected(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.boardMode = BoardSelectionBox.SelectedIndex;
        }
        UpdateBoardExanderContent();
    }

    private void PlayerBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        UpdatePlayerBoxes();
    }

    private void BotsBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        UpdatePlayerBoxes();
    }

    private void TimerToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.matchTimerEnabled = TimerToggleSwitch.IsOn;
        }
    }

    private void SquaresInfoToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.boardInfoEnabled = SquaresInfoToggleSwitch.IsOn;
        }
    }

    private void PlayerCounterToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.playerCounterEnabled = PlayerCounterToggleSwitch.IsOn;
        }
    }

    private void LoadSettings()
    {
        try
        {
            //ThemeSelectionBox.SelectedIndex = Settings.theme;

            GamemodeSelectionBox.SelectedIndex = Settings.gamemode;
            BoardSelectionBox.SelectedIndex = Settings.boardMode;
            MultiplayerPlayersBox.Value = Settings.numPlayers;
            MultiplayerBotsBox.Value = Settings.numMultiplayerBots;
            SpectatorBotsBox.Value = Settings.numSpectatorBots;
            DifficultySelectionBox.SelectedIndex = Settings.difficulty;
            BoardRowSelection.Value = Settings.boardSize.Y;
            BoardColumnSelection.Value = Settings.boardSize.X;
            WinPatternSelectionBox.SelectedIndex = Settings.winPattern;
            TimerToggleSwitch.IsOn = Settings.matchTimerEnabled;
            SquaresInfoToggleSwitch.IsOn = Settings.boardInfoEnabled;
            PlayerCounterToggleSwitch.IsOn = Settings.playerCounterEnabled;

            UpdatePlayerBoxes();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void ResetGameSettingsClick(object sender, RoutedEventArgs e)
    {
        ResetGameSettingsButton.Flyout.Hide();
        Settings.Reset();

        //ThemeSelectionBox.SelectedIndex = Settings.theme;

        GamemodeSelectionBox.SelectedIndex = Settings.gamemode;
        BoardSelectionBox.SelectedIndex = Settings.boardMode;
        MultiplayerPlayersBox.Value = Settings.numPlayers;
        MultiplayerBotsBox.Value = Settings.numMultiplayerBots;
        SpectatorBotsBox.Value = Settings.numSpectatorBots;
        DifficultySelectionBox.SelectedIndex = Settings.difficulty;
        BoardRowSelection.Value = Settings.boardSize.Y;
        BoardColumnSelection.Value = Settings.boardSize.X;
        WinPatternSelectionBox.SelectedIndex = Settings.winPattern;
        TimerToggleSwitch.IsOn = Settings.matchTimerEnabled;
        SquaresInfoToggleSwitch.IsOn = Settings.boardInfoEnabled;
        PlayerCounterToggleSwitch.IsOn = Settings.playerCounterEnabled;
    }
}
