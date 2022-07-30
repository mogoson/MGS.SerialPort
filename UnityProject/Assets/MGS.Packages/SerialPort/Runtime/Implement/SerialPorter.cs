/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPorter.cs
 *  Description  :  Handler with thread to read and write data
 *                  buffer base SerialPort.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  7/30/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace MGS.IO.Ports
{
    /// <summary>
    /// Handler with thread to read and write data buffer base SerialPort.
    /// </summary>
    public class SerialPorter : ISerialPorter
    {
        /// <summary>
        /// Event on error throw.
        /// </summary>
        public event Action<Exception> OnError;

        /// <summary>
        /// SerialPort is open?
        /// </summary>
        public bool IsOpen { get { return serialPort.IsOpen; } }

        /// <summary>
        /// Thread is reading continued?
        /// </summary>
        public bool IsReading { protected set; get; }

        /// <summary>
        /// Thread is writing continued?
        /// </summary>
        public bool IsWriting { protected set; get; }

        /// <summary>
        /// Bytes buffer read from SerialPort.
        /// </summary>
        public byte[] ReadBytes
        {
            get { return readBytes.Clone() as byte[]; }
        }

        /// <summary>
        /// Bytes buffer read from SerialPort.
        /// </summary>
        protected byte[] readBytes;

        /// <summary>
        /// Bytes buffer write to SerialPort.
        /// </summary>
        public byte[] WriteBytes
        {
            set
            {
                var buffer = new List<byte>();
                buffer.Add(DataHead);
                buffer.AddRange(value);
                buffer.Add(DataTail);
                writeBuffer = buffer.ToArray();
            }
        }

        /// <summary>
        /// Bytes buffer write to SerialPort.
        /// </summary>
        protected byte[] writeBuffer;

        /// <summary>
        /// Interval of thread read.
        /// </summary>
        public int ReadInterval { set; get; }

        /// <summary>
        /// Interval of thread write.
        /// </summary>
        public int WriteInterval { set; get; }

        /// <summary>
        /// Head of data frame.
        /// </summary>
        public byte DataHead { set; get; }

        /// <summary>
        /// Size of data frame.
        /// </summary>
        public int DataSize { set; get; }

        /// <summary>
        /// Tail of data frame.
        /// </summary>
        public byte DataTail { set; get; }

        /// <summary>
        /// SerialPort instance.
        /// </summary>
        protected SerialPort serialPort;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="baudRate"></param>
        /// <param name="parity"></param>
        /// <param name="dataBits"></param>
        /// <param name="stopBits"></param>
        /// <param name="readInterval"></param>
        /// <param name="writeInterval"></param>
        /// <param name="dataHead"></param>
        /// <param name="dataSize"></param>
        /// <param name="dataTail"></param>
        public SerialPorter(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits,
             int readInterval, int writeInterval, byte dataHead, int dataSize, byte dataTail)
        {
            serialPort = new SerialPort(portName, baudRate, parity, dataBits, stopBits);
            ReadInterval = readInterval;
            WriteInterval = writeInterval;
            DataHead = dataHead;
            DataSize = dataSize;
            DataTail = dataTail;
        }

        /// <summary>
        /// Open SerialPort.
        /// </summary>
        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                InvokeOnError(ex);
            }
        }

        /// <summary>
        /// Close SerialPort.
        /// </summary>
        public void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            try
            {
                IsReading = false;
                IsWriting = false;
                serialPort.Close();
            }
            catch (Exception ex)
            {
                InvokeOnError(ex);
            }
        }

        /// <summary>
        /// Read bytes form SerialPort (Once).
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public byte[] Read(int count)
        {
            Open();
            try
            {
                var bytes = new byte[count];
                serialPort.Read(bytes, 0, count);
                return bytes;
            }
            catch (Exception ex)
            {
                InvokeOnError(ex);
            }
            return null;
        }

        /// <summary>
        /// Write bytes to SerialPort (Once).
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public void Write(byte[] bytes)
        {
            Open();
            try
            {
                serialPort.Write(bytes, 0, bytes.Length);
            }
            catch (Exception ex)
            {
                InvokeOnError(ex);
            }
        }

        /// <summary>
        /// Begin read bytes form SerialPort (Continued).
        /// </summary>
        /// <returns></returns>
        public void BeginRead()
        {
            if (IsReading)
            {
                return;
            }
            new Thread(ContinueRead) { IsBackground = true }.Start();
        }

        /// <summary>
        /// End read bytes form SerialPort.
        /// </summary>
        /// <returns></returns>
        public void EndRead()
        {
            IsReading = false;
        }

        /// <summary>
        /// Begin write bytes to SerialPort (Continued).
        /// </summary>
        /// <returns></returns>
        public void BeginWrite()
        {
            if (IsWriting)
            {
                return;
            }
            new Thread(ContinueWrite) { IsBackground = true }.Start();
        }

        /// <summary>
        /// End write bytes to SerialPort.
        /// </summary>
        /// <returns></returns>
        public void EndWrite()
        {
            IsWriting = false;
        }

        /// <summary>
        /// Dispose all resource.
        /// </summary>
        public void Dispose()
        {
            Close();
            serialPort.Dispose();
            serialPort = null;
        }

        /// <summary>
        /// Invoke OnError event.
        /// </summary>
        /// <param name="e"></param>
        protected void InvokeOnError(Exception e)
        {
            if (OnError != null)
            {
                OnError.Invoke(e);
            }
        }

        /// <summary>
        /// Read bytes form SerialPort continued.
        /// </summary>
        protected virtual void ContinueRead()
        {
            //Frame bytes length is dataSize + 2(dataHead + dataTail).
            var frameLength = DataSize + 2;
            var frameBuffer = new List<byte>();

            //SerialPort.BytesToRead can not get in Unity.
            //Try to read more bytes of the SerialPort ReadBuffer to avoid delay.
            var readBuffer = new byte[frameLength * 3];

            Open();
            IsReading = true;
            while (IsReading)
            {
                try
                {
                    //Read bytes from serialport.
                    int readCount = serialPort.Read(readBuffer, 0, readBuffer.Length);

                    //Calculate the last index of double frame bytes to avoid delay.
                    //Under normal circumstances, bouble frame bytes affirm contain a intact frame bytes.
                    int index = readCount - 2 * frameLength;
                    index = index > 0 ? index : 0;

                    //Add filter bytes to frameBuffer.
                    for (; index < readCount; index++)
                    {
                        frameBuffer.Add(readBuffer[index]);
                    }

                    //Check frameBuffer is enough for frame bytes.
                    while (frameBuffer.Count >= frameLength)
                    {
                        //Find dataHead.
                        if (frameBuffer[0] == DataHead)
                        {
                            //Find dataTail, save the intact bytes to readBytes.
                            if (frameBuffer[frameLength - 1] == DataTail)
                            {
                                readBytes = frameBuffer.GetRange(1, DataSize).ToArray();
                            }

                            //Remove the obsolete or invalid frame bytes.
                            frameBuffer.RemoveRange(0, frameLength);
                        }
                        else
                        {
                            //Remove the invalid byte.
                            frameBuffer.RemoveAt(0);
                        }
                    }
                }
                catch (TimeoutException ex)
                {
                    InvokeOnError(ex);
                }
                catch (Exception e)
                {
                    InvokeOnError(e);
                    IsReading = false;
                    break;
                }

                Thread.Sleep(ReadInterval);
            }
        }

        /// <summary>
        /// Write bytes to SerialPort continued.
        /// </summary>
        protected virtual void ContinueWrite()
        {
            Open();
            IsWriting = true;
            while (IsWriting)
            {
                try
                {
                    //Write writeBuffer to serialport
                    serialPort.Write(writeBuffer, 0, writeBuffer.Length);
                }
                catch (TimeoutException ex)
                {
                    InvokeOnError(ex);
                }
                catch (Exception e)
                {
                    InvokeOnError(e);
                    IsWriting = false;
                    break;
                }

                Thread.Sleep(WriteInterval);
            }
        }
    }
}