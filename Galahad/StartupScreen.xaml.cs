using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Galahad
{
    using Galahad.Robotics;
    using Galatea.Diagnostics;
    using Galatea.Runtime.Net;
    using System.Threading.Tasks;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartupScreen : Page, IEngineInitializer, IDisposable
    {
        private SplashScreen splash;    // Variable to hold the splash screen object.

        internal Rect splashImageRect;  // Rect to store splash screen image coordinates.
        internal bool dismissed;        // Variable to track splash screen dismissal status.
        internal Frame rootFrame;

        private Galahad.Net.HttpService webService;

        // Define methods and constructor
#pragma warning disable CA1801 // Review unused parameters
        public StartupScreen(SplashScreen splashScreen, bool loadState)
        {
            InitializeComponent();

            // Listen for window resize events to reposition the extended splash screen image accordingly.
            // This ensures that the extended splash screen formats properly in response to window resizing.
            Window.Current.SizeChanged += OnSizeChanged;

            splash = splashScreen;
            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += OnSplashDismissed;
                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;

#if ARM
                splashImageRect = new Rect(
                    (splashImageRect.Width - imageContainer.Width) / 2,
                    (splashImageRect.Height - imageContainer.Height) / 2,
                    imageContainer.Width, imageContainer.Height);
#endif

                PositionWidgets();
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();
        }
#pragma warning restore CA1801 // Review unused parameters

        #region UI

        async void PositionWidgets()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                errorHandlerContainer.Visibility = Visibility.Collapsed;
                errorHandlerContainer.Height = 0;

                PositionImage();

                // If applicable, include a method for positioning a progress control.
                PositionRing();

                // Progress Bar
                PositionStatus();
            });
        }

        void PositionImage()
        {
            imageContainer.SetValue(Canvas.LeftProperty, splashImageRect.X);
            imageContainer.SetValue(Canvas.TopProperty, splashImageRect.Y);
            imageContainer.Height = splashImageRect.Height;
            imageContainer.Width = splashImageRect.Width;
        }
        void PositionRing()
        {
            progressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (progressRing.Width * 0.5));

            double progressRingY = (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1);
            progressRingY -= (progressRing.Height * 3.25D);
            progressRing.SetValue(Canvas.TopProperty, progressRingY);
        }
        void PositionStatus()
        {
            progressContainer.SetValue(Canvas.LeftProperty, splashImageRect.X);
            double progressContainerY = (double)progressRing.GetValue(Canvas.TopProperty) + progressRing.Height;
            //System.Diagnostics.Debug.WriteLine($"Status Bar Y: {progressContainerY}");
            //progressContainerY = 50;
            progressContainer.SetValue(Canvas.TopProperty, progressContainerY);
        }

        void OnSizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            PositionWidgets();
        }

        // Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
        void OnSplashDismissed(SplashScreen sender, object args)
        {
            RuntimeStartup();
        }
        #endregion

        #region Runtime Engine Initializer

        void RuntimeStartup()
        {
            dismissed = true;
            _starting = true;

            Task.Run(async () =>
            {
                try
                {
                    Galatea.Runtime.IRuntimeEngine runtimeEngine = await App.CreateEngine(this).ConfigureAwait(false);

                    // Start Engine
                    runtimeEngine.Startup();

                    // TODO:  Write output to (fake) console
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex).ConfigureAwait(false);
                    // TODO: Exit thread on error
                }
            });
        }

        async void IEngineInitializer.OnEngineInitializationStatusUpdated(object sender, Robotics.EngineInitializationEventArgs e)
        {
            // https://stackoverflow.com/questions/19341591/the-application-called-an-interface-that-was-marshalled-for-a-different-thread

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                progressBar.Value = e.Progress;
                status.Text = e.Message;
                App.Engine.Debugger.Log(DebuggerLogLevel.Diagnostic, e.Message);
            });
        }

        void IEngineInitializer.OnEngineStartupComplete(object sender, EventArgs e)
        {
            System.Threading.Tasks.Task.Run(async () =>
            {
                try
                {
                    // Start Web
                    webService = new Galahad.Net.HttpService(App.UWPSettings.HttpServiceHostPort);
                    webService.Initialize(App.Engine);

                    App.Engine.HttpService.StartListener();
                }
                catch (Exception ex)
                {
                    await HandleExceptionAsync(ex).ConfigureAwait(false);
                }
            });

            _starting = false;

            // Load Main Screen
            LoadMainScreen();
        }

        #endregion

        async void LoadMainScreen()
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    MainPage newMainPage = new MainPage();

                    rootFrame = new Frame
                    {
                        Content = newMainPage
                    };

                    Window.Current.Content = rootFrame;

                    webService.NetCommands = newMainPage;
                }
                catch (Exception ex)
                {
                    HandleException(ex);
                }
            });
        }

        async Task HandleExceptionAsync(Exception ex)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleException(ex));
        }

        void HandleException(Exception ex)
        {
            string msg = "SOMETHING HAPPEN!!!"; // TODO

            App.HandleException(ref msg, ex);
            status.Text = "SOMETHING HAPPEN!!!  " + ex.Message;

            // Log stack trace

            // Show Retry and Quit buttons
            progressRing.SetValue(Canvas.TopProperty, (double)progressRing.GetValue(Canvas.TopProperty) - 32);
            progressContainer.SetValue(Canvas.TopProperty, (double)progressContainer.GetValue(Canvas.TopProperty) - 32);

            errorHandlerContainer.SetValue(Canvas.TopProperty, (double)progressContainer.GetValue(Canvas.TopProperty) + 62);
            errorHandlerContainer.Height = 24;
            errorHandlerContainer.Visibility = Visibility.Visible;
        }

#pragma warning disable CA1801 // Review unused parameters
        private void RetryButton_Click(object sender, RoutedEventArgs e)
        {
            PositionWidgets();

            App.ShutdownEngine();
            RuntimeStartup();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.ShutdownEngine();
            Application.Current.Exit();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_starting)
            {
                App.ShutdownEngine();
                Application.Current.Exit();     // User closed the form before startup was complete.
            }
        }
#pragma warning restore CA1801 // Review unused parameters

        void RestoreStateAsync(bool loadState)
        {
            if (loadState)
            {
                // code to load your app's state here
            }
        }

        internal static bool Starting { get { return _starting; } }

        private static bool _starting;

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state (managed objects).
                    webService.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StartupScreen() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
