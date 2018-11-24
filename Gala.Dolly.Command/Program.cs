using System.Drawing;
using System;

// https://appuals.com/how-to-setup-rdp-on-windows-10-all-versions/

namespace Gala.Dolly.Test
{
    using Galatea.Diagnostics;
    using Galatea.Imaging.IO;
    using Galatea.IO;
    using Galatea.Runtime.Services;

    internal static class Program
    {
        private static TestEngine engine;
        //private static bool started;

        public static TestEngine TestEngine { get { return engine; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Load Local Settings
            if (System.IO.File.Exists("Gala.Dolly.Command.config"))
                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", "Gala.Dolly.Command.config");

            // Start Galatea Robotics Engine
            Program.Startup();

            // Do Test Code
            Console.Write("Press any key to continue...");
            Console.ReadKey();
            //TestModule.TestMethod();

            // Shutdown Galatea Robotics Engine
            Program.Shutdown();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Galatea.TeaException)
            {
                engine.Debugger.HandleTeaException(e.ExceptionObject as Galatea.TeaException, null);
            }
            else
            {
                engine.Debugger.ThrowSystemException((Exception)e.ExceptionObject, null);
            }
        }
        internal static void Startup()
        {
            /*
            if(Properties.Settings.Default.ImagingSettings == null)
            {
                Properties.Settings.Default.ImagingSettings = Properties.ImagingSettings.Create();
                Properties.Settings.Default.Save();
            }
             */

            // Suppress Timeout
            Properties.Settings.Default.ImagingSettings.SuppressTimeout = true;

            // Initialize Runtime
            DebuggerLogLevelSettings.Initialize(Properties.Settings.Default.DebuggerLogLevel, Properties.Settings.Default.DebuggerAlertLevel);
            IDebugger debugger = new Gala.Dolly.Test.TestDebugger();

            Gala.Data.Databases.SerializedDataAccessManager serializedDataAccessManager =
                new Gala.Data.Databases.SerializedDataAccessManager(Properties.Settings.Default.DataAccessManagerConnectionString);
            serializedDataAccessManager.RestoreBackup(@"..\..\..\Data\SerializedData.1346.dat");

            engine = new TestEngine(debugger, serializedDataAccessManager);
            engine.User = new Galatea.Runtime.Services.User("Test");

            engine.Startup();

            //// Silence Speech
            //engine.AI.LanguageModel.SpeechModule.StaySilent = true;
        }
        internal static void Shutdown()
        {
            // Shutdown AI Engine
            engine.Shutdown();

            // Save Runtime Settings
            Properties.Settings.Default.DebuggerLogLevel = DebuggerLogLevelSettings.DebuggerLogLevel;
            Properties.Settings.Default.DebuggerAlertLevel = DebuggerLogLevelSettings.DebuggerAlertLevel;
            Properties.Settings.Default.Save();

            //started = false;
        }


        private static void DoGifThings()
        {

        }
    }
}