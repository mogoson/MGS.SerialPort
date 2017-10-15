/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *------------------------------------------------------------------------
 *  File         :  SerialPortConfigurerHUD.cs
 *  Description  :  Draw UI in scene to config serialport parameters.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/4/2017
 *  Description  :  Initial development version.
 *************************************************************************/

using System.IO.Ports;
using UnityEngine;

namespace Developer.IO.Ports
{
    [RequireComponent(typeof(SerialPortConfigurer))]
    [AddComponentMenu("Developer/IO/Ports/SerialPortConfigurerHUD")]
    public class SerialPortConfigurerHUD : MonoBehaviour
    {
        #region Property and Field
        public float xOffset = 10;
        public float yOffset = 10;

        private SerialPortConfigurer configurer;
        private SerialPortConfig config;
        #endregion

        #region Private Method
        private void Start()
        {
            string error;
            configurer = GetComponent<SerialPortConfigurer>();
            configurer.ReadConfig(out config, out error);
        }

        private void OnGUI()
        {
            var rect = new Rect(xOffset, yOffset, 180, 180);
            GUILayout.BeginArea(rect, "Configurer", "Window");

            GUILayout.BeginHorizontal();
            GUILayout.Label("PortName");
            config.portName = GUILayout.TextField(config.portName, GUILayout.Width(60));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("BaudRate");
            config.baudRate = int.Parse(GUILayout.TextArea(config.baudRate.ToString(), GUILayout.Width(60)));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Parity");
            config.parity = (Parity)int.Parse(GUILayout.TextArea(((int)config.parity).ToString(), GUILayout.Width(20)));
            GUILayout.Label(config.parity.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("DataBits");
            config.dataBits = int.Parse(GUILayout.TextArea(config.dataBits.ToString(), GUILayout.Width(60)));
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("StopBits");
            config.stopBits = (StopBits)int.Parse(GUILayout.TextArea(((int)config.stopBits).ToString(), GUILayout.Width(20)));
            GUILayout.Label(config.stopBits.ToString());
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Apply"))
            {
                string error;
                configurer.WriteConfig(config, out error);
            }
            GUILayout.EndArea();
        }
        #endregion
    }
}