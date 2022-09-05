using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Resources;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
        Button button = (Button)sender; 
        Brush brush = new SolidColorBrush(Colors.AliceBlue);
        //button.Background = brush;
        //Style style = new Style();
        //button.Style =  "{StaticResource AccentButtonStyle}";
        button.Content = button.Content.Equals("x") ? "o":"x";
    }

    public void PlacePiece(string piece, int row, int col)
    {
        if (Board.Children.Contains(Board))
        {
            
        }
    }
}
