using System;
using Galatea;

namespace Gala.Dolly.Test
{
    using Galatea.Diagnostics;

    internal class TestDebugger : Galatea.Runtime.Services.Debugger
    {
        /// <summary>
        /// Handles expected Galatea Core Exceptions, typically by logging them.
        /// </summary>
        /// <param name="ex">
        /// A run-time <see cref="TeaException"/>.
        /// </param>
        protected override void HandleTeaException(TeaException ex)
        {
            string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            Log(DebuggerLogLevel.Error, msg);
            Log(DebuggerLogLevel.StackTrace, ex.StackTrace, true);

            throw (ex);
        }
        /// <summary>
        /// Handles unexpected System Errors, typically by logging them, and then
        /// re-throwing them.
        /// </summary>
        /// <param name="ex">
        /// A run-time <see cref="System.Exception"/>.
        /// </param>
        protected override void ThrowSystemException(Exception ex)
        {
            Log(DebuggerLogLevel.Critical, ex.Message);
            Log(DebuggerLogLevel.StackTrace, ex.StackTrace, true);

            throw (ex);
        }
    }
}
