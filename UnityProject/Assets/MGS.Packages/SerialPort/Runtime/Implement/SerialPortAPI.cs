/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortAPI.cs
 *  Description  :  API of SerialPorter.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  7/30/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace MGS.IO.Ports
{
    /// <summary>
    /// API of SerialPorter.
    /// </summary>
    public sealed partial class SerialPortAPI
    {
        /// <summary>
        /// A global instance for API of SerialPorter.
        /// </summary>
        public static ISerialPorter SerialPorter { private set; get; }

        /// <summary>
        /// A global instance for SPConfigurator.
        /// </summary>
        public static ISPConfigurator Configurator { private set; get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        static SerialPortAPI()
        {
            var file = Application.persistentDataPath + "/Config/SPConfig.json";
            Configurator = new SPConfigurator(file);

            var cfg = Configurator.Config;
            SerialPorter = new SerialPorter(cfg.portName, cfg.baudRate, cfg.parity, cfg.dataSize, cfg.stopBits,
                cfg.readInterval, cfg.writeInterval, cfg.dataHead, cfg.dataSize, cfg.dataTail);
        }
    }
}