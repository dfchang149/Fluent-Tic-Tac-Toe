using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        LoadSettings();
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

    private void ClearGamemodeExpanderContent()
    {
        if (GamemodeExpanderContent.Children.Contains(PlayersCard))
        {
            GamemodeExpanderContent.Children.Remove(PlayersCard);
        }
        if (GamemodeExpanderContent.Children.Contains(BotsCard))
        {
            GamemodeExpanderContent.Children.Remove(BotsCard);
        }
        if (GamemodeExpanderContent.Children.Contains(MaxPlayersCard))
        {
            GamemodeExpanderContent.Children.Remove(MaxPlayersCard);
        }
    }

    private void UpdateMaxPlayersText()
    {
        if(MaxPlayersText != null)
        {
            int maxPlayers = Settings.GetMaxPlayers();  
            MaxPlayersText.Text = maxPlayers.ToString();
            BotsBox.Maximum = maxPlayers;
            PlayersBox.Maximum = maxPlayers;
        }
    }

    private void LoadPlayerBoxes()
    {
        if (GamemodeExpander.IsExpanded)
        {
            if (Settings.gamemode == 0)
            {
                PlayersBox.Value = 1;
                BotsBox.Value = 1;
            }
            else if (Settings.gamemode == 1)
            {
                BotsBox.Value = Settings.numMultiplayerBots;
                PlayersBox.Value = Settings.numPlayers;
            }
            else if (Settings.gamemode == 2)
            {
                BotsBox.Value = Settings.numSpectatorBots;
            }
        }
            
    }

    private void UpdatePlayerBoxesV2(bool bypass = false)
    {
        if (!this.IsLoaded)
        {
            return;
        }
        if (GamemodeSelectionBox.SelectedIndex == 1)
        {
            int maxPlayers = Settings.GetMaxPlayers();
            BotsBox.Minimum = 0;
            PlayersBox.Minimum = 2;
            PlayersBox.Maximum = maxPlayers;
            BotsBox.Maximum = maxPlayers - PlayersBox.Value;
            PlayersBox.Maximum = maxPlayers - BotsBox.Value;
            
            Settings.SaveValue("numPlayers", (int)PlayersBox.Value);
            Settings.SaveValue("numMultiplayerBots", (int)BotsBox.Value);
        }
        else if (GamemodeSelectionBox.SelectedIndex == 2)
        {
            BotsBox.Minimum = 2;
            BotsBox.Maximum = Settings.GetMaxPlayers();
            Settings.SaveValue("numSpectatorBots", (int)BotsBox.Value);
        }
    }

    private void UpdateGamemodeExpanderContent(bool wasSelected = false)
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
            ClearGamemodeExpanderContent();
            if (GamemodeSelectionBox.SelectedIndex == 0)
            {
                PlayersBox.Minimum = 0;
                BotsBox.Minimum = 0;

                GamemodeExpanderContent.Children.Add(PlayersCard);
                GamemodeExpanderContent.Children.Add(BotsCard);

                LoadPlayerBoxes();

                PlayersBox.IsEnabled = false;
                BotsBox.IsEnabled = false;
            }
            else if (GamemodeSelectionBox.SelectedIndex == 1)
            {
                GamemodeExpanderContent.Children.Add(PlayersCard);
                GamemodeExpanderContent.Children.Add(BotsCard);
                GamemodeExpanderContent.Children.Add(MaxPlayersCard);

                UpdateMaxPlayersText();
                LoadPlayerBoxes();

                int maxPlayers = Settings.GetMaxPlayers();
                BotsBox.Minimum = 0;
                PlayersBox.Minimum = 2;
                PlayersBox.Maximum = maxPlayers;
                BotsBox.Maximum = maxPlayers - PlayersBox.Value;
                PlayersBox.Maximum = maxPlayers - BotsBox.Value;


                PlayersBox.IsEnabled = true;
                BotsBox.IsEnabled = true;
            }
            else
            {
                GamemodeExpanderContent.Children.Add(BotsCard);
                GamemodeExpanderContent.Children.Add(MaxPlayersCard);

                UpdateMaxPlayersText();

                BotsBox.IsEnabled = true;
                BotsBox.Minimum = 2;
                BotsBox.Maximum = Settings.GetMaxPlayers();

                LoadPlayerBoxes();
            }
        }
    }

    private void GamemodeExpanderExpanded(Expander sender, ExpanderExpandingEventArgs e)
    {
        UpdateGamemodeExpanderContent();
    }

    private void GamemodeExpanderCollapsed(Expander sender, ExpanderCollapsedEventArgs e)
    {
        ClearGamemodeExpanderContent();
    }

    private void GamemodeExpanderLoaded(object sender, RoutedEventArgs e)
    {
        ClearGamemodeExpanderContent();
    }

    private void GamemodeSelected(object sender, RoutedEventArgs e)
    {
        LoadPlayerBoxes();
        UpdateGamemodeExpanderContent(true);
        //UpdatePlayerBoxesV2();
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

    private void PlayerBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        UpdatePlayerBoxesV2();
    }

    private void BotsBoxChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        UpdatePlayerBoxesV2();
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
            UpdatePlayerBoxesV2();
        }
    }

    private void BoardColumnsChanged(NumberBox sender, NumberBoxValueChangedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("boardCols", (float)e.NewValue);
            UpdatePlayerBoxesV2();
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
            }
            else
            {
                Settings.boardSize.Y = Convert.ToInt32(Settings.GetValue("boardRows"));
                Settings.boardSize.X = Convert.ToInt32(Settings.GetValue("boardCols"));
                Settings.winPattern = (int)Settings.GetValue("winPattern");
            }
        }
        BoardExpander.IsExpanded = BoardSelectionBox.SelectedIndex != 0;
        UpdateBoardExanderContent();
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

    private void OnLimitSizeToggled(object sender, RoutedEventArgs e)
    {
        if (this.IsLoaded)
        {
            Settings.SaveValue("limitBoardSize", LimitSizeToggleSwitch.IsOn);
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
            LimitSizeToggleSwitch.IsOn = Settings.limitBoardSize;

            GamemodeSelectionBox.SelectedIndex = Settings.gamemode;
            BoardSelectionBox.SelectedIndex = Settings.boardMode;

            LoadPlayerBoxes();
            //MultiplayerPlayersBox.Value = Settings.numPlayers;
            //MultiplayerBotsBox.Value = Settings.numMultiplayerBots;
            //SpectatorBotsBox.Value = Settings.numSpectatorBots;
            DifficultySelectionBox.SelectedIndex = Settings.difficulty;
            BotsSpeedSelectionBox.SelectedIndex = Settings.botsSpeed;

            if (Settings.boardMode == 0)
            {
                Settings.boardSize.Y = Convert.ToInt32(Settings.GetValue("boardRows", true));
                Settings.boardSize.X = Convert.ToInt32(Settings.GetValue("boardCols", true));
                Settings.winPattern = (int)Settings.GetValue("winPattern", true);
            }

            BoardRowSelection.Value = Settings.boardSize.Y;
            BoardColumnSelection.Value = Settings.boardSize.X;
            WinPatternSelectionBox.SelectedIndex = Settings.winPattern;

            TimerToggleSwitch.IsOn = Settings.matchTimerEnabled;
            SquaresInfoToggleSwitch.IsOn = Settings.boardInfoEnabled;
            PlayerCounterToggleSwitch.IsOn = Settings.playerCounterEnabled;

            UpdatePlayerBoxesV2(true);
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
