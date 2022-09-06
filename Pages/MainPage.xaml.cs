using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe.Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
    }

    public void PlaySingleplayerClick(object sender, RoutedEventArgs e)
    {
        PageContent.Visibility = Visibility.Collapsed;
        ContentFrame.Navigate(typeof(Pages.PlayingPage), null, new DrillInNavigationTransitionInfo());
    }

    private void PlayMultiplayerClick(object sender, RoutedEventArgs e)
    {
        PageContent.Visibility = Visibility.Collapsed;
        ContentFrame.Navigate(typeof(Pages.PlayingPage), null, new DrillInNavigationTransitionInfo());
    }

    private void SettingsButtonClick(object sender, RoutedEventArgs e)
    {

    }
}
