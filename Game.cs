﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

namespace Fluent_Tic_tac_toe;
internal class Game
{
    private int gamemode { get; }
    public List<Player> players { get; }
    public Player currentTurn { get; set; }
    public List<Piece> winningPieces { get; set; }
    public Piece[,] board { get; }
    public Player winner { get; set; }
    private bool timed { get; set; }

    public Game(List<Player> players,int gamemode )
    {
        this.players = players;
        this.board = new Piece[3,3];
        this.gamemode = gamemode;
        this.winningPieces = new List<Piece>();
    }

    public void Start()
    {
        this.players[0].symbol = "X";
        this.players[1].symbol = "O";
        this.currentTurn = this.players[0];
        this.winner = null;
    }

    public bool Won()
    {
        winningPieces.Clear();
        for (var r = 0; r < board.Rank+1; r++)
        {
            Piece initialPiece = board[r, 0];
            var foundWinner = true;
            if(initialPiece != null)
            {
                winningPieces.Add(initialPiece);

                for (var c = 1; c < board.GetLength(0); c++) // Checks horizontal wins
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
                    return true;
                }
            }
        }
        winningPieces.Clear();
        for (var c = 0; c < board.GetLength(0); c++)
        {
            Piece initialPiece = board[0, c];
            var foundWinner = true;
            if (initialPiece != null)
            {
                winningPieces.Add(initialPiece);

                for (var r = 1; r < board.Rank+1; r++) // Checks vertical wins
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
            for (var i = 0; i < board.Rank+1 && i < board.GetLength(0); i++)
            {
                Piece piece = board[i, i];
                if (piece == null) { foundWinner = false; break; }
                winningPieces.Add(piece);

                if (initialPiece == null)
                {
                    initialPiece = piece;
                } else {
                    if (!initialPiece.Matches(piece))
                    {
                        foundWinner = false;
                        break;
                    }
                }
            }
            if (foundWinner)
            {
                return true;
            }
            initialPiece = null;
            winningPieces.Clear();
            foundWinner = true;
            for (var i = 0; i < board.Rank+1 && i < board.GetLength(0); i++)
            {
                Piece piece = board[i, (board.GetLength(0)-1)-i];
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
                return true;
            }
        }
        
        return false;
    }

    public void TakeTurn()
    {
    
    }

    public bool PlacePiece(Player player,int row, int col)
    {
        if (board[row, col] == null)
        {
            Piece piece = new Piece(player, row, col);
            board[row, col] = piece;
            return true;
        }
        return false;
    }

    public string GetGridName(int row,int col)
    {
        return "square"+((row * 3) + col);
    }
}

public class Player
{
    public string name { get; set; }
    public int number { get; }
    public bool isComputer { get; }
    public string symbol { get; set;}
    private static int playerNumber = 0;

    public Player(string name,bool isComputer)
    {
        this.name = name;
        this.number = playerNumber;
        this.isComputer = isComputer;
        playerNumber++;
    }

    public Player(bool isComputer)
    {
        this.name = "Player"+playerNumber;
        this.number = playerNumber;
        this.isComputer = isComputer;
        playerNumber++;
    }

    public Player(string name)
    {
        this.name = name;
        this.number = playerNumber;
        this.isComputer = false;
        playerNumber++;
    }

    public void Turn()
    {
    
    }

    public string getName()
    {
        return this.name;
    }

    public bool PlacePiece()
    {
        return false;
    }
}

public class Piece
{
    public Player player { get; }
    public int row { get; }
    public int col { get; }

    public Piece(Player player,int row, int col)
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
        return (this.row * 3) + this.col;
    }
}