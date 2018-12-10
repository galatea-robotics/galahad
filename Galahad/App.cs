using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Galahad
{
    using Galatea;
    using Galatea.Runtime;
    using Galatea.Runtime.Services;
    using Properties;

    partial class App : IProvider
    {
        const string APPLICATION_TITLE = "Galahad";

        // Galahad
        static App _current;
        static Galahad.Robotics.Debugger _debugger;
        static Galahad.Robotics.Engine _engine = null;
        static UWPSettings _settings = null;

        internal static IRuntimeEngine Engine
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _engine; }
        }
        internal static UWPSettings UWPSettings
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _settings; }
        }

        public string ProviderID => "Galahad.exe";
        public string ProviderName => "Galahad Application";
        public ISite Site { get; set; }
        public void Dispose()
        {
            ShutdownEngine();
            Disposed?.Invoke(null, null);
        }

        public event EventHandler Disposed;

        internal static async Task<IRuntimeEngine> CreateEngine(IEngineInitializer initializer)
        {
            try
            {
                _settings = await UWPSettings.Load("Properties\\GalahadConfig.json");
                Galatea.Diagnostics.DebuggerLogLevelSettings.Initialize(_settings.DebuggerLogLevel, _settings.DebuggerAlertLevel);

                _debugger = new Galahad.Robotics.Debugger();
                _engine = new Galahad.Robotics.Engine(_settings, _debugger);
            }
            catch (Exception ex)
            {
                _debugger.ThrowSystemException(ex, _current);
                throw;
            }

            // Handle Initialization Status Updates
            _engine.InitializationStatusUpdated += initializer.OnEngineInitializationStatusUpdated;
            _engine.StartupComplete += initializer.OnEngineStartupComplete;

            // Finalize
            return _engine;
        }

        internal static void ShutdownEngine()
        {
            _engine?.Shutdown();
            _engine = null;
        }

        internal static void HandleException(Exception exception)
        {
            string msg = null;
            HandleException(ref msg, exception);
        }
        internal static void HandleException(ref string msg, Exception exception)
        {
            if (_engine != null)
            {
                Galatea.TeaException teaException = exception as Galatea.TeaException;
                if (teaException != null)
                {
                    _engine.Debugger.HandleTeaException(teaException, _current, false);
                    msg = _engine.Debugger.ErrorMessage;
                    msg = "##ERROR## > " + _engine.Debugger.ErrorMessage;   // TODO: use resource
                }
                else
                {
                    _engine.Debugger.ThrowSystemException(exception, _current);
                    msg = "##UNEXPECTED ERROR## > " + _engine.Debugger.ErrorMessage;    // TODO: use resource
                }

                _engine.Debugger.ClearError();
            }

            // Something happen!! 
            if (msg == null) msg = "##UNHANDLED ERROR## > " + exception.Message;    // TODO: use resource
        }

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            string msg = null;

            // Main Handler 
            HandleException(ref msg, e.Exception);

            e.Handled = true;
            if (StartupScreen.Starting) return;

            // Notify UI 
            try
            {
                await MainPage.Current.SendResponse(msg);
            }
            catch
            {
                this.Exit();
            }
        }
    }
}