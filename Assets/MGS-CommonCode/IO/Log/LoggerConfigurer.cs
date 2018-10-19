/*************************************************************************
 *  Copyright © 2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  LoggerConfigurer.cs
 *  Description  :  Configurer of system logger.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  9/19/2018
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Mogoson.IO
{
    /// <summary>
    /// Configurer of system logger.
    /// </summary>
    static class LoggerConfigurer
    {
        #region Field and Property
#if !UNITY_EDITOR
        /// <summary>
        /// Path of log file.
        /// </summary>
        static readonly string FilePath = Application.persistentDataPath + "/Log.txt";
#endif
        #endregion

        #region Public Method
        /// <summary>
        /// Initialize system logger.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        static void Initialize()
        {
#if UNITY_EDITOR
            Logger.Set(UnityDebugger.Instance);
#else
            FileLogger.Instance.FilePath = FilePath;
            Logger.Set(FileLogger.Instance);
#endif
        }
        #endregion
    }
}