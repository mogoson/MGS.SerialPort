/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortController.cs
 *  Description  :  Synchronous read and write serialport data.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *  
 *  Author       :  Mogoson
 *  Version      :  0.2.0
 *  Date         :  2/13/2018
 *  Description  :  Singleton pattern version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using Developer.Singleton;
using UnityEngine;

namespace Developer.IO.Ports
{
    /// <summary>
    /// Controller of serialport.
    /// </summary>
    public sealed class SerialPortController : Singleton<SerialPortController>
    {
        #region Field and Property
        /// <summary>
        /// Bytes read from serialport.
        /// </summary>
        public byte[] ReadBytes { get { return readBytes; } }

        /// <summary>
        /// Bytes write to serialport.
        /// </summary>
        public byte[] WriteBytes
        {
            set { value.CopyTo(writeBytes, 0); }
            get { return writeBytes; }
        }

        /// <summary>
        /// SerialPort is open.
        /// </summary>
        public bool IsOpen { get { return serialPort.IsOpen; } }

        /// <summary>
        /// Is reading from serialport.
        /// </summary>
        public bool IsReading { get { return readThread.IsAlive; } }

        /// <summary>
        /// Is writing to serialport.
        /// </summary>
        public bool IsWriting { get { return writeThread.IsAlive; } }

        /// <summary>
        /// Is Timeout reading from serialport.
        /// </summary>
        public bool IsReadTimeout { private set; get; }

        /// <summary>
        /// Is Timeout writing to serialport.
        /// </summary>
        public bool IsWriteTimeout { private set; get; }

        /// <summary>
        /// Target serialport of controller.
        /// </summary>
        private SerialPort serialPort;

        /// <summary>
        /// Config of serialport.
        /// </summary>
        private SerialPortConfig config;

        /// <summary>
        /// Thread to read bytes from serialport.
        /// </summary>
        private Thread readThread;

        /// <summary>
        /// Thread to write bytes to serialport.
        /// </summary>
        private Thread writeThread;

        /// <summary>
        /// Bytes read from serialport.
        /// </summary>
        private byte[] readBytes;

        /// <summary>
        /// Bytes write to serialport.
        /// </summary>
        private byte[] writeBytes;
        #endregion

        #region Private Method
        /// <summary>
        /// Private constructor.
        /// </summary>
        private SerialPortController()
        {
            var error = string.Empty;
            InitializeSerialPort(out error);
        }

        /// <summary>
        /// Initialize serialport.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Initialize base on config file.</returns>
        private bool InitializeSerialPort(out string error)
        {
            //Read config and initialize serialport.
            var isRead = SerialPortConfigurer.ReadConfig(out config, out error);
            serialPort = new SerialPort(config.portName, config.baudRate, config.parity, config.dataBits, config.stopBits)
            {
                ReadBufferSize = config.readBufferSize,
                ReadTimeout = config.readTimeout,
                WriteBufferSize = config.writeBufferSize,
                WriteTimeout = config.writeTimeout
            };

            //Initialize read thread.
            readThread = new Thread(ReadBytesFromBuffer)
            {
                IsBackground = true
            };

            //Initialize write thread.
            writeThread = new Thread(WriteBytesToBuffer)
            {
                IsBackground = true
            };

            //Initialize bytes array.
            readBytes = new byte[config.readCount];
            writeBytes = new byte[config.writeCount];

            if (isRead)
                Debug.Log("Initialize succeed.");
            else
                Debug.LogWarning("Initialize with default config. error : " + error);

            //Return state.
            return isRead;
        }

        /// <summary>
        /// Read bytes from serialport buffer.
        /// </summary>
        private void ReadBytesFromBuffer()
        {
            //SerialPort.BytesToRead can not get in Unity.
            //Try to read all bytes of the SerialPort ReadBuffer to avoid delay.
            //So SerialPort.ReadBufferSize should be try to set small in the config file to reduce memory spending.
            var readBuffer = new byte[serialPort.ReadBufferSize];
            var readCount = 0;
            var index = 0;

            //Frame bytes length is config.readCount + 2(readHead + readTail).
            var frameLength = config.readCount + 2;
            var frameBuffer = new List<byte>();

            while (true)
            {
                try
                {
                    //Read bytes from serialport.
                    readCount = serialPort.Read(readBuffer, 0, readBuffer.Length);
                }
                catch (TimeoutException te)
                {
                    Debug.Log(te.Message);
                    ClearReadBytes();
                    IsReadTimeout = true;
                    Thread.Sleep(config.readCycle);
                    continue;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    readThread.Abort();
                    ClearReadBytes();
                    IsReadTimeout = false;
                    break;
                }

                //Clear read timeout flag.
                IsReadTimeout = false;

                //Calculate the last index of double frame bytes to avoid delay.
                //Under normal circumstances, bouble frame bytes affirm contain a intact frame bytes.
                index = readCount - 2 * frameLength;
                index = index > 0 ? index : 0;

                //Add filter bytes to frameBuffer.
                for (; index < readCount; index++)
                {
                    frameBuffer.Add(readBuffer[index]);
                }

                //Check frameBuffer is enough for frame bytes.
                while (frameBuffer.Count >= frameLength)
                {
                    //Find readHead.
                    if (frameBuffer[0] == config.readHead)
                    {
                        //Find readTail, save the intact bytes to readBytes.
                        if (frameBuffer[frameLength - 1] == config.readTail)
                            readBytes = frameBuffer.GetRange(1, config.readCount).ToArray();
                        else
                            Debug.Log("Discard invalid frame bytes.");

                        //Remove the obsolete or invalid frame bytes.
                        frameBuffer.RemoveRange(0, frameLength);
                    }
                    else
                        //Remove the invalid byte.
                        frameBuffer.RemoveAt(0);
                }
                Thread.Sleep(config.readCycle);
            }
        }

        /// <summary>
        /// Write bytes to serialport buffer.
        /// </summary>
        private void WriteBytesToBuffer()
        {
            //writeBuffer length is config.writeCount + 2(writeHead + writeTail).
            var writeBuffer = new byte[config.writeCount + 2];

            //Add writeHead and writeTail to writeBuffer.
            writeBuffer[0] = config.writeHead;
            writeBuffer[config.writeCount + 1] = config.writeTail;

            while (true)
            {
                //Add writeBytes to writeBuffer.
                writeBytes.CopyTo(writeBuffer, 1);
                try
                {
                    //Write writeBuffer to serialport
                    serialPort.Write(writeBuffer, 0, writeBuffer.Length);
                }
                catch (TimeoutException te)
                {
                    Debug.Log(te.Message);
                    IsWriteTimeout = true;
                    Thread.Sleep(config.writeCycle);
                    continue;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    writeThread.Abort();
                    IsWriteTimeout = false;
                    break;
                }

                //Clear write timeout flag.
                IsWriteTimeout = false;
                Thread.Sleep(config.writeCycle);
            }
        }

        /// <summary>
        /// Clear the elements of ReadBytes to default value(zero).
        /// </summary>
        private void ClearReadBytes()
        {
            Array.Clear(readBytes, 0, readBytes.Length);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// ReInitialize serialport.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>ReInitialize succeed.</returns>
        public bool ReInitializeSerialPort(out string error)
        {
            if (CloseSerialPort(out error))
            {
                InitializeSerialPort(out error);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Open serialport.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Open succeed.</returns>
        public bool OpenSerialPort(out string error)
        {
            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            error = string.Empty;
            Debug.Log("Open succeed.");
            return true;
        }

        /// <summary>
        /// Close serialport.
        /// This method will try abort thread if it is alive.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Close succeed.</returns>
        public bool CloseSerialPort(out string error)
        {
            if (IsReading)
            {
                if (!StopRead(out error))
                    return false;
            }
            if (IsWriting)
            {
                if (!StopWrite(out error))
                    return false;
            }
            try
            {
                serialPort.Close();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            error = string.Empty;
            Debug.Log("Close succeed.");
            return true;
        }

        /// <summary>
        /// Start thread to read.
        /// This method will try to open serialport if it is not open.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Start read thread succeed.</returns>
        public bool StartRead(out string error)
        {
            //SerialPort.ReceivedBytesThreshold is not implemented in Unity.
            //SerialPort.DataReceived event is can not work in Unity.
            //So use thread to read bytes.

            if (!IsOpen)
            {
                if (!OpenSerialPort(out error))
                    return false;
            }
            if (!IsReading)
            {
                //readThread can not start after readThread.Abort().
                //New readThread.
                readThread = new Thread(ReadBytesFromBuffer)
                {
                    IsBackground = true
                };
            }
            try
            {
                //SerialPort.DiscardInBuffer can not work in Unity.
                //Do not do it is ok.
                serialPort.DiscardInBuffer();
                readThread.Start();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            error = string.Empty;
            Debug.Log("Start read succeed.");
            return true;
        }

        /// <summary>
        /// Stop thread of read.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop read thread succeed.</returns>
        public bool StopRead(out string error)
        {
            try
            {
                readThread.Abort();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            ClearReadBytes();
            IsReadTimeout = false;
            error = string.Empty;
            Debug.Log("Stop read succeed.");
            return true;
        }

        /// <summary>
        /// Start thread to write.
        /// This method will try to open serialport if it is not open.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Start write thread succeed.</returns>
        public bool StartWrite(out string error)
        {
            if (!IsOpen)
            {
                if (!OpenSerialPort(out error))
                    return false;
            }
            if (!IsWriting)
            {
                //writeThread can not start after writeThread.Abort().
                //New writeThread.
                writeThread = new Thread(WriteBytesToBuffer)
                {
                    IsBackground = true
                };
            }
            try
            {
                //SerialPort.DiscardOutBuffer can not work in Unity.
                //Do not do it is ok.
                serialPort.DiscardOutBuffer();
                writeThread.Start();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            error = string.Empty;
            Debug.Log("Start write succeed.");
            return true;
        }

        /// <summary>
        /// Stop thread of write.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop write thread succeed.</returns>
        public bool StopWrite(out string error)
        {
            try
            {
                writeThread.Abort();
            }
            catch (Exception e)
            {
                error = e.Message;
                Debug.LogError(error);
                return false;
            }

            IsWriteTimeout = false;
            error = string.Empty;
            Debug.Log("Stop write succeed.");
            return true;
        }

        /// <summary>
        /// Clear the elements of WriteBytes to default value(zero).
        /// </summary>
        public void ClearWriteBytes()
        {
            Array.Clear(writeBytes, 0, writeBytes.Length);
        }
        #endregion
    }
}