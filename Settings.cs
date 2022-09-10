using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;

namespace Fluent_Tic_tac_toe;
internal class Settings
{
    public static string[] gamemodes = { "Singleplayer", "Multiplayer", "Spectator" };
    public static string gamemode = gamemodes.First();

    public static string[] difficulties = { "Easy", "Medium", "Hard" };
    public static string difficulty = difficulties.First();

    public static List<Player> players = new();
    public static int numPlayers = 0;
    public static int numBots = 0;

    public static string[] boardModes = { "Default", "Custom" };
    public static string boardMode = difficulties.First();
    public static Vector2 boardSize = new(3,3);

    public static string[] winPatterns = { "3 in a row", "Full row" };
    public static string winPattern = winPatterns.First();

    public static bool matchTimerEnabled = true;
    public static bool boardInfoEnabled = true;
    public static bool playerCounterEnabled = true;

}
