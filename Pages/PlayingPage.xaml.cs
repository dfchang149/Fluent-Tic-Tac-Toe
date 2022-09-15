using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ABI.Windows.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Windows.UI.ViewManagement;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class PlayingPage : Page
{
    Game game;
    DispatcherTimer matchTimer;
    bool canPress = true;

    public PlayingPage()
    {
        this.InitializeComponent();
        this.SetUpGame();
        this.InitializeTimer();
        PageGrid.Children.Remove(AgainButton);
    }

    private void BoardGridSizeChanged(object sender, RoutedEventArgs e)
    {
        var aspectRatio = Settings.boardSize.X / Settings.boardSize.Y;

        if (aspectRatio > 1)
        {
            var minLength = Math.Min(BoardGrid.ActualHeight * aspectRatio, BoardGrid.ActualWidth);
            Board.Height = minLength / aspectRatio;
            Board.Width = minLength;
        } else if (aspectRatio < 1)
        {
            var minLength = Math.Min(BoardGrid.ActualHeight, BoardGrid.ActualWidth);
            Board.Height = minLength;
            Board.Width = minLength * aspectRatio;
        } else
        {
            var minLength = Math.Min(BoardGrid.ActualHeight, BoardGrid.ActualWidth);
            Board.Height = minLength;
            Board.Width = minLength;
        }
        if (game != null)
        {
            var spacing = 8 - (Math.Max(Settings.boardSize.X, Settings.boardSize.Y) / 2);
            int buttonLength = (int)((Board.Height / Settings.boardSize.Y) - (spacing));
            for (var r = 0; r < Settings.boardSize.Y; r++)
            {
                for (var c = 0; c < Settings.boardSize.X; c++)
                {
                    var index = (r * Settings.boardSize.X) + c;
                    Button square = (Button)Board.FindName("square" + index);

                    if (square != null)
                    {
                        square.Height = buttonLength;
                        square.Width = buttonLength;
                        square.CenterPoint = new Vector3((float)(buttonLength / 2));
                        square.CornerRadius = new CornerRadius(buttonLength / 16);

                        Piece piece = game.board[r, c];

                        if (piece != null)
                        {
                            SetSquareText(square,piece.player.symbol);
                        }
                    }
                }
            }
        }
    }

    private void BoardLoaded(object sender, RoutedEventArgs e)
    {
        BoardGridSizeChanged(null,null);

        // Update Board
        var spacing = 8 - (Math.Max(Settings.boardSize.X, Settings.boardSize.Y) / 2);
        Board.RowSpacing = spacing;
        Board.ColumnSpacing = spacing;

        for (var r = 0; r < Settings.boardSize.Y; r++)
        {
            RowDefinition rowDef = new RowDefinition();
            rowDef.Height = new GridLength(1, GridUnitType.Star);
            Board.RowDefinitions.Add(rowDef);
        }

        for (var c = 0; c < Settings.boardSize.X; c++)
        {
            ColumnDefinition columnDef = new ColumnDefinition();
            columnDef.Width = new GridLength(1, GridUnitType.Star);
            Board.ColumnDefinitions.Add(columnDef);
        }

        // Add buttons
        for (var r = 0; r < Settings.boardSize.Y; r++)
        {
            for (var c = 0; c < Settings.boardSize.X; c++)
            {
                var index = (r * Settings.boardSize.X) + c;
                int buttonLength = (int)((Board.ActualHeight / Settings.boardSize.Y) - (spacing));

                Vector3Transition vector3Transition = new Vector3Transition();
                vector3Transition.Duration = System.TimeSpan.FromMilliseconds(100);

                var square = new Button()
                {
                    Height = buttonLength,
                    Width = buttonLength,
                    Content = new TextBlock().Text = "",
                    Name = "square" + index,
                    FontSize = 24,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    CenterPoint = new Vector3((float)(buttonLength / 2)),
                    Padding = new Thickness(0),
                    CornerRadius = new CornerRadius(buttonLength / 16),

                // Add transitions
                ScaleTransition = vector3Transition,
                    TranslationTransition = vector3Transition
                };

                // Handle Events
                square.Click += OnSquarePressed;
                square.PointerEntered += OnSquareEntered;
                square.PointerExited += OnSquareLeft;
                square.PointerCanceled += OnSquareLeft;

                Grid.SetRow(square, r);
                Grid.SetColumn(square, c);
                Board.Children.Add(square);
            }
        }
    }

    private void SetUpGame()
    {
        game = new Game();

        if (game.gamemode == 0) // singleplayer
        {
            PlayersIcon.Symbol = Symbol.Contact;
        }
        else if (game.gamemode == 1) // multiplayer
        {
            PlayersIcon.Symbol = Symbol.People;
        }
        else // spectator
        {
            InfosPanel.Children.Remove(PlayersInfo);
            PlayersIcon.Symbol = Symbol.People;
        }

        // Update Textblocks
        UpdateTurnText();

        PlayersTextBlock.Text = game.GetNumberOfRealPlayers().ToString();
        TimeTextBlock.Text = game.time.ToString();
        TurnsTextBlock.Text = game.time.ToString();

        var botPlayers = (game.players.Count - game.GetNumberOfRealPlayers());

        if (botPlayers > 0)
        {
            BotsTextBlock.Text = botPlayers.ToString();
        }
        else
        {
            InfosPanel.Children.Remove(BotsInfo);
        }


        if (!Settings.matchTimerEnabled && !Settings.boardInfoEnabled && !Settings.playerCounterEnabled)
        {
            InfosPanel.Visibility = Visibility.Collapsed;
        }
        else
        {
            if (!Settings.matchTimerEnabled)
            {
                InfosPanel.Children.Remove(TimerInfo);
            }

            if (!Settings.boardInfoEnabled)
            {
                InfosPanel.Children.Remove(BoardsInfo);
            }

            if (!Settings.playerCounterEnabled)
            {
                InfosPanel.Children.Remove(PlayersInfo);
                InfosPanel.Children.Remove(BotsInfo);
            }
        }
    }

    private void InitializeTimer()
    {
        if (Settings.matchTimerEnabled)
        {
            matchTimer = new DispatcherTimer();
            matchTimer.Interval = TimeSpan.FromSeconds(1);
            EventHandler<Object> handler = new EventHandler<object>((s, e) =>
            {
                if (game.winner == null)
                {
                    game.time++;
                    TimeTextBlock.Text = game.time.ToString();
                }
                else
                {
                    matchTimer.Stop();
                }
            });
            matchTimer.Tick += handler;
        }
    }

    private void SetMatchTimerActive(bool value)
    {
        if(matchTimer != null)
        {
            if (value)
            {
                matchTimer.Start();
            }
            else
            {
                matchTimer.Stop();
            }
        }
    }

    private void UpdateTurnText()
    {
        if (game.GetCurrentPlayerTurn().isComputer && !game.started)
        {
            TurnTextBlock.Text = "Click a square to start the game.";
        }
        else if (game.gamemode == 0 && !game.GetCurrentPlayerTurn().isComputer)
        {
            TurnTextBlock.Text = "Your Turn";
        }
        else
        {
            TurnTextBlock.Text = game.GetCurrentPlayerTurn().name + "'s Turn";
        }
    }

    private void OnSquareEntered(object sender, RoutedEventArgs e)
    {
        if (game.winner == null && !game.GetCurrentPlayerTurn().isComputer)
        {
            ShrinkSquare(sender as Button);
        }
    }

    private void ShrinkSquare(Button square)
    {
        if (square.IsEnabled)
        {
            square.Scale = new Vector3(0.9f);
        }
    }

    private void OnSquareLeft(object sender, RoutedEventArgs e)
    {
        if (game.winner == null)
        {
            Button button = sender as Button;
            button.Scale = new Vector3(1);
        }
    }

    private void OnSquarePressed(object sender, RoutedEventArgs e)
    {
        if (!canPress)
        {
            return;
        }

        if (!game.started)
        {
            game.Start();
            SetMatchTimerActive(true);
            if (game.GetCurrentPlayerTurn().isComputer) // if it's a computer's turn
            {
                OnBoardUpdated();
                return;
            }
        }

        if (game.winner == null)
        {
            if (game.GetCurrentPlayerTurn().isComputer) // if it's a computer's turn
            {
                return;
            }

            Button square = sender as Button;
            // Add piece to board
            var name = square.Name.ToString().ToLower();
            name = name.Substring("square".Length);
            var number = Int32.Parse(name);
            var row = number / Settings.boardSize.X;
            var col = number % Settings.boardSize.X;

            var wasPlaced = game.PlacePiece((int)row, (int)col);

            if (wasPlaced)
            {
                OnBoardUpdated();
            }
            else
            {
                square.Opacity = 1;
                square.IsEnabled = true;
            }
        }
        //TimeTextBlock.Text = "Row: " + row + ", Col: " + col;
    }

    private void OnBoardUpdated()
    {
        if (game.pieces.Count > 0)
        {
            Piece recentPiece = game.pieces.Last();
            Button square = GetSquareFromPiece(recentPiece);
            TurnsTextBlock.Text = game.turns.ToString();
            square.IsEnabled = false;
            SetSquareText(square, recentPiece.player.symbol);
        }

        // Check if won
        if (game.Won())
        {
            if (game.gamemode == 0 && game.winner.isComputer) 
            {
                TurnTextBlock.Text = "You lost!";
                AgainButtonText.Text = "Try again";
            }
            else
            {
                if (game.gamemode == 0 && !game.winner.isComputer)
                {
                    TurnTextBlock.Text = "You won!";
                } else {
                    TurnTextBlock.Text = String.Concat(game.winner.name, " won!");
                }

                if (game.gamemode == 2)
                {
                    AgainButtonText.Text = "New round";
                } else
                {
                    AgainButtonText.Text = "Play again";
                }
            }
            OnGameEnded();
        }
        else if (game.IsDraw())
        {
            TurnTextBlock.Text = "Draw!";
            OnGameEnded();
        }
        else
        {
            UpdateTurnText();
            if (game.GetCurrentPlayerTurn().isComputer)
            {
                if(game.ComputerTurn()) // Computer's Turn
                {
                    // Simulate button pressing
                    canPress = false;
                    Button buttonPressed = GetSquareFromPiece(game.pieces.Last());
                    ShrinkSquare(buttonPressed);
                    buttonPressed.BorderThickness = new Microsoft.UI.Xaml.Thickness(1.5,1.5,1.5,1.5);

                    DispatcherTimer timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(0.25);
                    EventHandler<Object> timerHandler = new EventHandler<object>((s2, e2) =>
                    {
                        timer.Stop();
                        buttonPressed.BorderThickness = new Microsoft.UI.Xaml.Thickness(1,1,1,1);
                        buttonPressed.Scale = new Vector3(1);
                        OnBoardUpdated();
                        canPress = true;
                    });
                    timer.Tick += timerHandler;
                    timer.Start();
                }
            }
        }
    }

    private void OnGameEnded()
    {
        SetMatchTimerActive(false);
        if (game.winner != null)
        {
            var num = 0;
            foreach (Piece piece in game.winningPieces)
            {
                num++;
                Button square = (Button)Board.FindName("square" + piece.GetIndex());


                // Animate button size
                TimeSpan savedDuration = square.ScaleTransition.Duration;
                square.ScaleTransition.Duration = savedDuration.Multiply(3);
                square.Scale = new Vector3(0.95f);

                // shrink first
                DispatcherTimer shrinkTimer = new DispatcherTimer();
                shrinkTimer.Interval = savedDuration + TimeSpan.FromMilliseconds(50 * num);
                EventHandler<Object> shrinkHandler = new EventHandler<object>((s1, e1) =>
                {
                    shrinkTimer.Stop();
                    square.Style = Application.Current.Resources["AccentButtonStyle"] as Style;
                    square.IsEnabled = true;
                    square.Scale = new Vector3(1);
                    // grow back and reset ScaleTransition
                    DispatcherTimer growTimer = new DispatcherTimer();
                    growTimer.Interval = savedDuration.Divide(2) + TimeSpan.FromMilliseconds(100 * num);
                    EventHandler<Object> growHandler = new EventHandler<object>((s2, e2) =>
                    {
                        square.ScaleTransition.Duration = savedDuration;
                        growTimer.Stop();

                        if (piece.Equals(game.winningPieces.Last()))
                        {
                            if (AgainButton.Parent != PageGrid)
                            {
                                PageGrid.Children.Add(AgainButton);
                            }
                        }
                    });
                    growTimer.Tick += growHandler;
                    growTimer.Start();
                });
                shrinkTimer.Tick += shrinkHandler;
                shrinkTimer.Start();

                // Hopefully animate background color
                /*var uiSettings = new UISettings();
                var accentColor = new UISettings().GetColorValue(UIColorType.Accent);
                Brush brush = new SolidColorBrush(accentColor);
                button.Background = brush;

                
                ColorAnimation colorAnim = new ColorAnimation()
                {
                    To = accentColor,
                    Duration = TimeSpan.FromSeconds(1),
                    AutoReverse = true,
                };

                DoubleAnimation widthAnimation = new DoubleAnimation();
                120, 300, TimeSpan.FromSeconds(5));
                widthAnimation.RepeatBehavior = RepeatBehavior.Forever;
                widthAnimation.AutoReverse = true;
                grid.BeginAnimation(Button.WidthProperty, widthAnimation);
                OnGridLeft(grid, null);
                */
            }
        }
        else
        {
            if (AgainButton.Parent != PageGrid)
            {
                PageGrid.Children.Add(AgainButton);
                Grid.SetRow(AgainButton, 2);
            }
        }
    }

    private void PlayAgainButtonPressed(object sender, RoutedEventArgs e)
    {
        PageGrid.Children.Remove(AgainButton);
        SetMatchTimerActive(false);

        foreach (Piece piece in game.pieces)
        {
            var index = (piece.row * Settings.boardSize.X) + piece.col;
            Button square = (Button)Board.FindName("square" + index);
            if (square != null)
            {
                SetSquareText(square, "");
                square.IsEnabled = true;
                square.Style = Application.Current.Resources["DefaultButtonStyle"] as Style;
            }

        }

        game.Restart();
        TimeTextBlock.Text = game.time.ToString();
        TurnsTextBlock.Text = game.turns.ToString();
        UpdateTurnText();
    }


    private void SetSquareText(Button square,string text)
    {
        TextBlock textblock = new TextBlock();
        textblock.Text = text;
        textblock.HorizontalAlignment = HorizontalAlignment.Center;
        textblock.VerticalAlignment = VerticalAlignment.Center;
        textblock.TextAlignment = TextAlignment.Center;
        textblock.FontSize = Math.Min(square.Height / 2.5,28);
        square.Content = textblock;
    }

    private Button GetSquareFromPiece(Piece piece)
    {
        var index = (piece.row * Settings.boardSize.X) + piece.col;
        return (Button) Board.FindName("square" + index);
    }
}
