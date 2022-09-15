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

    private void UpdateTheme()
    {
        FrameworkElement windowContent = (FrameworkElement)Settings.window.WindowContent;
        switch (ThemeSelectionBox.SelectedIndex)
        {
            case 0:
                windowContent.RequestedTheme = ElementTheme.Light;
                break;
            case 1:
                windowContent.RequestedTheme = ElementTheme.Dark;
                break;
            case 2:
                bool IsDarkTheme = (bool)Application.Current.Resources["IsDarkTheme"];
                windowContent.RequestedTheme = IsDarkTheme ? ElementTheme.Dark : ElementTheme.Light;
                windowContent.RequestedTheme = ElementTheme.Default;
                break;
            default:
                ThemeSelectionBox.SelectedIndex = 2;
                windowContent.RequestedTheme = ElementTheme.Default;
                break;
        }
    }

    private void ThemeSelected(object sender, RoutedEventArgs e)
    {
        if (!this.IsLoaded)
        {
            return;
        }
        UpdateTheme();
        Settings.SaveValue("theme", ThemeSelectionBox.SelectedIndex);
    }

    private void UpdateGamemodeExanderContent(bool wasSelected = false)
    {
        if (!this.IsLoaded)
        {
            return;
        }
        if (wasSelected)
        {
            Settings.SaveValue("gamemode", GamemodeSelectionBox.SelectedIndex);
        }
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
        UpdateGamemodeExanderContent(true);
        UpdatePlayerBoxes(true);
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
            }
            else
            {
                BoardRowSelection.IsEnabled = true;
                BoardColumnSelection.IsEnabled = true;
                WinPatternSelectionBox.IsEnabled = true;
            }

            BoardRowSelection.Value = Settings.boardSize.Y;
            BoardColumnSelection.Value = Settings.boardSize.X;
            WinPatternSelectionBox.SelectedIndex = Settings.winPattern;
        }
    }



    private void UpdateBotsExanderContent(bool wasSelected = false)
    {
        if (!this.IsLoaded)
        {
            return;
        }
        if (wasSelected)
        {
            Settings.SaveValue("botsSpeed", BotsSpeedSelectionBox.SelectedIndex);
        }
    }

    private void BotsExanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateBotsExanderContent();
    }

    private void BotsSpeedSelected(object sender, RoutedEventArgs e)
    {
        UpdateBotsExanderContent(true);
    }

    private void UpdatePlayerBoxes(bool bypass = false)
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

            if (this.IsLoaded || bypass)
            {
                if (Settings.gamemode == 1)
                {
                    Settings.SaveValue("numPlayers", (int)MultiplayerPlayersBox.Value);
                    Settings.SaveValue("numMultiplayerBots", (int)MultiplayerBotsBox.Value);
                }
                else if (Settings.gamemode == 2)
                {
                    Settings.SaveValue("numPlayers", (int)SpectatorBotsBox.Value);
                    Settings.SaveValue("numSpectatorBots", (int)SpectatorBotsBox.Value);
                }
            }
            
        }
    }

    private void DifficultySelected(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("difficulty", DifficultySelectionBox.SelectedIndex);
        }
    }

    private void BoardRowsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("boardRows", (float)e.NewValue);
            UpdatePlayerBoxes();
        }
    }

    private void BoardColumnsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("boardCols", (float)e.NewValue);
            UpdatePlayerBoxes();
        }
    }

    private void WinPatternChanged(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("winPattern", WinPatternSelectionBox.SelectedIndex);
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
            Settings.SaveValue("boardMode", BoardSelectionBox.SelectedIndex);

            if (BoardSelectionBox.SelectedIndex == 0)
            {
                Settings.boardSize.Y = Convert.ToInt32(Settings.GetValue("boardRows", true));
                Settings.boardSize.X = Convert.ToInt32(Settings.GetValue("boardCols", true));
                Settings.winPattern = (int)Settings.GetValue("winPattern", true);
            } else
            {
                Settings.boardSize.Y = Convert.ToInt32(Settings.GetValue("boardRows"));
                Settings.boardSize.X = Convert.ToInt32(Settings.GetValue("boardCols"));
                Settings.winPattern = (int)Settings.GetValue("winPattern");
            }
        }
        BoardExpander.IsExpanded = BoardSelectionBox.SelectedIndex != 0;
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

    private void OnTopToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("alwaysOnTop", OnTopToggleSwitch.IsOn);
        }
    }

    private void OnClearSquaresToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("clearlyPressedSquares", ClearlyUsedSquaresToggleSwitch.IsOn);
        }
    }

    private void TimerToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("matchTimerEnabled", TimerToggleSwitch.IsOn);
        }
    }

    private void SquaresInfoToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("boardInfoEnabled", SquaresInfoToggleSwitch.IsOn);
        }
    }

    private void PlayerCounterToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("playerCounterEnabled", PlayerCounterToggleSwitch.IsOn);
        }
    }

    private void LoadSettings()
    {
        try
        {
            ThemeSelectionBox.SelectedIndex = Settings.theme;
            OnTopToggleSwitch.IsOn = Settings.alwaysOnTop;
            ClearlyUsedSquaresToggleSwitch.IsOn = Settings.clearlyPressedSquares;

            GamemodeSelectionBox.SelectedIndex = Settings.gamemode;
            BoardSelectionBox.SelectedIndex = Settings.boardMode;
            MultiplayerPlayersBox.Value = Settings.numPlayers;
            MultiplayerBotsBox.Value = Settings.numMultiplayerBots;
            SpectatorBotsBox.Value = Settings.numSpectatorBots;
            DifficultySelectionBox.SelectedIndex = Settings.difficulty;
            BotsSpeedSelectionBox.SelectedIndex = Settings.botsSpeed;

            if (Settings.boardMode == 0)
            {
                BoardRowSelection.Value = (int)Settings.GetValue("boardRows", true);
                BoardColumnSelection.Value = (int)Settings.GetValue("boardCols", true);
                WinPatternSelectionBox.SelectedIndex = (int)Settings.GetValue("winPattern", true);
            }
            else
            {
                BoardRowSelection.Value = Settings.boardSize.Y;
                BoardColumnSelection.Value = Settings.boardSize.X;
                WinPatternSelectionBox.SelectedIndex = Settings.winPattern;
            }

            TimerToggleSwitch.IsOn = Settings.matchTimerEnabled;
            SquaresInfoToggleSwitch.IsOn = Settings.boardInfoEnabled;
            PlayerCounterToggleSwitch.IsOn = Settings.playerCounterEnabled;

            UpdatePlayerBoxes(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private void ResetGameSettingsClick(object sender, RoutedEventArgs e)
    {
        ResetGameSettingsButton.Flyout.Hide();
        GamemodeExpander.IsExpanded = false;
        BoardExpander.IsExpanded = false;
        Settings.Load(true);
        LoadSettings();
    }
}
