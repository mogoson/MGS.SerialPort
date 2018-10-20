/*************************************************************************
 *  Copyright © 2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  VSDebugger.cs
 *  Description  :  Debugger for visual studio.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  9/19/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using Mogoson.DesignPattern;
using System;
using System.Diagnostics;

namespace Mogoson.IO
{
    /// <summary>
    /// Debugger for visual studio.
    /// </summary>
    public sealed class VSDebugger : Singleton<VSDebugger>, ILogger
    {
        #region Private Method
        /// <summary>
        /// Constructor.
        /// </summary>
        private VSDebugger() { }

        /// <summary>
        /// Logs a formatted message.
        /// </summary>
        /// <param name="tag">Tag of log message.</param>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        private void DebugLog(string tag, string format, params object[] args)
        {
            Debug.WriteLine(string.Format("{0} - {1}", tag, string.Format(format, args)));
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Logs a formatted message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public void Log(string format, params object[] args)
        {
            DebugLog("Log", format, args);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public void LogError(string format, params object[] args)
        {
            DebugLog("Error", format, args);
        }

        /// <summary>
        /// Logs a formatted warning message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public void LogWarning(string format, params object[] args)
        {
            DebugLog("Warning", format, args);
        }

        /// <summary>
        /// Logs a formatted exception message.
        /// </summary>
        /// <param name="exception">Runtime exception.</param>
        public void LogException(Exception exception)
        {
            DebugLog("Exception", "{0}\nStackTrace: {1}", exception.Message, exception.StackTrace);
        }
        #endregion
    }
}