/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SPConfig.cs
 *  Description  :  Config of SerialPorter.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  7/30/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.IO.Ports;

namespace MGS.IO.Ports
{
    /// <summary>
    /// Config of SerialPorter.
    /// </summary>
    [Serializable]
    public class SPConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string portName = "COM1";

        /// <summary>
        /// 
        /// </summary>
        public int baudRate = 9600;

        /// <summary>
        /// 
        /// </summary>
        public Parity parity = Parity.None;

        /// <summary>
        /// 
        /// </summary>
        public int dataBits = 8;

        /// <summary>
        /// 
        /// </summary>
        public StopBits stopBits = StopBits.One;

        /// <summary>
        /// 
        /// </summary>
        public int readInterval = 250;

        /// <summary>
        /// 
        /// </summary>
        public int writeInterval = 250;

        /// <summary>
        /// 
        /// </summary>
        public byte dataHead = 254;

        /// <summary>
        /// 
        /// </summary>
        public int dataSize = 10;

        /// <summary>
        /// 
        /// </summary>
        public byte dataTail = 255;
    }
}