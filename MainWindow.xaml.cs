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
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUIEx;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Fluent_Tic_tac_toe;
/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : WinUIEx.WindowEx
{
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        //ExtendsContentIntoTitleBar = true;
        //SetTitleBar(AppTitleBar);
    }

    private void PlaySingleplayerClick(object sender, RoutedEventArgs e)
    {
        
        ContentFrame.Navigate(typeof(Pages.PlayingPage));
        PlayButton.Visibility = Visibility.Collapsed;
        SettingsButton.Visibility = Visibility.Collapsed;
    }

    private void PlayMultiplayerClick(object sender, RoutedEventArgs e)
    {
        ContentFrame.Navigate(typeof(Pages.PlayingPage));
    }

    private void SettingsButtonClick(object sender, RoutedEventArgs e)
    {
        
    }

    private void WindowActivated(object sender, WindowActivatedEventArgs e)
    {
        if (e.WindowActivationState == WindowActivationState.Deactivated)
        {
            TitleBarFill.Visibility = Visibility.Collapsed;
        }
        else
        {
            TitleBarFill.Visibility = Visibility.Visible;
        }
    }
}

