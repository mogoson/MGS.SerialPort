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
        private static SerialPortController controller;

        /// <summary>
        /// Name of SerialPortController gameobject.
        /// </summary>
        private const string controllerObjectName = "SerialPortController";
        #endregion

        #region Public Method
        /// <summary>
        /// Get Instance of SerialPortController.
        /// </summary>
        public static SerialPortController GetController()
        {
            if (controller == null)
            {
                var controllerObject = new GameObject(controllerObjectName);
                controller = controllerObject.AddComponent<SerialPortController>();
                Object.DontDestroyOnLoad(controllerObject);
            }
            return controller;
        }

        /// <summary>
        /// Destroy instance of SerialPortController.
        /// </summary>
        public static void DestroyController()
        {
            if (controller)
            {
                Object.Destroy(controller.gameObject);
                controller = null;
            }
        }
        #endregion
    }
}