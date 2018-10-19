/*************************************************************************
 *  Copyright © 2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  Logger.cs
 *  Description  :  Logger of system.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  9/5/2015
 *  Description  :  Initial development version.
 *************************************************************************/

namespace Mogoson.IO
{
    /// <summary>
    /// Logger of system.
    /// </summary>
    public static class Logger
    {
        #region Field and Property
        /// <summary>
        /// Inner logger.
        /// </summary>
        private static ILogger innerLogger;
        #endregion

        #region Public Method
        /// <summary>
        /// Set the inner logger.
        /// </summary>
        /// <param name="logger">Inner logger.</param>
        public static void Set(ILogger logger)
        {
            innerLogger = logger;
        }

        /// <summary>
        /// Logs a formatted message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void Log(string format, params object[] args)
        {
            if (innerLogger != null)
                innerLogger.Log(format, args);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogError(string format, params object[] args)
        {
            if (innerLogger != null)
                innerLogger.LogError(format, args);
        }

        /// <summary>
        /// Logs a formatted warning message.
        /// </summary>
        /// <param name="format">A composite format string.</param>
        /// <param name="args">Format arguments.</param>
        public static void LogWarning(string format, params object[] args)
        {
            if (innerLogger != null)
                innerLogger.LogWarning(format, args);
        }
        #endregion
    }
}