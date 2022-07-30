# MGS.SerialPort

## Summary
- Unity communicate with serialport.

## Demand
- Serialport asynchronous read and write data; 
- parameters of serialport config in local file.

## Environment
- Unity 5.3 or above.

- .Net Framework 2.0(Set the "Api Compatibility Level" as ".NET 2.0". Find in the path
  "Build Settings -> Player Settings -> Other Settings").

## Background
- Current Unity can not read serialport data in "Update" or "FixedUpdate" method.
- Current Unity not implemented the "SerialPort.ReceivedBytesThreshold" property.
- Current Unity can not callback the "SerialPort.DataReceived" event.
- Current Unity can not read the "SerialPort.BytesToRead" property.
- Current Unity can not execute the "SerialPort.DiscardInBuffer" method effectively.
- Current Unity can not execute the "SerialPort.DiscardOutBuffer" method effectively.

## Design

- Config SerialPort parameters in local file.
- Thread to read and write data buffer to SerialPort continued.
- Mark data Head and Tail to recognize data frame.

------

Copyright © 2022 Mogoson.	mogoson@outlook.com