/*************************************************************************
 *  Copyright (C), 2017-2018, Mogoson Tech. Co., Ltd.
 *------------------------------------------------------------------------
 *  File         :  SerialPortManager.cs
 *  Description  :  Manage SerialPortController.
 *------------------------------------------------------------------------
 *  Author       :  Mogoson
 *  Version      :  0.1.0
 *  Date         :  4/5/2017
 *  Description  :  Initial development version.
 *************************************************************************/

using UnityEngine;

namespace Developer.IO.Ports
{
    public static class SerialPortManager
    {
        #region Property and Field
        /// <summary>
        /// Instance of SerialPortController.
        /// </summary>
        public static SerialPortController Instance
        {
            get
            {
                if (instance == null)
                {
                    var controller = new GameObject(instanceName);
                    instance = controller.AddComponent<SerialPortController>();
                    Object.DontDestroyOnLoad(controller);
                }
                return instance;
            }
        }

        /// <summary>
        /// Instance of SerialPortController.
        /// </summary>
        private static SerialPortController instance;

        /// <summary>
        /// Name of instance.
        /// </summary>
        private const string instanceName = "SerialPortController";
        #endregion

        #region Public Method
        /// <summary>
        /// Destroy instance of SerialPortController.
        /// </summary>
        public static void DestroyInstance()
        {
            if (instance)
            {
                Object.Destroy(instance.gameObject);
                instance = null;
            }
        }
        #endregion
    }
}