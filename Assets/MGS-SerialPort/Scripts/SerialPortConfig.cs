/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson tech. Co., Ltd.
 *  FileName: SerialPortConfig.cs
 *  Author: Mogoson   Version: 1.0   Date: 4/5/2017
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
 *     1.     Mogoson     4/5/2017       1.0        Build this file.
 *************************************************************************/

namespace Developer.SerialPort
{
    using System.IO.Ports;

    /// <summary>
    /// SerialPort Config.
    /// </summary>
    public struct SerialPortConfig
    {
        public string portName;
        public int baudRate;
        public Parity parity;
        public int dataBits;
        public StopBits stopBits;

        public int readBufferSize;
        public int readTimeout;
        public byte readHead;
        public byte readTail;
        public int readCount;
        public int readCycle;

        public int writeBufferSize;
        public int writeTimeout;
        public byte writeHead;
        public byte writeTail;
        public int writeCount;
        public int writeCycle;

        /// <summary>
        /// SerialPortConfig constructor.
        /// </summary>
        public SerialPortConfig(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits,
            int readBufferSize = 4096, int readTimeout = 500, byte readHead = 254, byte readTail = 255, int readCount = 10, int readCycle = 250,
            int writeBufferSize = 4096, int writeTimeout = 500, byte writeHead = 254, byte writeTail = 255, int writeCount = 10, int writeCycle = 250)
        {
            this.portName = portName;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;

            this.readBufferSize = readBufferSize;
            this.readTimeout = readTimeout;
            this.readHead = readHead;
            this.readTail = readTail;
            this.readCount = readCount;
            this.readCycle = readCycle;

            this.writeBufferSize = writeBufferSize;
            this.writeTimeout = writeTimeout;
            this.writeHead = writeHead;
            this.writeTail = writeTail;
            this.writeCount = writeCount;
            this.writeCycle = writeCycle;
        }

        public static SerialPortConfig Default { get { return new SerialPortConfig("COM1", 9600, Parity.None, 8, StopBits.One); } }
    }
}