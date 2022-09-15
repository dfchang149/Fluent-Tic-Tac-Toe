﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;
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

    public static bool matchTimerEnabled;
    public static bool boardInfoEnabled;
    public static bool playerCounterEnabled;

    public static string[] themes = {"Light","Dark","System default" };
    public static int theme;

    public static bool alwaysOnTop;

    public static MainWindow window;

    public static ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
    public static StorageFolder localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;

    public static void Load(bool useDefault = false)
    {
        theme = (int) GetValue("theme",useDefault);
        alwaysOnTop = (bool)GetValue("alwaysOnTop", useDefault);

        gamemode = (int)GetValue("gamemode", useDefault);
        boardMode = (int)GetValue("boardMode", useDefault);
        numPlayers = (int)GetValue("numPlayers", useDefault);
        numSingleplayerBots = (int)GetValue("numSingleplayerBots", useDefault);
        numMultiplayerBots = (int)GetValue("numMultiplayerBots", useDefault);
        numSpectatorBots = (int)GetValue("numSpectatorBots", useDefault);

        difficulty = (int)GetValue("difficulty", useDefault);
        var cols = Convert.ToInt32(GetValue("boardCols", useDefault));
        var rows = Convert.ToInt32(GetValue("boardRows", useDefault));
        boardSize = new Vector2(cols,rows);
        winPattern = (int)GetValue("winPattern", useDefault);
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
            value ??= GetValue(key,true);
            localSettings.Values[key] = value;
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

    public static Object GetValue(string key,bool useDefault = false)
    {
        var value = localSettings.Values[key];

        if (value != null && !useDefault) // retrieve saved data
        {
            return localSettings.Values[key];
        } else {
            switch (key)
            {
                case "theme":
                    return 2;
                case "alwaysOnTop":
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
        switch (theme)
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
                windowContent.RequestedTheme = ElementTheme.Default;
                break;
        }
    }
}
