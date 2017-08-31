/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson tech. Co., Ltd.
 *  FileName: SerialPortConfigurer.cs
 *  Author: Mogoson   Version: 1.0   Date: 4/4/2017
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
 *     1.     Mogoson     4/4/2017       1.0        Build this file.
 *************************************************************************/

namespace Developer.SerialPort
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.IO.Ports;
    using System.Text;
    using UnityEngine;

    /// <summary>
    /// SerialPort Configurer.
    /// </summary>
    [AddComponentMenu("Developer/SerialPort/SerialPortConfigurer")]
    public class SerialPortConfigurer : MonoBehaviour
    {
        #region Property and Field
        /// <summary>
        /// Config file of serialport.
        /// </summary>
        public string configFile = "SerialPortConfig.txt";

        /// <summary>
        /// Config file path of serialport.
        /// </summary>
        protected string configPath { get { return Application.dataPath + "/" + configFile; } }

        /// <summary>
        /// Separator of config text.
        /// </summary>
        protected char[] separator = { ':', '\n' };
        #endregion

        #region Public Method
        /// <summary>
        /// Read serialport config from file.
        /// Return SerialPortConfig.Default if any error when read file.
        /// </summary>
        /// <param name="config">Serialport config.</param>
        /// <param name="error">Error message.</param>
        /// <returns>Read config from file.</returns>
        public virtual bool ReadConfig(out SerialPortConfig config, out string error)
        {
            config = SerialPortConfig.Default;
            try
            {
                var configTextSplit = File.ReadAllText(configPath, Encoding.Default).Split(separator);
                config.portName = configTextSplit[1].Trim();
                config.baudRate = int.Parse(configTextSplit[3].Trim());
                config.parity = (Parity)(int.Parse(configTextSplit[5].Trim()));
                config.dataBits = int.Parse(configTextSplit[7].Trim());
                config.stopBits = (StopBits)(int.Parse(configTextSplit[9].Trim()));
                config.readBufferSize = int.Parse(configTextSplit[11].Trim());
                config.writeBufferSize = int.Parse(configTextSplit[13].Trim());
                config.readTimeout = int.Parse(configTextSplit[15].Trim());
                config.writeTimeout = int.Parse(configTextSplit[17].Trim());
                config.readHead = byte.Parse(configTextSplit[19].Trim(), NumberStyles.HexNumber);
                config.readTail = byte.Parse(configTextSplit[21].Trim(), NumberStyles.HexNumber);
                config.writeHead = byte.Parse(configTextSplit[23].Trim(), NumberStyles.HexNumber);
                config.writeTail = byte.Parse(configTextSplit[25].Trim(), NumberStyles.HexNumber);
                config.readCount = int.Parse(configTextSplit[27].Trim());
                config.writeCount = int.Parse(configTextSplit[29].Trim());
                config.readCycle = int.Parse(configTextSplit[31].Trim());
                config.writeCycle = int.Parse(configTextSplit[33].Trim());
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
        /// Write serialport config to file.
        /// </summary>
        /// <param name="config">Serialport config.</param>
        public virtual void WriteConfig(SerialPortConfig config)
        {
            string[] configTextLines = {
                "PortName: " + config.portName,
                "BaudRate: " + config.baudRate,
                "Parity: " + (int)config.parity,
                "DataBits: " + config.dataBits,
                "Stopbits: " + (int)config.stopBits,
                "ReadBufferSize: " + config.readBufferSize,
                "WriteBufferSize: " + config.writeBufferSize,
                "ReadTimeout: " + config.readTimeout,
                "WriteTimeout: " + config.writeTimeout,
                "ReadHead: " + config.readHead.ToString("X2"),
                "ReadTail: " + config.readTail.ToString("X2"),
                "WriteHead: " + config.writeHead.ToString("X2"),
                "WriteTail: " + config.writeTail.ToString("X2"),
                "ReadCount: " + config.readCount,
                "WriteCount: " + config.writeCount,
                "ReadCycle: " + config.readCycle,
                "WriteCycle: " + config.writeCycle
            };
            File.WriteAllLines(configPath, configTextLines, Encoding.Default);
        }
        #endregion
    }
}