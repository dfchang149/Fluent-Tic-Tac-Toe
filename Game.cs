using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

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
            bot.SetSymbol(i + numPlayers);
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
        List<Piece> selectedPieces = new();
        bool won = false;
        int maxDiagonalPieces = (int)Math.Min(Settings.boardSize.X, Settings.boardSize.Y);

        void CheckSelectedPieces()
        {
            if (!won && selectedPieces.Count > 2)
            {
                if (Settings.winPattern == 0)// check for 3 in a row
                {
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

        for (var r = 0; r < Settings.boardSize.Y - 2; r++) // Finds down-right diagonal pieces
        {
            for (var c = 0; c < Settings.boardSize.X - 2; c++)
            {
                selectedPieces.Clear();
                for (var i = 0; i < Math.Min(Settings.boardSize.Y - r, Settings.boardSize.X - c); i++)
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

        for (var r = 0; r < Settings.boardSize.Y - 2; r++) // Finds down-left diagonal pieces
        {
            for (var c = 2; c < Settings.boardSize.X; c++)
            {
                selectedPieces.Clear();
                for (var i = 0; i < Math.Min(Settings.boardSize.Y - r, c + 1); i++)
                {
                    TryAddingSelectedPiece(board[r + i, c - i]);
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
            bool isWithinBoard(Vector2 location)
            {
                int clampedX = (int)Math.Clamp(location.X, 0, Settings.boardSize.X - 1);
                int clampedY = (int)Math.Clamp(location.Y, 0, Settings.boardSize.Y - 1);

                return (clampedX == location.X && clampedY == location.Y);
            }

            List<Vector2> directions = new(){
                new Vector2() {X=-1,Y=0},
                new Vector2() {X=-1,Y=-1},
                new Vector2() {X=0,Y=-1},
                new Vector2() {X=1,Y=-1},
            };

            if (Settings.difficulty == 0) // easy
            {
                Vector2 selectedSpace = spaces[new Random().Next(spaces.Count)]; // pick random spot
                return PlacePiece((int)selectedSpace.Y, (int)selectedSpace.X);
            }
            else if (Settings.difficulty == 1) // medium
            {
                Vector2 selectedSpace = spaces.First();
                int selectedSpotPriority = 0;
                int enemyPriority = 4;
                int allyPriority = 2;

                int evaluatePriority(Vector2 location)
                {
                    if (isWithinBoard(location))
                    {
                        Piece piece = board[(int)location.Y, (int)location.X];
                        if (piece != null)
                        {
                            if (piece.player == GetCurrentPlayerTurn())
                            {
                                return allyPriority;
                            }
                            else
                            {
                                return enemyPriority;
                            }
                        }
                    }
                    return 0;
                }

                foreach (Vector2 space in spaces)
                {
                    int priority = new Random().Next((int)(spaces.Count + Settings.boardSize.X + Settings.boardSize.Y));
                    foreach (Vector2 dir in directions)
                    {
                        Vector2 spot = space + dir;
                        Vector2 oppositeSpot = space - dir;
                        priority += evaluatePriority(spot) + evaluatePriority(oppositeSpot);
                    }
                    if (priority > selectedSpotPriority)
                    {
                        selectedSpace = space;
                        selectedSpotPriority = priority;
                    }
                }

                return PlacePiece((int)selectedSpace.Y, (int)selectedSpace.X);
            }
            else
            { // hard
                Vector2 selectedSpace = spaces[new Random().Next((int)spaces.Count)];
                int selectedSpotPriority = 0;
                int enemyPriority = 2;
                int allyPriority = 3;

                int evaluatePriority(Vector2 location)
                {
                    if (isWithinBoard(location))
                    {
                        Piece piece = board[(int)location.Y, (int)location.X];
                        if (piece != null)
                        {
                            if (piece.player == GetCurrentPlayerTurn())
                            {
                                return allyPriority;
                            }
                            else
                            {
                                return enemyPriority;
                            }
                        }
                    }
                    return 0;
                }

                foreach (Vector2 space in spaces)
                {
                    foreach (Vector2 dir in directions)
                    {
                        Vector2 spot = space + dir;
                        Vector2 oppositeSpot = space - dir;
                        int priority = 0;

                        void evaluateInDirection(Piece pieceToMatch)
                        {
                            while (evaluatePriority(spot) > 0 && board[(int)spot.Y, (int)spot.X] != null && board[(int)spot.Y, (int)spot.X].Matches(pieceToMatch))
                            {
                                priority += evaluatePriority(spot);
                                spot += dir;
                            }
                        }

                        void evaluateInOppositeDirection(Piece pieceToMatch)
                        {
                            while (evaluatePriority(oppositeSpot) > 0 && board[(int)oppositeSpot.Y, (int)oppositeSpot.X] != null && board[(int)oppositeSpot.Y, (int)oppositeSpot.X].Matches(pieceToMatch))
                            {
                                priority += evaluatePriority(oppositeSpot);
                                oppositeSpot -= dir;
                            }
                        }

                        if (isWithinBoard(spot) && isWithinBoard(oppositeSpot))
                        {
                            Piece spotPiece = board[(int)spot.Y, (int)spot.X];
                            Piece oppositeSpotPiece = board[(int)oppositeSpot.Y, (int)oppositeSpot.X];

                            if (spotPiece != null && oppositeSpotPiece != null && !spotPiece.Matches(oppositeSpotPiece))
                            {
                                continue;
                            }
                            else if (spotPiece == null && oppositeSpotPiece == null)
                            {
                                continue;
                            }

                            Piece mainPiece = spotPiece != null ? spotPiece : oppositeSpotPiece;

                            evaluateInDirection(mainPiece);
                            evaluateInOppositeDirection(mainPiece);
                        }
                        else if (isWithinBoard(spot))
                        {
                            Piece spotPiece = board[(int)spot.Y, (int)spot.X];
                            if (spotPiece != null)
                            {
                                evaluateInDirection(spotPiece);
                            }

                        }
                        else if (isWithinBoard(oppositeSpot))
                        {
                            Piece oppositeSpotPiece = board[(int)oppositeSpot.Y, (int)oppositeSpot.X];
                            if (oppositeSpotPiece != null)
                            {
                                evaluateInOppositeDirection(oppositeSpotPiece);
                            }
                        }

                        if (priority > selectedSpotPriority)
                        {
                            selectedSpace = space;
                            selectedSpotPriority = priority;
                        }
                    }
                }

                return PlacePiece((int)selectedSpace.Y, (int)selectedSpace.X);
            }
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
    public static string[] symbols = { "x", "o" };

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

    public Player(string name, bool isComputer = false)
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

    public void SetSymbol(int num)
    {
        this.symbol = num < symbols.Length ? symbols[num] : Convert.ToChar(63 + num).ToString();
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
