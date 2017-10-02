/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *  FileName: SerialPortConfig.cs
 *  Author: Mogoson   Version: 0.1.0   Date: 4/5/2017
 *  Version Description:
 *    Internal develop version,mainly to achieve its function.
 *  File Description:
 *    Ignore.
 *  Class List:
 *    <ID>           <name>             <description>
 *     1        SerialPortConfig           Ignore.
 *  Function List:
 *    <class ID>     <name>             <description>
 *     1.
 *  History:
 *    <ID>    <author>      <time>      <version>      <description>
 *     1.     Mogoson     4/5/2017       0.1.0        Create this file.
 *************************************************************************/

namespace Developer.SerialPort
{
    using System.IO.Ports;

    /// <summary>
    /// Config of SerialPort.
    /// </summary>
    public class SerialPortConfig
    {
        public string portName = "COM1";
        public int baudRate = 9600;
        public Parity parity = Parity.None;
        public int dataBits = 8;
        public StopBits stopBits = StopBits.One;

        public int readBufferSize = 1024;
        public int readTimeout = 500;
        public byte readHead = 254;
        public byte readTail = 255;
        public int readCount = 10;
        public int readCycle = 250;

        public int writeBufferSize = 1024;
        public int writeTimeout = 500;
        public byte writeHead = 254;
        public byte writeTail = 255;
        public int writeCount = 10;
        public int writeCycle = 250;

        /// <summary>
        /// Default Constructor.
        /// </summary>
        public SerialPortConfig() { }

        /// <summary>
        /// Constructor of SerialPortConfig.
        /// </summary>
        public SerialPortConfig(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;
        }
    }
}