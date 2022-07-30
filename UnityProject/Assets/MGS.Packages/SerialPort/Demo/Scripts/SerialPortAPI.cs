/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortAPI.cs
 *  Description  :  API of SerialPort for demo.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  7/30/2022
 *  Description  :  Initial development version.
 *************************************************************************/

namespace MGS.IO.Ports
{
    public sealed partial class SerialPortAPI
    {
        public static void RebuildPorter()
        {
            var cfg = Configurator.ReadCfg();
            SerialPorter = new SerialPorter(cfg.portName, cfg.baudRate, cfg.parity, cfg.dataSize, cfg.stopBits,
                cfg.readInterval, cfg.writeInterval, cfg.dataHead, cfg.dataSize, cfg.dataTail);
        }
    }
}