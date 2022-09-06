using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Resources;
using Microsoft.UI.Xaml.XamlTypeInfo;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlayingPage : Page
{
    Game game;

    public PlayingPage()
    {
        this.InitializeComponent();
        this.CreateBoard();
    }

    private void CreateBoard()
    {
        // Initiate game
        Player player1 = new Player(false);
        Player player2 = new Player(false);
        List<Player> playerList = new List<Player>();
        playerList.Add(player1);
        playerList.Add(player2);
        game = new Game(playerList,0);

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
                button.Click += OnGridPressed;

                Grid.SetRow(button, r % 3);
                Grid.SetColumn(button, c % 3);
                Board.Children.Add(button);
            }
        }
    }

    private async void OnGridPressed(object sender, RoutedEventArgs e)
    {
        //var uiSettings = new UISettings();
        //var accentColor = uiSettings.GetColorValue(UIColorType.Accent);
        Button button = sender as Button; 
        //Brush brush = new SolidColorBrush(accentColor);
        //button.Background = brush;
        button.IsEnabled = false;
        button.Content = button.Content.Equals("x") ? "o":"x";

        // Add piece to board
        var name = button.Name.ToString().ToLower();
        name = name.Substring("square".Length);
        var number = Int32.Parse(name);
        var row = number / game.board.GetLength(0);
        var col = number % game.board.GetLength(0);

        game.PlacePiece(game.players[0], row,col);

        TimeTextBlock.Text = "Row: " + row + ", Col: " + col;

        // Check if won
        if(game.Won())
        {
            TurnTextBlock.Text = "WINNER";
            foreach (Piece piece in game.winningPieces)
            {
                Button grid = (Button)Board.FindName("square"+piece.GetIndex());
                grid.Style = Application.Current.Resources["AccentButtonStyle"] as Style;
            }
        }
    }

    public void PlacePiece(string piece, int row, int col)
    {
        if (Board.Children.Contains(Board))
        {
            
        }
    }


}
