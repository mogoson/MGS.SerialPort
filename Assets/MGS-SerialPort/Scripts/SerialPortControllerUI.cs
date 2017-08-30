/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson tech. Co., Ltd.
 *  FileName: SerialPortControllerUI.cs
 *  Author: Mogoson   Version: 1.0   Date: 4/5/2017
 *  Version Description:
 *    Internal develop version,mainly to achieve its function.
 *  File Description:
 *    Ignore.
 *  Class List:
 *    <ID>           <name>             <description>
 *     1.    SerialPortControllerUI        Ignore.
 *  Function List:
 *    <class ID>     <name>             <description>
 *     1.
 *  History:
 *    <ID>    <author>      <time>      <version>      <description>
 *     1.     Mogoson     4/5/2017       1.0        Build this file.
 *************************************************************************/

namespace Developer.SerialPort
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;

    [RequireComponent(typeof(SerialPortController))]
    [AddComponentMenu("Developer/SerialPort/SerialPortControllerUI")]
    public class SerialPortControllerUI : MonoBehaviour
    {
        #region Property and Field
        public float xOffset = 10;
        public float yOffset = 10;
        private string readText = string.Empty;
        private string writeText = string.Empty;
        private Vector2 scrollPosition;
        private SerialPortController controller;
        private char[] separater = { '\x0020' };
        #endregion

        #region Private Method
        void Start()
        {
            controller = GetComponent<SerialPortController>();
        }//Start()_end

        void Update()
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
        }//Update()_end

        void OnGUI()
        {
            var rect = new Rect(xOffset, yOffset, 180, 250);
            GUILayout.BeginArea(rect, "Manager", "Window");

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Open"))
            {
                string error;
                if (controller.OpenSerialPort(out error))
                    Debug.Log("Open Succeed.");
            }
            if (GUILayout.Button("Close"))
            {
                string error;
                if (controller.CloseSerialPort(out error))
                    Debug.Log("Close Succeed.");
            }
            GUILayout.EndHorizontal();

            writeText = GUILayout.TextArea(writeText, GUILayout.Height(50));
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StartWrite"))
            {
                string error;
                if (controller.StartWrite(out error))
                    Debug.Log("Start Write Succeed.");
            }
            if (GUILayout.Button("Stop"))
            {
                string error;
                if (controller.StopWrite(out error))
                    Debug.Log("Stop Write Succeed.");
            }
            if (GUILayout.Button("Clear"))
                writeText = string.Empty;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("StartRead"))
            {
                string error;
                if (controller.StartRead(out error))
                    Debug.Log("Start Read Succeed.");
            }
            if (GUILayout.Button("Stop"))
            {
                string error;
                if (controller.StopRead(out error))
                    Debug.Log("Stop Read Succeed.");
            }
            if (GUILayout.Button("Clear"))
                readText = string.Empty;
            GUILayout.EndHorizontal();
            GUILayout.TextArea(readText, GUILayout.ExpandHeight(true));
            GUILayout.EndArea();
        }//OnGUI()_end
        #endregion
    }//class_end
}//namespace_end