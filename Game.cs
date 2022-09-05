using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent_Tic_tac_toe;
internal class Game
{
    private int Gamemode { get; }
    public List<Player> Players { get; }
    public int PlayerTurn { get; }
    private bool Timed { get; set; }
    private int[,] Board { get; }
    private int Winner { get; }

    public Game(List<Player> players,int gameMode )
    {
        this.Players = players;
        this.Board = new int[3,3];
        
    }

    public bool Won()
    {
        for(var r = 0; r < Board.Rank; r++)
        {
            var initialPiece = Board[r, 0];
            bool foundWinner = true;

            for (var c = 1; c < Board.GetLength(r); c++) // Checks horizontal wins
            {
                if (!initialPiece.Equals(Board[r, c]))
                {
                    foundWinner = false;
                    break;
                }
            }
            if(foundWinner)
            {
                return true;
            }
        }
        for(var c = 0; c < Board.Rank; c++)
        {
            var initialPiece = Board[c, 0];
            bool foundWinner = true;

            for (var r = 1; r < Board.GetLength(c); r++) // Checks vertical wins
            {
                if (!initialPiece.Equals(Board[r, c]))
                {
                    foundWinner = false;
                    break;
                }
            }
            if(foundWinner)
            {
                return true;
            }
        }

        if (Board[0, 0].Equals(Board[1, 1]) && Board[0, 0].Equals(Board[2, 2])){
            return true;
        } else if (Board[0, 2].Equals(Board[1, 1]) && Board[0, 2].Equals(Board[3, 0])){
            return true;
        }
        
        return false;
    }

    public bool PlacePiece()
    {
        return false;
    }
}

public class Player
{
    public string Name { get; set; }
    public int Number { get; }
    public bool IsComputer { get; }
    private static int playerNumber = 0;

    public Player(string name,bool isComputer)
    {
        this.Name = name;
        this.Number = playerNumber;
        this.IsComputer = isComputer;
        playerNumber++;
    }

    public Player(bool isComputer)
    {
        this.Name = "Player"+playerNumber;
        this.Number = playerNumber;
        this.IsComputer = isComputer;
        playerNumber++;
    }

    public void Turn()
    {
    
    }

    public string getName()
    {
        return this.Name;
    }

    public bool PlacePiece()
    {
        return false;
    }
}
