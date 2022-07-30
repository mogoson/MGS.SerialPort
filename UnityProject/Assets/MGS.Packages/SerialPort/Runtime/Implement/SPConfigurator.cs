/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SPConfigurator.cs
 *  Description  :  Configurator for SerialPorter.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  7/30/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.IO;
using UnityEngine;

namespace MGS.IO.Ports
{
    /// <summary>
    /// Configurator for SerialPorter.
    /// </summary>
    public class SPConfigurator : ISPConfigurator
    {
        /// <summary>
        /// Path of config file.
        /// </summary>
        public string FilePath { protected set; get; }

        /// <summary>
        /// Config read from file.
        /// </summary>
        public SPConfig Config { protected set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath"></param>
        public SPConfigurator(string filePath)
        {
            FilePath = filePath;
            Config = ReadCfg();
        }

        /// <summary>
        /// Read config from file.
        /// </summary>
        /// <returns>Config read from file. (You can handle the result or get it from the property Config)</returns>
        public SPConfig ReadCfg()
        {
            try
            {
                var json = File.ReadAllText(FilePath);
                Config = JsonUtility.FromJson<SPConfig>(json);
            }
            catch (Exception ex)
            {
                LogError("Read serialport config from file exception: {0}\r\n{1}", ex.Message, ex.StackTrace);
                Config = new SPConfig();
            }
            return Config;
        }

        /// <summary>
        /// Write config from file.
        /// </summary>
        /// <returns></returns>
        public bool WriteCfg()
        {
            try
            {
                var configJson = JsonUtility.ToJson(Config);
                var dir = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                File.WriteAllText(FilePath, configJson);
                return true;
            }
            catch (Exception ex)
            {
                LogError("Write serialport config to file exception: {0}\r\n{1}", ex.Message, ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        /// Dispose all resources.
        /// </summary>
        public void Dispose()
        {
            FilePath = null;
            Config = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        private void LogError(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }
    }
}