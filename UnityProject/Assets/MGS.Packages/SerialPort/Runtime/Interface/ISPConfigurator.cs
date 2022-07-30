/*************************************************************************
 *  Copyright © 2022 Mogoson. All rights reserved.
 *------------------------------------------------------------------------
 *  File         :  ISPConfigurator.cs
 *  Description  :  Interface of SerialPort configurator.
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
    /// Interface of SerialPort configurator.
    /// </summary>
    public interface ISPConfigurator : IDisposable
    {
        /// <summary>
        /// Config read from file.
        /// </summary>
        SPConfig Config { get; }

        /// <summary>
        /// Read config from file.
        /// </summary>
        /// <returns>Config read from file. (You can handle the result or get it from the property Config)</returns>
        SPConfig ReadCfg();

        /// <summary>
        /// Write config from file.
        /// </summary>
        /// <returns></returns>
        bool WriteCfg();
    }
}