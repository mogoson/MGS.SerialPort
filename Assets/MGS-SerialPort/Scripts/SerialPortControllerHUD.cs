/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *------------------------------------------------------------------------
 *  File         :  SerialPortControllerHUD.cs
 *  Description  :  Draw UI in scene to control serialport.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Developer.IO.Ports
{
    [RequireComponent(typeof(SerialPortController))]
    [AddComponentMenu("Developer/IO/Ports/SerialPortControllerHUD")]
    public class SerialPortControllerHUD : MonoBehaviour
    {
        #region Property and Field
        public float xOffset = 10;
        public float yOffset = 10;

        private SerialPortController controller;
        private string readText = string.Empty;
        private string writeText = string.Empty;
        private char[] separater = { '\x0020' };
        #endregion

        #region Private Method
        private void Start()
        {
            controller = GetComponent<SerialPortController>();
        }

        private void Update()
        {
            if (controller.isReading)
            {
                var readString = string.Empty;
                foreach (var @byte in controller.readBytes)
                {
                    readString += @byte.ToString("X2") + "\x0020";
                }
                readText = readString;
            }
            if (controller.isWriting)
            {
                var writeBuffer = new List<byte>();
                var bytesString = writeText.Split(separater, StringSplitOptions.RemoveEmptyEntries);
                foreach (var @byte in bytesString)
                {
                    try
                    {
                        writeBuffer.Add(byte.Parse(@byte.Trim(), NumberStyles.HexNumber));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
                controller.writeBytes = writeBuffer.ToArray();
            }
        }

        private void OnGUI()
        {
            var rect = new Rect(xOffset, yOffset, 180, 220);
            GUILayout.BeginArea(rect, "Controller", "Window");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Initialise"))
            {
                string error;
                if (controller.InitialiseSerialPort(out error))
                    Debug.Log("Initialise Succeed.");
                else
                    Debug.LogWarning("Initialise with default config. error : " + error);
            }
            if (controller.isOpen)
            {
                if (GUILayout.Button("Close"))
                {
                    string error;
                    if (controller.CloseSerialPort(out error))
                        Debug.Log("Close Succeed.");
                }
            }
            else
            {
                if (GUILayout.Button("Open"))
                {
                    string error;
                    if (controller.OpenSerialPort(out error))
                        Debug.Log("Open Succeed.");
                }
            }
            GUILayout.EndHorizontal();

            writeText = GUILayout.TextArea(writeText, GUILayout.Height(50));

            GUILayout.BeginHorizontal();
            if (controller.isWriting)
            {
                if (GUILayout.Button("StopWrite"))
                {
                    string error;
                    if (controller.StopWrite(out error))
                        Debug.Log("Stop Write Succeed.");
                }
            }
            else
            {
                if (GUILayout.Button("StartWrite"))
                {
                    string error;
                    if (controller.StartWrite(out error))
                        Debug.Log("Start Write Succeed.");
                }
            }
            if (GUILayout.Button("Clear"))
                writeText = string.Empty;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (controller.isReading)
            {
                if (GUILayout.Button("StopRead"))
                {
                    string error;
                    if (controller.StopRead(out error))
                        Debug.Log("Stop Read Succeed.");
                }
            }
            else
            {
                if (GUILayout.Button("StartRead"))
                {
                    string error;
                    if (controller.StartRead(out error))
                        Debug.Log("Start Read Succeed.");
                }
            }
            if (GUILayout.Button("Clear"))
                readText = string.Empty;
            GUILayout.EndHorizontal();

            GUILayout.TextArea(readText, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }
        #endregion
    }
}