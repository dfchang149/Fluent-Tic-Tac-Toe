using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
    DispatcherTimer matchTimer;
    bool canPress = true;

    public PlayingPage()
    {
        this.InitializeComponent();
        this.SetUpGame();
        this.CreateBoard();
        this.InitializeTimer();
        PageStackPanel.Children.Remove(AgainButton);
    }

    private void CreateBoard()
    {
        // Create grid
        for (var r = 0; r < Settings.boardSize.Y; r++)
        {
            for (var c = 0; c < Settings.boardSize.X; c++)
            {
                var index = (r * Settings.boardSize.X) + c;

                var button = new Button();
                button.Height = 80;
                button.Width = 80;
                button.Content = "";
                button.Name = "square" + index;
                button.FontSize = 24;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.CenterPoint = new Vector3((float)button.Height / 2);

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

                Grid.SetRow(button, r);
                Grid.SetColumn(button, c);
                Board.Children.Add(button);
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
        if (game.gamemode == 0 && game.GetCurrentPlayerTurn().Equals(game.players.First()))
        {
            TurnTextBlock.Text = "Your Turn";
        }
        else
        {
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
                button.Scale = new Vector3(0.9f);
            }
        }
    }

    private void OnGridLeft(object sender, RoutedEventArgs e)
    {
        if (game.winner == null)
        {
            Button button = sender as Button;
            button.Scale = new Vector3(1);
        }
    }

    private void OnGridPressed(object sender, RoutedEventArgs e)
    {
        if (!canPress)
        {
            return;
        }

        if (!game.started)
        {
            game.Start();
            SetMatchTimerActive(true);
        }

        if (game.winner == null)
        {
            if (game.GetCurrentPlayerTurn().isComputer)
            {
                // if it's a computer's turn
                return;
            }

            Button button = sender as Button;
            // Add piece to board
            var name = button.Name.ToString().ToLower();
            name = name.Substring("square".Length);
            var number = Int32.Parse(name);
            var row = number / game.board.GetLength(0);
            var col = number % game.board.GetLength(0);

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
        Button button = GetButtonFromPiece(recentPiece);
        TurnsTextBlock.Text = game.turns.ToString();
        button.IsEnabled = false;
        button.Content = recentPiece.player.symbol;

        // Check if won
        if (game.Won())
        {
            if (game.gamemode == 0 && game.winner != game.players.First())
            {
                TurnTextBlock.Text = "You lost!";
                AgainButtonText.Text = "Try again";
            }
            else
            {
                TurnTextBlock.Text = String.Concat(game.winner.name, " won!");

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
                    Button buttonPressed = GetButtonFromPiece(game.pieces.Last());
                    OnGridEntered(buttonPressed, null);
                    buttonPressed.BorderThickness = new Microsoft.UI.Xaml.Thickness(2,2,2,2);

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
                Button grid = (Button)Board.FindName("square" + piece.GetIndex());


                // Animate button size
                TimeSpan savedDuration = grid.ScaleTransition.Duration;
                grid.ScaleTransition.Duration = savedDuration.Multiply(3);
                grid.Scale = new Vector3(0.95f);

                // shrink first
                DispatcherTimer shrinkTimer = new DispatcherTimer();
                shrinkTimer.Interval = savedDuration + TimeSpan.FromMilliseconds(50 * num);
                EventHandler<Object> shrinkHandler = new EventHandler<object>((s1, e1) =>
                {
                    shrinkTimer.Stop();
                    grid.Style = Application.Current.Resources["AccentButtonStyle"] as Style;
                    grid.IsEnabled = true;
                    grid.Scale = new Vector3(1);
                    // grow back and reset ScaleTransition
                    DispatcherTimer growTimer = new DispatcherTimer();
                    growTimer.Interval = savedDuration.Divide(2) + TimeSpan.FromMilliseconds(100 * num);
                    EventHandler<Object> growHandler = new EventHandler<object>((s2, e2) =>
                    {
                        grid.ScaleTransition.Duration = savedDuration;
                        growTimer.Stop();

                        if (piece.Equals(game.winningPieces.Last()))
                        {
                            if (AgainButton.Parent != PageStackPanel)
                            {
                                PageStackPanel.Children.Add(AgainButton);
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
            if (AgainButton.Parent != PageStackPanel)
            {
                PageStackPanel.Children.Add(AgainButton);
            }
        }
    }

    private void PlayAgainButtonPressed(object sender, RoutedEventArgs e)
    {
        PageStackPanel.Children.Remove(AgainButton);
        SetMatchTimerActive(false);

        foreach (Piece piece in game.pieces)
        {
            var index = (piece.row * Settings.boardSize.X) + piece.col;
            Button grid = (Button)Board.FindName("square" + index);
            if (grid != null)
            {
                grid.Content = "";
                grid.IsEnabled = true;
                grid.Style = Application.Current.Resources["DefaultButtonStyle"] as Style;
            }

        }

        game.Restart();
        TimeTextBlock.Text = game.time.ToString();
        TurnsTextBlock.Text = game.turns.ToString();
        UpdateTurnText();
    }

    private Button GetButtonFromPiece(Piece piece)
    {
        var index = (piece.row * Settings.boardSize.X) + piece.col;
        return (Button) Board.FindName("square" + index);
    }
}
