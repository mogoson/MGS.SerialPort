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
    [AddComponentMenu("Developer/IO/Ports/SerialPortController")]
    public class SerialPortController : MonoBehaviour
    {
        #region Property and Field
        /// <summary>
        /// SerialPortConfigurer of SerialPort.
        /// </summary>
        public SerialPortConfigurer configurer;

        /// <summary>
        /// Bytes read from serialport.
        /// </summary>
        public byte[] readBytes { protected set; get; }

        /// <summary>
        /// Bytes write to serialport.
        /// </summary>
        public byte[] writeBytes { set; get; }

        /// <summary>
        /// SerialPort is open.
        /// </summary>
        public bool isOpen { get { return serialPort.IsOpen; } }

        /// <summary>
        /// Is reading from serialport.
        /// </summary>
        public bool isReading { get { return readThread.IsAlive; } }

        /// <summary>
        /// Is writing to serialport.
        /// </summary>
        public bool isWriting { get { return writeThread.IsAlive; } }

        /// <summary>
        /// Loop read bytes from serialport.
        /// </summary>
        protected bool loopRead;

        /// <summary>
        /// Loop write bytes to serialport.
        /// </summary>
        protected bool loopWrite;

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
            string error;
            if (InitialiseSerialPort(out error))
                Debug.Log("Initialise Succeed.");
            else
                Debug.LogWarning("Initialise with default config. error : " + error);
        }

        protected virtual void OnApplicationQuit()
        {
            string error;
            if (CloseSerialPort(out error))
                Debug.Log("Close Succeed.");
            else
                Debug.LogError(error);
        }

        /// <summary>
        /// Read bytes from serialport.
        /// </summary>
        protected virtual void ReadBytes()
        {
            var readBuffer = new List<byte>();
            while (loopRead)
            {
                Thread.Sleep(config.readCycle);
                try
                {
                    //SerialPort.BytesToRead can not get in Unity.
                    //Try to read all bytes of the SerialPort ReadBuffer to avoid delay.
                    //So SerialPort.ReadBuffer should be try to set small in the config file to reduce memory spending.
                    var buffer = new byte[serialPort.ReadBufferSize];
                    var count = serialPort.Read(buffer, 0, buffer.Length);

                    //Frame bytes length is config.readCount + 2(readHead + readTail).
                    //Calculate the last index of double frame bytes to avoid delay.
                    //Under normal circumstances, bouble frame bytes affirm contain a intact frame bytes.
                    var frame = config.readCount + 2;
                    var index = count - 2 * frame;
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
                                readBytes = readBuffer.GetRange(1, config.readCount).ToArray();

                            //Remove the obsolete or invalid frame bytes.
                            readBuffer.RemoveRange(0, frame);
                        }
                        else
                            //Remove the invalid byte.
                            readBuffer.RemoveAt(0);
                    }
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(TimeoutException))
                        Debug.Log(e.Message);
                    else
                    {
                        Debug.LogError(e.Message);
                        loopRead = false;
                        readThread.Abort();
                    }
                }
            }
        }

        /// <summary>
        /// Write bytes to serialport.
        /// </summary>
        protected virtual void WriteBytes()
        {
            while (loopWrite)
            {
                Thread.Sleep(config.writeCycle);
                try
                {
                    //writeBytes length match config.
                    if (writeBytes.Length == config.writeCount)
                    {
                        //writeBuffer length is config.writeCount + 2(writeHead + writeTail).
                        var writeBuffer = new byte[config.writeCount + 2];

                        //Add writeHead and writeTail to writeBuffer.
                        writeBuffer[0] = config.writeHead;
                        writeBuffer[config.writeCount + 1] = config.writeTail;

                        //Add writeBytes to writeBuffer and write it to serialport.
                        writeBytes.CopyTo(writeBuffer, 1);
                        serialPort.Write(writeBuffer, 0, writeBuffer.Length);
                    }
                    else
                        Debug.LogWarning("Length of writeBytes is not match config.");
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(TimeoutException))
                        Debug.Log(e.Message);
                    else
                    {
                        Debug.LogError(e.Message);
                        loopWrite = false;
                        writeThread.Abort();
                    }
                }
            }
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Initialise serialport.
        /// </summary>
        /// <param name="error">Error message</param>
        /// <returns>Initialise normal.</returns>
        public virtual bool InitialiseSerialPort(out string error)
        {
            //Read comfig and initialise serialport.
            var normal = configurer.ReadConfig(out config, out error);
            serialPort = new SerialPort(config.portName, config.baudRate, config.parity, config.dataBits, config.stopBits);
            serialPort.ReadBufferSize = config.readBufferSize;
            serialPort.ReadTimeout = config.readTimeout;
            serialPort.WriteBufferSize = config.writeBufferSize;
            serialPort.WriteTimeout = config.writeTimeout;

            //Initialise thread.
            readThread = new Thread(ReadBytes);
            writeThread = new Thread(WriteBytes);

            //Initialise bytes array.
            readBytes = new byte[config.readCount];
            writeBytes = new byte[config.writeCount];

            //Return state.
            return normal;
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
        /// Close serialport.
        /// This method will try abort thread if it is alive.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Close succeeded.</returns>
        public virtual bool CloseSerialPort(out string error)
        {
            if (isReading)
            {
                if (!StopRead(out error))
                    return false;
            }
            if (isWriting)
            {
                if (!StopWrite(out error))
                    return false;
            }
            try
            {
                serialPort.Close();
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

            if (!isOpen)
            {
                if (!OpenSerialPort(out error))
                    return false;
            }
            if (!isReading)
            {
                //readThread can not start after readThread.Abort().
                //New readThread.
                readThread = new Thread(ReadBytes);
            }
            try
            {
                //SerialPort.DiscardInBuffer can not work in Unity.
                //Do not do it is ok.

                loopRead = true;
                readThread.Start();
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
        /// Stop thread of read.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop read thread succeeded.</returns>
        public virtual bool StopRead(out string error)
        {
            try
            {
                loopRead = false;
                readThread.Abort();
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
        /// Start thread to write.
        /// This method will try to open serialport if it is not opened.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Start write thread succeeded.</returns>
        public virtual bool StartWrite(out string error)
        {
            if (!isOpen)
            {
                if (!OpenSerialPort(out error))
                    return false;
            }
            if (!isWriting)
            {
                //writeThread can not start after writeThread.Abort().
                //New writeThread.
                writeThread = new Thread(WriteBytes);
            }
            try
            {
                //SerialPort.DiscardOutBuffer can not work in Unity.
                //Do not do it is ok.

                loopWrite = true;
                writeThread.Start();
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
        /// Stop thread of write.
        /// </summary>
        /// <param name="error">Error message.</param>
        /// <returns>Stop write thread succeeded.</returns>
        public virtual bool StopWrite(out string error)
        {
            try
            {
                loopWrite = false;
                writeThread.Abort();
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