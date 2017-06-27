==========================================================================
  Copyright (C), 2017-2018, Mogoson tech. Co., Ltd.
  Name: MGS-SerialPort
  Author: Mogoson   Version: 1.0   Date: 4/5/2017
==========================================================================
  [Summeray]
    This package can be used to communicate with serialport in Unity3D.
--------------------------------------------------------------------------
  [Environment]
    Package applies to Unity3D 5.0, .Net Framework 2.0 or above version.
    If namespace error, set the "Api Compatibility Level" as ".NET 2.0".
    Find in the path "Build Settings -> Player Settings -> Other Settings".
--------------------------------------------------------------------------
  [Usage]
    Find the demos in the path "MGS-SerialPort/Scenes".
    Play the scene named "SerialPort" and click the "Commit" button to
    create a config file(default name is SerialPortConfig.txt) in Asset
    folder[Refresh it to show config file] to test.
    Test serialport communicate with virtual serial port or hardware.
    Understand the usages of component scripts in the demos.
    Use the compnent scripts in your project.
    After building exe pack(example Test.exe), copy the config file to
    the data folder(example Test_Data).
--------------------------------------------------------------------------
  [Config]
    ReadBufferSize should be try to set as a small value to reduce memory
    spending, example 512.
    ReadCount not include ReadHead and ReadTail, example, the ReadCount
    of a bytes frame "FE 00 01 00 01 00 01 00 01 00 01 FF" is 10.
    WriteCount is the same truth as ReadCount.
--------------------------------------------------------------------------
  [Test]
    Use follows data to test serialport communicate whith default config.
    If this plugin, write bytes: "00 01 00 01 00 01 00 01 00 01".
    If other tool, write bytes: "FE 00 01 00 01 00 01 00 01 00 01 FF".
--------------------------------------------------------------------------
  [Contact]
    If you have any questions, feel free to contact me at mogoson@qq.com.
--------------------------------------------------------------------------