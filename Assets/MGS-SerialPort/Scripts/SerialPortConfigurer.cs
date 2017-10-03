/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *  FileName: SerialPortConfigurer.cs
 *  Author: Mogoson   Version: 0.1.0   Date: 4/4/2017
 *  Version Description:
 *    Internal develop version,mainly to achieve its function.
 *  File Description:
 *    Ignore.
 *  Class List:
 *    <ID>           <name>             <description>
 *     1.     SerialPortConfigurer         Ignore.
 *  Function List:
 *    <class ID>     <name>             <description>
 *     1.
 *  History:
 *    <ID>    <author>      <time>      <version>      <description>
 *     1.     Mogoson     4/4/2017        0.1.0       Create this file.
 *************************************************************************/

namespace Developer.SerialPort
{
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

    /// <summary>
    /// Configurer of SerialPort.
    /// </summary>
    [AddComponentMenu("Developer/SerialPort/SerialPortConfigurer")]
    public class SerialPortConfigurer : MonoBehaviour
    {
        #region Property and Field
        /// <summary>
        /// Config file of serialport.
        /// </summary>
        public string configFile = "SerialPortConfig.json";

        /// <summary>
        /// Full path of serialport config file.
        /// </summary>
        protected string configPath { get { return Application.streamingAssetsPath + "/" + configFile; } }
        #endregion

        #region Public Method
        /// <summary>
        /// Read SerialPortConfig from config file.
        /// </summary>
        /// <param name="config">Config of serialport.</param>
        /// <param name="error">Error message.</param>
        /// <returns>Succeed read.</returns>
        public virtual bool ReadConfig(out SerialPortConfig config, out string error)
        {
            config = new SerialPortConfig();
            try
            {
                var json = File.ReadAllText(configPath, Encoding.Default);
                config = JsonUtility.FromJson<SerialPortConfig>(json);
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
        public virtual bool WriteConfig(SerialPortConfig config, out string error)
        {
            try
            {
                var configJson = JsonUtility.ToJson(config);
                File.WriteAllText(configPath, configJson, Encoding.Default);
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