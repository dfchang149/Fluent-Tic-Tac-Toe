// Reference: winui3gallery://item/SystemBackdrops
using System.Runtime.InteropServices;
using WinRT;
using Microsoft.UI.Xaml;
using Microsoft.UI.Composition.SystemBackdrops;
using Windows.UI.ViewManagement;
using Fluent_Tic_tac_toe;

namespace Fluent_Tic_tac_toe;

public class AcrylicWindow : Window
{
    WindowsSystemDispatcherQueueHelper? m_wsdqHelper; // See separate sample below for implementation
    DesktopAcrylicController? acrylicControler;
    SystemBackdropConfiguration? m_configurationSource;

    public AcrylicWindow()
    {
        TrySetAcrylicBackdrop();
    }
    bool TrySetAcrylicBackdrop()
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

            acrylicControler = new DesktopAcrylicController();

            // Enable the system backdrop.
            // Note: Be sure to have "using WinRT;" to support the Window.As<...>() call.
            acrylicControler.AddSystemBackdropTarget(this.As<Microsoft.UI.Composition.ICompositionSupportsSystemBackdrop>());
            acrylicControler.SetSystemBackdropConfiguration(m_configurationSource);
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
        if (acrylicControler != null)
        {
            acrylicControler.Dispose();
            acrylicControler = null;
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