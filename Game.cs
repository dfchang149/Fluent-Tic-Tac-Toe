using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Composition.Interactions;
using Microsoft.VisualBasic;

namespace Fluent_Tic_tac_toe;
internal class Game
{
    public int gamemode;
    public List<Player> players
    {
        get;
    }
    public List<Piece> pieces
    {
        get; set;
    }
    public List<Piece> winningPieces
    {
        get; set;
    }
    public Piece[,] board
    {
        get; set;
    }
    public Player winner
    {
        get; set;
    }
    public int turns
    {
        get; set;
    }
    public int time
    {
        get; set;
    }
    public bool started
    {
        get; set;
    }
    private bool timed
    {
        get; set;
    }

    public Game()
    {
        this.gamemode = Settings.gamemode;
        this.players = new List<Player>();

        // add in players
        var numPlayers = 1;
        var numBots = 1;

        if (gamemode == 1)
        {
            numPlayers = Settings.numPlayers;
            Debug.WriteLine(numPlayers);
            numBots = Settings.numMultiplayerBots;
        }
        else if (gamemode == 2)
        {
            numBots = Settings.numSpectatorBots;
            numPlayers = 0;
        }

        for (var i = 0; i < numPlayers; i++)
        {
            Player player = new Player(false);
            player.SetSymbol(i);
            this.players.Add(player);
        }
        
        // add in bots

        for (var i = 0; i < numBots; i++)
        {
            Player bot = new Player(true);
            bot.SetSymbol(i+numPlayers);
            this.players.Add(bot);
        }

        // initialize other vars
        this.board = new Piece[(int)Settings.boardSize.Y, (int)Settings.boardSize.X];
        this.winningPieces = new List<Piece>();
        this.pieces = new List<Piece>();
        this.winner = null;
        this.time = 0;
        this.started = false;
    }

    public void Start()
    {
        this.turns = 0;
        this.winner = null;
        this.time = 0;
        this.started = true;
    }

    public void Restart()
    {
        this.winner = null;
        this.winningPieces.Clear();
        this.pieces.Clear();
        this.turns = 0;
        this.board = new Piece[(int)Settings.boardSize.Y, (int)Settings.boardSize.X];
        this.time = 0;
        this.started = false;
        AlternatePlayers();
    }

    public void AlternatePlayers() // so the first player doesn't keep going first
    {
        Player FirstPlayer = this.players.First();
        this.players.Remove(FirstPlayer);
        this.players.Add(FirstPlayer);

        /* Decided not to use this because it would confuse players and their symbols
        for (var i = 0; i < this.players.Count(); i++)
        {
            this.players[i].SetSymbol(i);
        }
        */
    }

    public bool Won()
    {
        for (var r = 0; r < Settings.boardSize.Y; r++)
        {
            winningPieces.Clear();
            Piece initialPiece = board[r, 0];
            var foundWinner = true;
            if (initialPiece != null)
            {
                winningPieces.Add(initialPiece);

                for (var c = 1; c < Settings.boardSize.X; c++) // Checks horizontal wins
                {
                    Piece piece = board[r, c];
                    if (piece == null) { foundWinner = false; break; }
                    winningPieces.Add(piece);
                    if (!initialPiece.Matches(piece))
                    {
                        foundWinner = false;
                        break;
                    }
                }
                if (foundWinner)
                {
                    winner = initialPiece.player;
                    winner.wins++;
                    return true;
                }
            }
        }
        for (var c = 0; c < Settings.boardSize.X; c++)
        {
            winningPieces.Clear();
            Piece initialPiece = board[0, c];
            var foundWinner = true;
            if (initialPiece != null)
            {
                winningPieces.Add(initialPiece);

                for (var r = 1; r < Settings.boardSize.Y; r++) // Checks vertical wins
                {
                    Piece piece = board[r, c];
                    if (piece == null) { foundWinner = false; break; }
                    winningPieces.Add(piece);
                    if (!initialPiece.Matches(piece))
                    {
                        foundWinner = false;
                        break;
                    }
                }
                if (foundWinner)
                {
                    winner = initialPiece.player;
                    winner.wins++;
                    return true;
                }
            }
        }

        winningPieces.Clear();
        // Check diagonal wins
        if (board[1, 1] != null)
        {
            Piece initialPiece = null;
            var foundWinner = true;
            for (var i = 0; i < Settings.boardSize.Y && i < Settings.boardSize.X; i++)
            {
                Piece piece = board[i, i];
                if (piece == null) { foundWinner = false; break; }
                winningPieces.Add(piece);

                if (initialPiece == null)
                {
                    initialPiece = piece;
                }
                else
                {
                    if (!initialPiece.Matches(piece))
                    {
                        foundWinner = false;
                        break;
                    }
                }
            }
            if (foundWinner)
            {
                winner = initialPiece.player;
                winner.wins++;
                return true;
            }
            initialPiece = null;
            winningPieces.Clear();
            foundWinner = true;
            for (var i = 0; i < Settings.boardSize.Y && i < Settings.boardSize.X; i++)
            {
                Piece piece = board[i, (int)(Settings.boardSize.X - 1) - i];
                if (piece == null) { foundWinner = false; break; }
                winningPieces.Add(piece);

                if (initialPiece == null)
                {
                    initialPiece = piece;
                }
                else
                {
                    if (!initialPiece.Matches(piece))
                    {
                        foundWinner = false;
                        break;
                    }
                }

            }
            if (foundWinner)
            {
                winner = initialPiece.player;
                winner.wins++;
                return true;
            }
        }

        return false;
    }

    public bool Won2()
    {
        List<Piece> selectedPieces = new();
        bool won = false;
        int maxDiagonalPieces = (int)Math.Min(Settings.boardSize.X, Settings.boardSize.Y);

        void CheckSelectedPieces()
        {
            if(!won && selectedPieces.Count > 2)
            {
                if (Settings.winPattern == 0)// check for 3 in a row
                {
                    Debug.WriteLine("WON 3");
                    Debug.WriteLine(selectedPieces.Count);
                    won = selectedPieces.Count >= 3;
                }
                else // check for full row wins
                {
                    if (selectedPieces[0].row.Equals(selectedPieces[1].row)) // check for horizontal win
                    {
                        won = selectedPieces.Count >= Settings.boardSize.X;
                    }
                    else if (selectedPieces[0].col.Equals(selectedPieces[1].col)) // check for vertical win
                    {
                        won = selectedPieces.Count >= Settings.boardSize.Y;
                    }
                    else // check diagonal wins
                    {
                        won = selectedPieces.Count >= maxDiagonalPieces;
                    }
                }
                if (won)
                {
                    this.winner = selectedPieces.First().player;
                    this.winningPieces.Clear();
                    this.winningPieces.AddRange(selectedPieces);
                }
            }
        }
        void TryAddingSelectedPiece(Piece piece)
        {
            if (piece != null)
            {
                if (selectedPieces.Count > 0 && !selectedPieces.First().Matches(piece))
                {
                    CheckSelectedPieces();
                    selectedPieces.Clear();
                }
                selectedPieces.Add(piece);
            }
            else
            {
                CheckSelectedPieces();
                selectedPieces.Clear();
            }
        }

        // Gather selected pieces
        Debug.WriteLine("Checking horizontal");
        for (var r = 0; r < Settings.boardSize.Y; r++) // Finds horizontal pieces
        {
            selectedPieces.Clear();
            for (var c = 0; c < Settings.boardSize.X; c++)
            {
                TryAddingSelectedPiece(board[r, c]);
            }
            CheckSelectedPieces();
            if (won)
            {
                return true;
            }
        }

        Debug.WriteLine("Checking Vetical");
        for (var c = 0; c < Settings.boardSize.X; c++) // Finds vertical pieces
        {
            selectedPieces.Clear();
            for (var r = 0; r < Settings.boardSize.Y; r++)
            {
                TryAddingSelectedPiece(board[r, c]);
            }
            CheckSelectedPieces();
            if (won)
            {
                return true;
            }
        }

        Debug.WriteLine("Checking Diagonal right");
        for (var r = 0; r <= Settings.boardSize.Y-maxDiagonalPieces; r++) // Finds down-right diagonal pieces
        {
            for (var c = 0; c <= Settings.boardSize.X - maxDiagonalPieces; c++)
            {
                selectedPieces.Clear();
                for (var i = 0; i < maxDiagonalPieces; i++)
                {
                    TryAddingSelectedPiece(board[r + i, c + i]);
                }
                CheckSelectedPieces();
                if (won)
                {
                    return true;
                }
            }
        }

        Debug.WriteLine("Checking diagonal left");
        for (var r = 0; r <= Settings.boardSize.Y - maxDiagonalPieces; r++) // Finds down-left diagonal pieces
        {
            for (var c = 0; c <= Settings.boardSize.X - maxDiagonalPieces; c++)
            {
                selectedPieces.Clear();
                for (var i = 0; i < maxDiagonalPieces; i++)
                {
                    int row = (int)(Settings.boardSize.Y - r - 1) - i;
                    int col = c + i;
                    TryAddingSelectedPiece(board[row,col]);
                }
                CheckSelectedPieces();
                if (won)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool PlacePiece(Player player, int row, int col)
    {
        if (board[row, col] == null)
        {
            Piece piece = new Piece(player, row, col);
            board[row, col] = piece;
            this.pieces.Add(piece);
            this.turns++;
            return true;
        }
        return false;
    }

    public bool PlacePiece(int row, int col)
    {
        if (board[row, col] == null)
        {
            Piece piece = new Piece(GetCurrentPlayerTurn(), row, col);
            board[row, col] = piece;
            this.pieces.Add(piece);
            this.turns++;
            return true;
        }
        return false;
    }

    public bool ComputerTurn()
    {
        List<Vector2> spaces = GetEmptySpaces();
        if (spaces.Count > 0)
        {
            Vector2 selectedSpace = spaces[new Random().Next(spaces.Count)];
            return PlacePiece((int)selectedSpace.Y, (int)selectedSpace.X);
        }
        return false;
    }

    public bool IsDraw()
    {
        return this.winner == null && this.board.Length == this.turns;
    }

    public List<Vector2> GetEmptySpaces()
    {
        List<Vector2> spaces = new List<Vector2>();
        for (int r = 0; r < Settings.boardSize.Y; r++)
        {
            for (int c = 0; c < Settings.boardSize.X; c++)
            {
                if (board[r, c] == null)
                {
                    spaces.Add(new Vector2(c, r));
                }
            }
        }
        return spaces;
    }

    public Player GetCurrentPlayerTurn()
    {
        return players[turns % players.Count];
    }

    public Player GetFirstRealPlayer()
    {
        foreach (Player player in players)
        {
            if (!player.isComputer)
            {
                return player;
            }
        }
        return null;
    }

    public string GetGridName(int row, int col)
    {
        return "square" + ((row * 3) + col);
    }

    public int GetNumberOfRealPlayers() // amount of players that aren't a computer
    {
        var result = 0;
        foreach (Player player in this.players)
        {
            if (player.isComputer == false)
            {
                result++;
            }
        }
        return result;
    }
}

public class Player
{
    public static string[] symbols = { "x", "o"};
    
    public string name
    {
        get; set;
    }
    public int number
    {
        get;
    }
    public bool isComputer
    {
        get;
    }
    public string symbol
    {
        get; set;
    }
    public int wins;

    public static int playerNumber = 1;
    public static int botNum = 1;

    public Player(string name, bool isComputer)
    {
        new Player(isComputer);
        this.name = name;
    }

    public Player(bool isComputer)
    {
        if (isComputer)
        {
            this.name = "Bot " + botNum;
            this.number = botNum;
            botNum++;
        }
        else
        {
            this.name = "Player " + playerNumber;
            this.number = playerNumber;
            playerNumber++;
        }
        this.isComputer = isComputer;
        this.wins = 0;
    }

    public Player(string name)
    {
        new Player(false);
        this.name = name;
    }

    public void SetSymbol(int num)
    {
        this.symbol = num < symbols.Length ? symbols[num] : Convert.ToChar(63+num).ToString();
    }
}

public class Piece
{
    public Player player
    {
        get;
    }
    public int row
    {
        get;
    }
    public int col
    {
        get;
    }

    public Piece(Player player, int row, int col)
    {
        this.player = player;
        this.row = row;
        this.col = col;
    }

    public bool Matches(Piece otherPiece)
    {
        return this.player.Equals(otherPiece.player);
    }

    public int GetIndex()
    {
        return (int)((this.row * Settings.boardSize.X) + this.col);
    }
}
