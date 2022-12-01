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

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace MGS.IO.Ports.Demo
{
    public class SerialPorterHUD : MonoBehaviour
    {
        #region Field and Property
        public float top = 10;
        public float left = 10;

        private string readText = string.Empty;
        private string writeText = string.Empty;
        private string errorText = string.Empty;

        private const string SPACE = "\x0020";
        private readonly string[] SEPARATER = { SPACE };

        private ISerialPorter Handler { get { return SerialPortAPI.SerialPorter; } }
        #endregion

        #region Private Method
        private void Start()
        {
            Handler.OnError += (e) => errorText = e.Message;
        }

        private void Update()
        {
            if (Handler.IsReading)
            {
                var readString = string.Empty;
                foreach (var @byte in Handler.ReadBytes)
                {
                    readString += @byte.ToString("X2") + SPACE;
                }
                readText = readString;
            }
            if (Handler.IsWriting)
            {
                var writeBuffer = new List<byte>();
                var bytesString = writeText.Split(SEPARATER, StringSplitOptions.RemoveEmptyEntries);
                foreach (var @byte in bytesString)
                {
                    try
                    {
                        writeBuffer.Add(byte.Parse(@byte.Trim(), NumberStyles.HexNumber));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError(ex.Message);
                    }
                }
                Handler.WriteBytes = writeBuffer.ToArray();
            }
        }

        private void OnGUI()
        {
            var rect = new Rect(left, top, 180, 250);
            GUILayout.BeginArea(rect, "Controller", "Window");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("ReInitialize"))
            {
                if (Handler.IsOpen)
                {
                    Handler.Close();
                }
                SerialPortAPI.RebuildPorter();
            }
            if (Handler.IsOpen)
            {
                if (GUILayout.Button("Close"))
                {
                    Handler.Close();
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    Handler.Open();
                }
            }
            GUILayout.EndHorizontal();

            writeText = GUILayout.TextArea(writeText, GUILayout.Height(50));

            GUILayout.BeginHorizontal();
            if (Handler.IsWriting)
            {
                if (GUILayout.Button("EndWrite"))
                {
                    Handler.EndWrite();
                }
            }
            else
            {
                if (GUILayout.Button("BeginWrite"))
                {
                    Handler.BeginWrite();
                }
            }
            if (GUILayout.Button("Clear"))
            {
                writeText = string.Empty;
                errorText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (Handler.IsReading)
            {
                if (GUILayout.Button("EndRead"))
                {
                    Handler.EndRead();
                }
            }
            else
            {
                if (GUILayout.Button("BeginRead"))
                {
                    Handler.BeginRead();
                }
            }
            if (GUILayout.Button("Clear"))
            {
                readText = string.Empty;
                errorText = string.Empty;
            }
            GUILayout.EndHorizontal();

            GUILayout.TextArea(readText, GUILayout.ExpandHeight(true));
            GUILayout.TextArea(errorText, GUILayout.Height(40));
            GUILayout.EndArea();
        }

        private void OnApplicationQuit()
        {
            Handler.Close();
        }
        #endregion
    }
}