using System;
using System.IO;

namespace Galahad.Robotics
{
    using Galatea;
    using Galatea.Diagnostics;
    using Galatea.Globalization;
    using Galatea.Runtime;

    class Debugger: Galatea.Runtime.Services.Debugger, Galatea.Diagnostics.IDebugger
    {
        public Debugger()
        {
            _fileLogger = new DebuggerLogger();
        }

        /// <summary>
        /// Gets or sets a <see cref="FileLogger"/> component reference.
        /// </summary>
        public virtual IFileLogger FileLogger
        {
            get { return _fileLogger; }
            set { _fileLogger = value; }
        }

        public override void HandleTeaException(TeaException ex, IProvider provider)
        {
            base.HandleTeaException(ex, provider);
        }

        /// <summary>
        /// Handles expected Galatea Core Exceptions, typically by logging them.
        /// </summary>
        /// <param name="ex">
        /// A run-time <see cref="TeaException"/>.
        /// </param>
        /// <param name="throwException">
        /// A value indicating if the Exception should remain unhandled after
        /// processing by the <see cref="Debugger"/> instance.
        /// </param>
        public override void HandleTeaException(TeaException ex, IProvider provider, bool throwException)
        {
            if (ex == null) return;

            string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            Log(DebuggerLogLevel.Error, msg);
            Log(DebuggerLogLevel.StackTrace, ex.StackTrace, true);

            string errorMessage = Galatea.Globalization.DiagnosticResources.Debugger_Error_Speech_Message;
            if (msg.Substring(0, 17) != "Exception of type") errorMessage += "  " + msg;

            this.Exception = ex;
            this.ErrorMessage = errorMessage;
        }
        /// <summary>
        /// Handles unexpected System Errors, typically by logging them, and then
        /// re-throwing them.
        /// </summary>
        /// <param name="ex">
        /// A run-time <see cref="System.Exception"/>.
        /// </param>
        public override void ThrowSystemException(Exception ex, IProvider provider)
        {
            if (ex == null) return;

            Log(DebuggerLogLevel.Critical, ex.Message);
            Log(DebuggerLogLevel.StackTrace, ex.StackTrace, true);

            this.Exception = ex;
            this.ErrorMessage = Galatea.Globalization.DiagnosticResources.Debugger_Unexpected_Error_Speech_Message;

            //if (Error != null) Error(provider, new ErrorEventArgs(ex, 
            //    Galatea.Globalization.DiagnosticResources.Debugger_Unexpected_Error_Speech_Message));
        }
        /// <summary>
        /// Logs messages and errors to a log file using a <see cref="IFileLogger"/> instance.
        /// </summary>
        /// <param name="level">
        /// The <see cref="DebuggerLogLevel"/> of the message to be logged.
        /// </param>
        /// <param name="message"> The message to be logged. </param>
        /// <param name="overrideLevel"> 
        /// A boolean value indicating that the Debugger should log the 
        /// message, regardless of <see cref="DebuggerLogLevel"/>.
        /// </param>
        public override void Log(DebuggerLogLevel level, string message, bool overrideLevel)
        {
            if (level >= this.LogLevel || overrideLevel)
            {
                string sOutput = string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    Galatea.Globalization.DiagnosticResources.Debugger_Log_Message_Format,
                    level.GetToken(), System.DateTime.Now, message);

                System.Diagnostics.Debug.WriteLine(sOutput);

                if (_fileLogger != null && _fileLogger.IsLogging)
                {
                    _fileLogger.Log(sOutput);
                }
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            _fileLogger.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// A component class that writes the Galatea.Runtime processes to a file.
        /// </summary>
        private class DebuggerLogger : RuntimeComponent, IFileLogger
        {
            private readonly object fileLock = new object();

            public DebuggerLogger() : base()
            {
            }

            /// <summary>
            /// Gets a boolean indicating that the <see cref="FileLogger"/> is open and writing to a file.
            /// </summary>
            public bool IsLogging { get { return _isLogging; } }
            /// <summary>
            /// Opens the text file for log writing.
            /// </summary>
            /// <param name="filename">
            /// A relative or absolute path for the file that the current FileStream object will
            /// encapsulate.
            /// </param>
            /// <param name="mode">
            /// A <see cref="System.IO.FileMode"/> constant that determines how to open or create the file.
            /// </param>
            public void StartLogging(string filename, FileMode mode)
            {
                _filename = filename;

                var appDataFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var logFileName = Path.Combine(appDataFolder.Path, _filename);

                if (!File.Exists(logFileName))
                {
                    appDataFolder.CreateFileAsync(_filename).GetResults();
                    // TODO:  Make this Async
                }

                // Set status
                _isLogging = true;
            }
            /// <summary>
            /// Closes the <see cref="StreamWriter"/> instance that is writing the log to the file.
            /// </summary>
            public void StopLogging()
            {
                //// Dispose FileStream
                //_writer.Close();

                // Set status
                _isLogging = false;
            }
            /// <summary>
            /// Writes a log message to the file using a <see cref="StreamWriter"/>.
            /// </summary>
            /// <param name="message">
            /// The log message to write to the file.
            /// </param>
            public void Log(string message)
            {
                lock (fileLock)
                {
                    using (FileStream stream = new FileStream(_filename, FileMode.Append))
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        // TODO:  Make so we can open the file and view the Log without stopping the application.
                        writer.WriteLine(message);
                    }
                }
            }
            /// <summary>
            /// Releases the unmanaged resources used by the <see cref="Debugger.FileLogger"/> and 
            /// optionally releases the managed resources.
            /// </summary>
            /// <param name="disposing">
            /// true to release both managed and unmanaged resources; false to release only unmanaged
            /// resources.
            /// </param>
            protected override void Dispose(bool disposing)
            {
                //_writer.Dispose();
                base.Dispose(disposing);
            }

            private string _filename;
            private bool _isLogging;
        }

        private IFileLogger _fileLogger;
    }
}
