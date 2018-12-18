using System;
using System.Collections.Generic;
using System.Text;

namespace Galatea.Diagnostics
{
    /// <summary>
    /// Contains functions for converting a <see cref="DebuggerLogLevel"/> enum 
    /// value into a text value for logging.
    /// </summary>
    public static class DebuggerLogLevelExtension
    {
        /// <summary>
        /// Converts a <see cref="DebuggerLogLevel"/> enum value into a text value
        /// for logging.
        /// </summary>
        /// <param name="value">
        /// The <see cref="DebuggerLogLevel"/> enum value to convert.
        /// </param>
        /// <returns>
        /// A tet value.
        /// </returns>
        public static string GetToken(this DebuggerLogLevel value)
        {
            switch (value)
            {
                case DebuggerLogLevel.Diagnostic: return " ^^^ ";
                case DebuggerLogLevel.Log: return " Log ";
                case DebuggerLogLevel.Event: return "Event";
                case DebuggerLogLevel.Message: return " Msg ";
                case DebuggerLogLevel.Warning: return "Warn ";
                case DebuggerLogLevel.Error: return "Error";
                case DebuggerLogLevel.Critical: return "*ERR*";
                case DebuggerLogLevel.StackTrace: return "TRACE";
                default: throw new Galatea.TeaArgumentException();
            }
        }
    }
}