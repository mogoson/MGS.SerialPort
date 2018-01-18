/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *------------------------------------------------------------------------
 *  File         :  SerialPortController.cs
 *  Description  :  Synchronous read and write serialport data.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

namespace Developer.IO.Ports
{
    public class SerialPortController : MonoBehaviour
    {
        #region Property and Field
        /// <summary>
        /// Bytes read from serialport.
        /// </summary>
        public byte[] ReadBytes { protected set; get; }

        /// <summary>
        /// Bytes write to serialport.
        /// </summary>
        public byte[] WriteBytes { set; get; }

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
        /// Target serialport of manager.
        /// </summary>
        protected SerialPort serialPort;

        /// <summary>
        /// Config of serialport.
        /// </summary>
        protected SerialPortConfig config;

        /// <summary>
        /// Thread to read bytes from serialport..
        /// </summary>
        protected Thread readThread;

        /// <summary>
        /// Thread to write bytes to serialport.
        /// </summary>
        protected Thread writeThread;
        #endregion

        #region Protected Method
        protected virtual void Awake()
        {
            var error = string.Empty;
            InitializeSerialPort(out error);
        }

        protected virtual void OnDestroy()
        {
            var error = string.Empty;
            CloseSerialPort(out error);
        }

        /// <summary>
        /// Initialize serialport.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Initialize normal.</returns>
        protected virtual bool InitializeSerialPort(out string error)
        {
            //Read comfig and initialize serialport.
            var normal = SerialPortConfigurer.ReadConfig(out config, out error);
            serialPort = new SerialPort(config.portName, config.baudRate, config.parity, config.dataBits, config.stopBits)
            {
                ReadBufferSize = config.readBufferSize,
                ReadTimeout = config.readTimeout,
                WriteBufferSize = config.writeBufferSize,
                WriteTimeout = config.writeTimeout
            };

            //Initialize thread.
            readThread = new Thread(ReadBytesFromBuffer);
            writeThread = new Thread(WriteBytesToBuffer);

            //Initialize bytes array.
            ReadBytes = new byte[config.readCount];
            WriteBytes = new byte[config.writeCount];

            if (normal)
                Debug.Log("Initialize Succeed.");
            else
                Debug.LogWarning("Initialize with default config. error : " + error);

            //Return state.
            return normal;
        }

        /// <summary>
        /// Read bytes from serialport buffer.
        /// </summary>
        protected virtual void ReadBytesFromBuffer()
        {
            //SerialPort.BytesToRead can not get in Unity.
            //Try to read all bytes of the SerialPort ReadBuffer to avoid delay.
            //So SerialPort.ReadBufferSize should be try to set small in the config file to reduce memory spending.
            var buffer = new byte[serialPort.ReadBufferSize];
            var count = 0;

            //Frame bytes length is config.readCount + 2(readHead + readTail).
            var frame = config.readCount + 2;
            var index = 0;
            var readBuffer = new List<byte>();

            while (true)
            {
                try
                {
                    //Read bytes from serialport.
                    count = serialPort.Read(buffer, 0, buffer.Length);
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(TimeoutException))
                    {
                        Debug.Log(e.Message);
                        continue;
                    }
                    else
                    {
                        Debug.LogError(e.Message);
                        readThread.Abort();
                        break;
                    }
                }

                //Calculate the last index of double frame bytes to avoid delay.
                //Under normal circumstances, bouble frame bytes affirm contain a intact frame bytes.
                index = count - 2 * frame;
                index = index > 0 ? index : 0;

                //Add select bytes to readBuffer.
                for (; index < count; index++)
                {
                    readBuffer.Add(buffer[index]);
                }

                //Check readBuffer is enough for frame bytes.
                while (readBuffer.Count >= frame)
                {
                    //Find readHead.
                    if (readBuffer[0] == config.readHead)
                    {
                        //Find readTail, save the intact bytes to readBytes.
                        if (readBuffer[frame - 1] == config.readTail)
                            ReadBytes = readBuffer.GetRange(1, config.readCount).ToArray();

                        //Remove the obsolete or invalid frame bytes.
                        readBuffer.RemoveRange(0, frame);
                    }
                    else
                        //Remove the invalid byte.
                        readBuffer.RemoveAt(0);
                }

                Thread.Sleep(config.readCycle);
            }
        }

        /// <summary>
        /// Write bytes to serialport buffer.
        /// </summary>
        protected virtual void WriteBytesToBuffer()
        {
            //writeBuffer length is config.writeCount + 2(writeHead + writeTail).
            var writeBuffer = new byte[config.writeCount + 2];

            while (true)
            {
                //writeBytes length match config.
                if (WriteBytes.Length == config.writeCount)
                {
                    //Add writeHead and writeTail to writeBuffer.
                    writeBuffer[0] = config.writeHead;
                    writeBuffer[config.writeCount + 1] = config.writeTail;

                    //Add writeBytes to writeBuffer.
                    WriteBytes.CopyTo(writeBuffer, 1);

                    try
                    {
                        //Write writeBuffer to serialport
                        serialPort.Write(writeBuffer, 0, writeBuffer.Length);
                    }
                    catch (Exception e)
                    {
                        if (e.GetType() == typeof(TimeoutException))
                            Debug.Log(e.Message);
                        else
                        {
                            Debug.LogError(e.Message);
                            writeThread.Abort();
                        }
                    }
                }
                else
                    Debug.LogWarning("Length of writeBytes is not match config.");

                Thread.Sleep(config.writeCycle);
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// ReInitialize serialport.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>ReInitialize succeeded.</returns>
        public virtual bool ReInitializeSerialPort(out string error)
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
        /// <returns>Open succeeded.</returns>
        public virtual bool OpenSerialPort(out string error)
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
            Debug.Log("Open Succeed.");
            return true;
        }

        /// <summary>
        /// Close serialport.
        /// This method will try abort thread if it is alive.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Close succeeded.</returns>
        public virtual bool CloseSerialPort(out string error)
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
            Debug.Log("Close Succeed.");
            return true;
        }

        /// <summary>
        /// Start thread to read.
        /// This method will try to open serialport if it is not opened.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Start read thread succeeded.</returns>
        public virtual bool StartRead(out string error)
        {
            //SerialPort.ReceivedBytesThreshold is not implemented in Unity3D.
            //SerialPort.DataReceived event is can not work in Unity3D.
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
                readThread = new Thread(ReadBytesFromBuffer);
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
            Debug.Log("Start Read Succeed.");
            return true;
        }

        /// <summary>
        /// Stop thread of read.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop read thread succeeded.</returns>
        public virtual bool StopRead(out string error)
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

            error = string.Empty;
            Debug.Log("Stop Read Succeed.");
            return true;
        }

        /// <summary>
        /// Start thread to write.
        /// This method will try to open serialport if it is not opened.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Start write thread succeeded.</returns>
        public virtual bool StartWrite(out string error)
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
                writeThread = new Thread(WriteBytesToBuffer);
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
            Debug.Log("Start Write Succeed.");
            return true;
        }

        /// <summary>
        /// Stop thread of write.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop write thread succeeded.</returns>
        public virtual bool StopWrite(out string error)
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

            error = string.Empty;
            Debug.Log("Stop Write Succeed.");
            return true;
        }
        #endregion
    }
}