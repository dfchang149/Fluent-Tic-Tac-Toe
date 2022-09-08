﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Numerics;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Resources;
using Microsoft.UI.Xaml.XamlTypeInfo;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using System.Diagnostics;
using System.Timers;
using Windows.Media;
using Microsoft.UI.Xaml.Media.Animation;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Documents;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlayingPage : Page
{
    Game game;
    DispatcherTimer timer;

    public PlayingPage()
    {
        this.InitializeComponent();
        this.CreateBoard();
        this.SetUpGame();
        this.InitializeTimer();
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        if (this.Frame.CanGoBack)
        {
            this.Frame.GoBack();
        }
    }

    private void CreateBoard()
    {
        // Create grid
        for (var r = 0; r < 3; r++)
        {
            for(var c = 0; c < 3; c++)
            {
                var index = (r * 3) + c;

                var button = new Button();
                button.Height = 80;
                button.Width = 80;
                button.Content = "";
                button.Name = "square" + index;
                button.FontSize = 24;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.CenterPoint = new Vector3((float)button.Height/2);

                // Add transitions
                Vector3Transition vector3Transition = new Vector3Transition();
                vector3Transition.Duration = System.TimeSpan.FromMilliseconds(100);
                button.ScaleTransition = vector3Transition;
                button.TranslationTransition = vector3Transition;

                // Handle Events
                button.Click += OnGridPressed;
                button.PointerEntered += OnGridEntered;
                button.PointerExited += OnGridLeft;
                button.PointerCanceled += OnGridLeft;

                Grid.SetRow(button, r % 3);
                Grid.SetColumn(button, c % 3);
                Board.Children.Add(button);
            }
        }
    }

    private void SetUpGame()
    {
        // Add players to game
        List<Player> playerList = new List<Player>();
        Player player1 = new Player("Player 1");
        Player player2 = new Player("Player 2");

        if (Game.Gamemode.Equals(Game.gamemodes[0]))
        {
            player1.name = "You";
            player2 = new Player("Computer", true);
        }

        playerList.Add(player1);
        playerList.Add(player2);
        game = new Game(playerList);

        // Update Textblocks
        UpdateTurnText();
        PlayersTextBlock.Text = game.players.Count.ToString();
        TimeTextBlock.Text = game.time.ToString();
        TurnsTextBlock.Text = game.time.ToString();
    }

    private void InitializeTimer()
    {
        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        EventHandler<Object> handler = new EventHandler<object>((s, e) =>
        {
            if (game.winner == null)
            {
                game.time++;
                //TimeSpan time = TimeSpan.FromSeconds(game.time);
                //string displayTime = time.ToString(@"h\:m\:s");
                //while (displayTime.StartsWith("0:")){
                //    displayTime = displayTime.Substring(2);
                //}

                TimeTextBlock.Text = game.time.ToString();
            }
            else
            {
                timer.Stop();
            }
        });
        timer.Tick += handler;
    }

    private void UpdateTurnText()
    {
        if (Game.Gamemode.Equals(Game.gamemodes[0]) && game.GetCurrentPlayerTurn().Equals(game.players[0]))
        {
            TurnTextBlock.Text = "Your Turn";
        } else {
            TurnTextBlock.Text = game.GetCurrentPlayerTurn().name + "'s Turn";
        }
    }

    private void OnGridEntered(object sender, RoutedEventArgs e)
    {
        if (game.winner == null)
        {
            Button button = sender as Button;
            if (button.Content.Equals(""))
            {
                button.Scale = new Vector3((float)0.9);
            }
        }
        
        //button.Height = 70;
        //button.Width = 70;
        //button.Opacity = 0.5;
    }
    private void OnGridLeft(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        button.Scale = new Vector3(1);
        //button.Height = 80;
        //button.Width = 80;
        //button.Opacity = 1;
    }

    private void OnGridPressed(object sender, RoutedEventArgs e)
    {
        //var uiSettings = new UISettings();
        //var accentColor = uiSettings.GetColorValue(UIColorType.Accent);
        //Brush brush = new SolidColorBrush(accentColor);
        //button.Background = brush;

        if(!game.started)
        {
            game.Start();
            timer.Start();
        }

        if(game.winner == null)
        {
            if (Game.Gamemode.Equals(Game.gamemodes[0]) && !game.GetCurrentPlayerTurn().Equals(game.players.First()))
            {
                // if it's singleplayer and its not the client's turn
                return;
            }

            Button button = sender as Button;
            // Add piece to board
            var name = button.Name.ToString().ToLower();
            name = name.Substring("square".Length);
            var number = Int32.Parse(name);
            var row = number / game.board.GetLength(0);
            var col = number % game.board.GetLength(0);

            Player currPlayer = game.GetCurrentPlayerTurn();
            var wasPlaced = game.PlacePiece(row, col);

            if (wasPlaced)
            {
                OnBoardUpdated();
            }
            else
            {
                button.Opacity = 1;
                button.IsEnabled = true;
            }
        }
        //TimeTextBlock.Text = "Row: " + row + ", Col: " + col;
    }

    private void OnBoardUpdated()
    {
        Piece recentPiece = game.pieces.Last();
        var index = (recentPiece.row * 3) + recentPiece.col;
        Button button = (Button) Board.FindName("square" + index);

        TurnsTextBlock.Text = game.turns.ToString();
        button.IsEnabled = false;
        button.Content = recentPiece.player.symbol;

        // Check if won
        if (game.Won())
        {
            TurnTextBlock.Text = String.Concat(game.winner.name, " won!");
            OnGameEnded();
        }
        else if (game.IsDraw())
        {
            TurnTextBlock.Text = "DRAW";
            OnGameEnded();
        }
        else
        {
            UpdateTurnText();
            if (game.GetCurrentPlayerTurn().isComputer)
            {
                game.ComputerTurn();
                OnBoardUpdated();
            }
        }
    }

    private void OnGameEnded()
    {
        timer.Stop();
        AgainButton.Visibility = Visibility.Visible;
        if (game.winner != null)
        {
            foreach (Piece piece in game.winningPieces)
            {
                Button grid = (Button)Board.FindName("square" + piece.GetIndex());
                
                grid.IsEnabled = true;
                grid.Style = Application.Current.Resources["AccentButtonStyle"] as Style;
                OnGridLeft(grid, null);
            }
        }
    }

    private void CreatePlayAgainButton()
    {
        Button button = new Button();
        button.Name = "AgainButton";

    }

    private void PlayAgainButtonPressed(object sender, RoutedEventArgs e)
    {
        AgainButton.Visibility = Visibility.Collapsed;
        timer.Stop();
        game.Restart();
        TimeTextBlock.Text = game.time.ToString();
        TurnsTextBlock.Text = game.turns.ToString();
        UpdateTurnText();
        for (var i = 0; i < game.board.Length; i++) // Maybe only reset game.pieces
        {
            Button grid = (Button)Board.FindName("square" + i);
            //DoubleAnimation widthAnimation = new DoubleAnimation();
            //120, 300, TimeSpan.FromSeconds(5));
            //widthAnimation.RepeatBehavior = RepeatBehavior.Forever;
            //widthAnimation.AutoReverse = true;
            //grid.BeginAnimation(Button.WidthProperty, widthAnimation);
            if (grid != null)
            {
                grid.Content = "";
                grid.IsEnabled = true;
                grid.Style = Application.Current.Resources["DefaultButtonStyle"] as Style;
            }
        }
    }
}
