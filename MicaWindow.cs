﻿// Reference: winui3gallery://item/SystemBackdrops
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.UI.ViewManagement;
using Windows.Storage;

namespace Fluent_Tic_tac_toe;

class MicaWindow : Window
{
    WindowsSystemDispatcherQueueHelper? m_wsdqHelper; // See separate sample below for implementation
    MicaController? m_micaController;
    SystemBackdropConfiguration? m_configurationSource;

    public MicaWindow()
    {
        TrySetMicaBackdrop();
    }
    bool TrySetMicaBackdrop()
    {
        if (MicaController.IsSupported())
        {
            m_wsdqHelper = new WindowsSystemDispatcherQueueHelper();
            m_wsdqHelper.EnsureWindowsSystemDispatcherQueueController();

            // Hooking up the policy object
            m_configurationSource = new SystemBackdropConfiguration();
            Activated += Window_Activated;
            Closed += Window_Closed;

            // Initial configuration state.
            m_configurationSource.IsInputActive = true;

            m_micaController = new MicaController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            m_micaController.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            m_micaController.SetSystemBackdropConfiguration(m_configurationSource);
            return true; // succeeded
        }

        return false; // Mica is not supported on this system
    }

    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (m_configurationSource == null) return;
        bool IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
        if (IsInputActive)
            m_configurationSource.IsInputActive = true;
        else if (!Settings.IsMicaInfinite)
            m_configurationSource.IsInputActive = false;
    }

    private void Window_Closed(object sender, WindowEventArgs args)
    {
        // Make sure any Mica/Acrylic controller is disposed so it doesn't try to
        // use this closed window.
        if (m_micaController != null)
        {
            m_micaController.Dispose();
            m_micaController = null;
        }
        this.Activated -= Window_Activated;
        m_configurationSource = null;
    }

    class WindowsSystemDispatcherQueueHelper
    {
        [StructLayout(LayoutKind.Sequential)]
        struct DispatcherQueueOptions
        {
            internal int dwSize;
            internal int threadType;
            internal int apartmentType;
        }

        [DllImport("CoreMessaging.dll")]
        private static extern int CreateDispatcherQueueController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController);

        object? m_dispatcherQueueController = null;
        public void EnsureWindowsSystemDispatcherQueueController()
        {
            if (Windows.System.DispatcherQueue.GetForCurrentThread() != null)
                // one already exists, so we'll just use it.
                return;

            if (m_dispatcherQueueController == null)
            {
                DispatcherQueueOptions options;
                options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
                options.threadType = 2;    // DQTYPE_THREAD_CURRENT
                options.apartmentType = 2; // DQTAT_COM_STA

                _ = CreateDispatcherQueueController(options, ref m_dispatcherQueueController);
            }
        }
    }
}

public static class Settings
{
    static ApplicationDataContainer ApplicationSetting = ApplicationData.Current.LocalSettings;
    public static bool IsMicaInfinite
    {
        get => (bool)(ApplicationSetting.Values[nameof(IsMicaInfinite)] ?? false);
        set => ApplicationSetting.Values[nameof(IsMicaInfinite)] = value;
    }
    public static bool IsSoundEnabled
    {
        get => (bool)(ApplicationSetting.Values[nameof(IsSoundEnabled)] ?? false);
        set => ApplicationSetting.Values[nameof(IsSoundEnabled)] = value;
    }

}