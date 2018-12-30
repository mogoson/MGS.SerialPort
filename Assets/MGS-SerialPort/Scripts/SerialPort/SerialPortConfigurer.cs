/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortConfigurer.cs
 *  Description  :  Read config from local file and write config to
 *                  local file.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/4/2017
 *  Description  :  Initial development version.
 *  
 *  Author       :  Mogoson
 *  Version      :  0.1.1
 *  Date         :  10/3/2017
 *  Description  :  Use JsonUtility to serialize and deserialize config.
 *  
 *  Author       :  Mogoson
 *  Version      :  0.1.2
 *  Date         :  3/2/2018
 *  Description  :  Optimize.
 *************************************************************************/

using System;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_5_3_OR_NEWER
using LitJson;
#endif

namespace Mogoson.IO.Ports
{
    /// <summary>
    /// Configurer of SerialPort.
    /// </summary>
    public static class SerialPortConfigurer
    {
        #region Field and Property
        /// <summary>
        /// Full path of serialport config file.
        /// </summary>
        public static readonly string ConfigPath = Application.streamingAssetsPath + "/Config/SerialPortConfig.json";
        #endregion

        #region Public Method
        /// <summary>
        /// Read SerialPortConfig from config file.
        /// </summary>
        /// <returns>Config of serialport.</returns>
        public static SerialPortConfig ReadConfig()
        {
            try
            {
                var json = File.ReadAllText(ConfigPath);
#if UNITY_5_3_OR_NEWER
                return JsonUtility.FromJson<SerialPortConfig>(json);
#else
                return JsonMapper.ToObject<SerialPortConfig>(json);
#endif
            }
            catch (Exception e)
            {
                LogUtility.LogError(e.Message);
                return new SerialPortConfig();
            }
        }

        /// <summary>
        /// Write SerialPortConfig to config file.
        /// </summary>
        /// <param name="config">Config of serialport.</param>
        public static void WriteConfig(SerialPortConfig config)
        {
            try
            {
#if UNITY_5_3_OR_NEWER
                var configJson = JsonUtility.ToJson(config);
#else
                var configJson = JsonMapper.ToJson(config);
#endif
                File.WriteAllText(ConfigPath, configJson);
#if UNITY_EDITOR
                AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                LogUtility.LogError(e.Message);
            }
        }
        #endregion
    }
}