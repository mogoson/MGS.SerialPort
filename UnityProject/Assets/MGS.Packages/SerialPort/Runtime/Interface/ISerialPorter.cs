/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ISerialPorter.cs
 *  Description  :  Interface of serial porter.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  1.0
 *  Date         :  7/27/2022
 *  Description  :  Initial development version.
 *************************************************************************/

using System;

namespace MGS.IO.Ports
{
    /// <summary>
    /// Interface of serial porter.
    /// </summary>
    public interface ISerialPorter : IDisposable
    {
        /// <summary>
        /// Event on error throw.
        /// </summary>
        event Action<Exception> OnError;

        /// <summary>
        /// SerialPort is open?
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Thread is reading continued?
        /// </summary>
        bool IsReading { get; }

        /// <summary>
        /// Thread is writing continued?
        /// </summary>
        bool IsWriting { get; }

        /// <summary>
        /// Interval of thread read.
        /// </summary>
        byte[] ReadBytes { get; }

        /// <summary>
        /// Interval of thread write.
        /// </summary>
        byte[] WriteBytes { set; }

        /// <summary>
        /// Open SerialPort.
        /// </summary>
        void Open();

        /// <summary>
        /// Close SerialPort.
        /// </summary>
        void Close();

        /// <summary>
        /// Read bytes form SerialPort (Once).
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        byte[] Read(int count);

        /// <summary>
        /// Write bytes to SerialPort (Once).
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        void Write(byte[] bytes);

        /// <summary>
        /// Begin read bytes form SerialPort (Continued).
        /// </summary>
        /// <returns></returns>
        void BeginRead();

        /// <summary>
        /// End read bytes form SerialPort.
        /// </summary>
        /// <returns></returns>
        void EndRead();

        /// <summary>
        /// Begin write bytes to SerialPort (Continued).
        /// </summary>
        /// <returns></returns>
        void BeginWrite();

        /// <summary>
        /// End write bytes to SerialPort.
        /// </summary>
        /// <returns></returns>
        void EndWrite();
    }
}