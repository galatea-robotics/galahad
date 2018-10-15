using System.Drawing;
using System;

// https://appuals.com/how-to-setup-rdp-on-windows-10-all-versions/

namespace Gala.Dolly.Test
{
    using Galatea.Imaging.IO;
    using Galatea.IO;
    using Galatea.Runtime.Services;

    internal static class Program
    {
        private static TestEngine engine;
        private static bool started;

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
            TestModule.TestMethod();

            // Shutdown Galatea Robotics Engine
            Program.Shutdown();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Galatea.TeaException)
            {
                engine.Debugger.HandleTeaException(e.ExceptionObject as Galatea.TeaException);
            }
            else
            {
                engine.Debugger.ThrowSystemException((Exception)e.ExceptionObject);
            }
        }
        internal static void Startup()
        {
            // Initialize Runtime
            Galatea.Diagnostics.IDebugger debugger = new Gala.Dolly.Test.TestDebugger();

            Gala.Data.Databases.SerializedDataAccessManager serializedDataAccessManager =
                new Gala.Data.Databases.SerializedDataAccessManager(Properties.Settings.Default.DataAccessManagerConnectionString);
            serializedDataAccessManager.RestoreBackup(@"..\..\..\Data\SerializedData.1346.dat");

            engine = new TestEngine(debugger, serializedDataAccessManager);
            engine.User = new Galatea.Runtime.Services.User("Test");

            engine.Startup();

            // Suppress Timeout
            Galatea.AI.Imaging.Properties.Settings.Default.SuppressTimeout = true;

            //// Silence Speech
            //engine.AI.LanguageModel.SpeechModule.StaySilent = true;
        }
        internal static void Shutdown()
        {
            // Shutdown AI Engine
            engine.Shutdown();

            started = false;
        }


        private static void DoGifThings()
        {

        }
    }
}