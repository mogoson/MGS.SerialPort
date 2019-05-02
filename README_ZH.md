# MGS-SerialPort
- [English Manual](./README.md)

## 概述 
- Unity串口通信。

## 需求
- 串口数据同步读取，持续写入；串口参数通过本地文件配置（便于安装调试和后续维护）。

## 环境
- Unity 5.0 或更高版本（如果是Unity 5.3 或者更高版本，Json插件LitJson.dll可以删除）。
- .Net Framework 2.0（项目打包设置“Build Settings -> Player Settings -> Other Settings”
  ->“Api Compatibility Level” 项设置为 ".NET 2.0"）。

## 问题
- Unity目前不能正常在Update，FixedUpdate等事件函数中直接读取串口数据，运行程序卡顿。
- Unity目前没有实现“SerialPort.ReceivedBytesThreshold”属性设置，抛出没有实现异常。
- Unity目前不能正常触发“SerialPort.DataReceived”事件，无异常抛出，无响应。
- Unity目前不能正常读取“SerialPort.BytesToRead”属性，运行程序卡死。
- Unity目前不能有效执行“SerialPort.DiscardInBuffer”方法，无异常抛出，输入缓冲区没能清空。
- Unity目前不能有效执行“SerialPort.DiscardOutBuffer”方法，无异常抛出，输出缓冲区没能清空。
- 上位机与下位机收发周期不一致（除特殊情况外可协调一致），每次从缓冲区读取到的数据基本
  不是一个完整的数据帧（单次接收/发送的数据），接收周期小于发送周期时，每次读取到的字
  节数小于1个数据帧长度；接收周期大于发送周期时，每次读取到的字节数大于1个数据帧长度。
- 即使上位机与下位机收发周期一致，因收发时刻差异，每次从缓冲区读取到的数据并非都是一个
  完整的数据帧，数据或长或短。

## 方案
### 参数配置
- 串口参数写入到本地文件，便于调试，维护时修改适配。

### 持续读写
- 使用线程按照读取周期持续从串口读取数据。
- 使用线程按照写入周期持续向串口写入数据。

### 数据同步
#### 协议
-  商定收发周期尽量一致，不一致也无妨。
-  商定收发协议，数据帧长度固定，数据帧的第一个字节作为收发头部标记（ReadHead/WiteHead），
   数据帧的最后一个字节作为收发尾部标记（ReadTail/WiteTail），剩余字节数作为收发数据计
   数（ReadCount/WriteCount）。

#### 读取
1. 读取整个缓冲区Buffer，并获取读取到的字节数Count；事实上，如果收发周期基本一致，那么
   单次可以只从缓冲区读取1~2个数据帧长度（ReadCount+2）的字节以节约内存开销，读者可根
   据自己的需求自行修改。

2. 如果Count不超过2个数据帧则数据没有延迟，直接将读取到的字节数据(Buffer的前Count个字节)
   添加到FrameBuffer列表；否则数据有延迟冗余，将读取到的字节数据(Buffer的前Count个字节个
   字节)的后2个数据帧长度的字节数据添加到FrameBuffer列表；因为正常情况下，2个数据帧长度
   的连续字节数据必然包含一个完整的数据帧。继续。

3. 检查FrameBuffer的长度，如果小于1个数据帧长度则返回步骤1；如果大于或等于1个数据帧长度
   （有可能包含一个完整的数据帧），继续。

4. 检查FrameBuffer，如果第一个字节不等于ReadHead标记，则删除这个无效字节，返回步骤3；如
   果第一个字节等于ReadHead标记，则检查1个数据帧长度位置的字节，如果等于ReadTail标记则
   找到一个完整的数据帧，将FrameBuffer前1个数据帧长度的字节存入ReadBytes，至此，缓冲区
   中由串口发来的最近一个完整数据帧已读取到；否则数据无效；现在FrameBuffer的前1个数据帧
   长度的字节已经检查过，将其从FrameBuffer中删除，返回步骤3。

#### 写入
- WriteBytes前面添加WiteHead标记，后面添加WriteTail标记，将其写入串口。

## 案例
- “MGS-SerialPort\Scenes”目录下存有上述功能的演示案例，供读者参考。

## 预览
- Serialport

![Serialport](./Attachment/README_Image/Serialport.png)

## 联系
- 如果你有任何问题或者建议，欢迎通过mogoson@outlook.com联系我。