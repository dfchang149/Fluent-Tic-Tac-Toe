using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace Fluent_Tic_tac_toe;
internal class Settings
{
    public static string[] gamemodes = { "Singleplayer", "Multiplayer", "Spectator" };
    public static int gamemode;

    public static string[] difficulties = { "Easy", "Medium", "Hard" };
    public static int difficulty;

    public static List<Player> players = new();
    public static int numPlayers;
    public static int numSingleplayerBots;
    public static int numMultiplayerBots;
    public static int numSpectatorBots;

    public static string[] boardModes = { "Default", "Custom" };
    public static int boardMode;
    public static Vector2 boardSize;

    public static string[] winPatterns = { "3 in a row", "Full row" };
    public static int winPattern;

    public static string[] botsSpeeds = { "Short", "Default", "Long" };
    public static int botsSpeed;

    public static bool matchTimerEnabled;
    public static bool boardInfoEnabled;
    public static bool playerCounterEnabled;

    public static string[] themes = { "Light", "Dark", "System default" };
    public static int theme;

    public static bool alwaysOnTop;
    public static bool clearlyPressedSquares;
    public static bool limitBoardSize;

    public static MainWindow window;

    public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    public static StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

    public static void Load(bool useDefault = false)
    {
        theme = (int)GetValue("theme", useDefault);
        alwaysOnTop = (bool)GetValue("alwaysOnTop", useDefault);
        clearlyPressedSquares = (bool)GetValue("clearlyPressedSquares", useDefault);
        limitBoardSize = (bool)GetValue("limitBoardSize", useDefault);

        gamemode = (int)GetValue("gamemode", useDefault);
        boardMode = (int)GetValue("boardMode", useDefault);
        numPlayers = Convert.ToInt32(GetValue("numPlayers", useDefault));
        numSingleplayerBots = (int)GetValue("numSingleplayerBots", useDefault);
        numMultiplayerBots = Convert.ToInt32(GetValue("numMultiplayerBots", useDefault));
        numSpectatorBots = Convert.ToInt32(GetValue("numSpectatorBots", useDefault));

        difficulty = (int)GetValue("difficulty", useDefault);
        botsSpeed = (int)GetValue("botsSpeed", useDefault);

        var isDefaultBoardMode = boardMode == 0;
        var cols = Convert.ToInt32(GetValue("boardCols", isDefaultBoardMode));
        var rows = Convert.ToInt32(GetValue("boardRows", isDefaultBoardMode));
        boardSize = new Vector2(cols, rows);
        winPattern = (int)GetValue("winPattern", isDefaultBoardMode);

        matchTimerEnabled = (bool)GetValue("matchTimerEnabled", useDefault);
        boardInfoEnabled = (bool)GetValue("boardInfoEnabled", useDefault);
        playerCounterEnabled = (bool)GetValue("playerCounterEnabled", useDefault);

        Settings.window.SetIsAlwaysOnTop(alwaysOnTop);
        UpdateTheme();
    }

    public static int GetMaxPlayers() // returns max amount of players for current board size
    {
        int maxPlayers = (int)Math.Round((boardSize.X * boardSize.Y) / 4.5f);
        return maxPlayers;
    }

    public static bool SaveValue(string key, Object value = null)
    {
        try
        {
            value ??= GetValue(key, true);
            localSettings.Values[key] = value;
            //Debug.WriteLine("Saved | " + key + " : " + value.ToString());
            switch (key)
            {
                case "theme":
                    theme = (int)value;
                    UpdateTheme();
                    break;
                case "alwaysOnTop":
                    alwaysOnTop = (bool)value;
                    Settings.window.SetIsAlwaysOnTop(alwaysOnTop);
                    break;
                case "clearlyPressedSquares":
                    clearlyPressedSquares = (bool)value;
                    break;
                case "limitBoardSize":
                    limitBoardSize = (bool)value;
                    break;
                case "gamemode":
                    gamemode = (int)value;
                    break;
                case "boardMode":
                    boardMode = (int)value;
                    break;
                case "numPlayers":
                    numPlayers = (int)value;
                    break;
                case "numSingleplayerBots":
                    numSingleplayerBots = (int)value;
                    break;
                case "numMultiplayerBots":
                    numMultiplayerBots = (int)value;
                    break;
                case "numSpectatorBots":
                    numSpectatorBots = (int)value;
                    break;
                case "difficulty":
                    difficulty = (int)value;
                    break;
                case "botsSpeed":
                    botsSpeed = (int)value;
                    break;
                case "boardRows":
                    boardSize.Y = (float)value;
                    break;
                case "boardCols":
                    boardSize.X = (float)value;
                    break;
                case "winPattern":
                    winPattern = (int)value;
                    break;
                case "matchTimerEnabled":
                    matchTimerEnabled = (bool)value;
                    break;
                case "boardInfoEnabled":
                    boardInfoEnabled = (bool)value;
                    break;
                case "playerCounterEnabled":
                    playerCounterEnabled = (bool)value;
                    break;
                default:
                    break;
            }
        }
        catch (Exception)
        {
            return false;
        };
        return true;
    }

    public static Object GetValue(string key, bool useDefault = false)
    {
        var value = localSettings.Values[key];

        if (value != null && !useDefault) // retrieve saved data
        {
            return localSettings.Values[key];
        }
        else
        {
            switch (key)
            {
                case "theme":
                    return 2;
                case "alwaysOnTop":
                    return false;
                case "clearlyPressedSquares":
                    return false;
                case "limitBoardSize":
                    return false;
                case "gamemode":
                    return 0;
                case "boardMode":
                    return 0;
                case "numPlayers":
                    return 2;
                case "numSingleplayerBots":
                    return 1;
                case "numMultiplayerBots":
                    return 0;
                case "numSpectatorBots":
                    return 2;
                case "difficulty":
                    return 1;
                case "botsSpeed":
                    return 1;
                case "boardRows":
                    return 3;
                case "boardCols":
                    return 3;
                case "winPattern":
                    return 0;
                case "matchTimerEnabled":
                    return true;
                case "boardInfoEnabled":
                    return true;
                case "playerCounterEnabled":
                    return true;
                default:
                    return null;
            }
        }
    }

    private static void UpdateTheme()
    {
        FrameworkElement windowContent = (FrameworkElement)Settings.window.WindowContent;
        void RequestTheme(ElementTheme elementTheme)
        {
            if (windowContent.ActualTheme != elementTheme)
            {
                windowContent.RequestedTheme = elementTheme;
            }
        }
        switch (theme)
        {
            case 0:
                RequestTheme(ElementTheme.Light);
                break;
            case 1:
                RequestTheme(ElementTheme.Dark);
                break;
            case 2:
                bool IsDarkTheme = (bool)Application.Current.Resources["IsDarkTheme"];
                RequestTheme(IsDarkTheme ? ElementTheme.Dark : ElementTheme.Light);
                RequestTheme(ElementTheme.Default);
                break;
            default:
                RequestTheme(ElementTheme.Default);
                break;
        }
    }
}
