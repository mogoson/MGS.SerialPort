/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *------------------------------------------------------------------------
 *  File         :  SerialPortConfigurer.cs
 *  Description  :  Read config from local file and write config to
 *                  local file.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/4/2017
 *  Description  :  Initial development version.
 *************************************************************************/

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Developer.IO.Ports
{
    /// <summary>
    /// Configurer of SerialPort.
    /// </summary>
    public static class SerialPortConfigurer
    {
        #region Property and Field
        /// <summary>
        /// Full path of serialport config file.
        /// </summary>
        public static string ConfigPath { get { return Application.streamingAssetsPath + "/Config/SerialPortConfig.json"; } }
        #endregion

        #region Public Method
        /// <summary>
        /// Read SerialPortConfig from config file.
        /// </summary>
        /// <param name="config">Config of serialport.</param>
        /// <param name="error">Error message.</param>
        /// <returns>Succeed read.</returns>
        public static bool ReadConfig(out SerialPortConfig config, out string error)
        {
            config = new SerialPortConfig();
            try
            {
                var json = File.ReadAllText(ConfigPath, Encoding.Default);
                config = JsonConvert.DeserializeObject<SerialPortConfig>(json);
                error = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }
        }

        /// <summary>
        /// Write SerialPortConfig to config file.
        /// </summary>
        /// <param name="config">Config of serialport.</param>
        /// <param name="error">Error message.</param>
        /// <returns>Succeed write.</returns>
        public static bool WriteConfig(SerialPortConfig config, out string error)
        {
            try
            {
                var configJson = JsonConvert.SerializeObject(config);
                File.WriteAllText(ConfigPath, configJson, Encoding.Default);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
                error = string.Empty;
                return true;
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }
        }
        #endregion
    }
}