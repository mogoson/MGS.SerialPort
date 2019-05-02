/*************************************************************************
 *  Copyright © 2017-2018 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  SerialPortControllerHUD.cs
 *  Description  :  Draw UI in scene to control serialport.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *  
 *  Author       :  Mogoson
 *  Version      :  0.1.1
 *  Date         :  3/2/2018
 *  Description  :  Optimize.
 *************************************************************************/

using MGS.Common.Logger;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MGS.IO.Ports
{
    [AddComponentMenu("MGS/IO/Ports/SerialPortControllerHUD")]
    public class SerialPortControllerHUD : MonoBehaviour
    {
        #region Field and Property
        public float top = 10;
        public float left = 10;

        private string readText = string.Empty;
        private string writeText = string.Empty;

        private const string SPACE = "\x0020";
        private readonly string[] SEPARATER = { SPACE };

        private SerialPortController Controller { get { return SerialPortController.Instance; } }
        #endregion

        #region Private Method
        private void Update()
        {
            if (Controller.IsReading)
            {
                if (Controller.IsReadTimeout)
                {
                    readText = "No serialport device input.";
                }
                else
                {
                    var readString = string.Empty;
                    foreach (var @byte in Controller.ReadBytes)
                    {
                        readString += @byte.ToString("X2") + SPACE;
                    }
                    readText = readString;
                }
            }
            if (Controller.IsWriting)
            {
                var writeBuffer = new List<byte>();
                var bytesString = writeText.Split(SEPARATER, StringSplitOptions.RemoveEmptyEntries);
                foreach (var @byte in bytesString)
                {
                    try
                    {
                        writeBuffer.Add(byte.Parse(@byte.Trim(), NumberStyles.HexNumber));
                    }
                    catch (Exception e)
                    {
                        LogUtility.LogError(e.Message);
                    }
                }
                Controller.WriteBytes = writeBuffer.ToArray();
            }
        }

        private void OnGUI()
        {
            var rect = new Rect(left, top, 180, 220);
            GUILayout.BeginArea(rect, "Controller", "Window");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ReInitialize"))
            {
                if (Controller.IsOpen)
                {
                    Controller.CloseSerialPort();
                }
                Controller.InitializeSerialPort();
            }
            if (Controller.IsOpen)
            {
                if (GUILayout.Button("Close"))
                {
                    Controller.CloseSerialPort();
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    Controller.OpenSerialPort();
                }
            }
            GUILayout.EndHorizontal();

            writeText = GUILayout.TextArea(writeText, GUILayout.Height(50));

            GUILayout.BeginHorizontal();
            if (Controller.IsWriting)
            {
                if (GUILayout.Button("StopWrite"))
                {
                    Controller.StopWrite();
                }
            }
            else
            {
                if (GUILayout.Button("StartWrite"))
                {
                    Controller.StartWrite();
                }
            }
            if (GUILayout.Button("Clear"))
            {
                writeText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (Controller.IsReading)
            {
                if (GUILayout.Button("StopRead"))
                {
                    Controller.StopRead();
                }
            }
            else
            {
                if (GUILayout.Button("StartRead"))
                {
                    Controller.StartRead();
                }
            }
            if (GUILayout.Button("Clear"))
            {
                readText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.TextArea(readText, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }

        private void OnApplicationQuit()
        {
            if (Controller.IsOpen)
            {
                Controller.CloseSerialPort();
            }
        }
        #endregion
    }
}