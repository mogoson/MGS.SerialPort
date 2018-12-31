==========================================================================
  Copyright © 2017-2018 Mogoson. All rights reserved.
  Name: MGS-SerialPort
  Author: Mogoson   Version: 0.2.0   Date: 2/14/2018
==========================================================================
  [Summary]
    Unity communicate with serialport.
--------------------------------------------------------------------------
  [Demand]
    Serialport synchronous read and write data; parameters of serialport
    config in local file.
--------------------------------------------------------------------------
  [Environment]
    Unity 5.0 or above.
    .Net Framework 2.0.

    If Unity 5.3 or higher version, the Json plugin(LitJson.dll) can be
	delete.

    If namespace error, set the "Api Compatibility Level" as ".NET 2.0".
    Find in the path "Build Settings -> Player Settings -> Other Settings".
--------------------------------------------------------------------------
  [Achieve]
    SerialPortConfig : Config of serialport parameters.

    SerialPortConfigurer : Read config from local file and write config
    to local file.

    SerialPortController : Synchronous read and write serialport data.
--------------------------------------------------------------------------
  [Usage]
    Find the prefab "SerialPortHUD" in the path "MGS-SerialPort\Prefabs"
    and add it to your scene.

    Play your scene, config the parameters of serialport in the HUD and
    test communicate with serialport.

    Delete the SerialPortHUD from your scene if you do not need it.

    Use the SerialPortController.Instance to get the instance of
    SerialPortController in your script to control SerialPort and read,
    write byte data.
--------------------------------------------------------------------------
  [Config]
    The config file in the path "StreamingAssets\Config\SerialPortConfig.json".

    ReadCount not include ReadHead and ReadTail, example, the ReadCount
    of a bytes frame "FE 00 01 00 01 00 01 00 01 00 01 FF" is 10.

    WriteCount is the same truth as ReadCount.
--------------------------------------------------------------------------
  [Test]
    Use follows data to test serialport communicate whith default config.

    If this plugin, write bytes: "00 01 00 01 00 01 00 01 00 01".

    If other tool, write bytes: "FE 00 01 00 01 00 01 00 01 00 01 FF".
--------------------------------------------------------------------------
  [Demo]
    Demos in the path "MGS-SerialPort\Scenes" provide reference to you.
--------------------------------------------------------------------------
  [Resource]
    https://github.com/mogoson/MGS-SerialPort.
--------------------------------------------------------------------------
  [Contact]
    If you have any questions, feel free to contact me at mogoson@outlook.com.
--------------------------------------------------------------------------