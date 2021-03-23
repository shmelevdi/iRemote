# iRemote

iRemote is a project that allows you to control a Windows computer using an IR remote control.

So far, unfortunately, a separate class library is not ready for convenient use of the project, but this can really be used.

For all questions write to Telegram: https://t.me/shmelevdi

### .NET Project

On board .NET Framework 4.5

The .NET part acts as a receiver of commands from the ESP8266.

#### Dependencies

WebSocketSharp | (https://www.nuget.org/packages?q=WebSocketSharp+)

Newtonsoft.Json | (https://www.nuget.org/packages/Newtonsoft.Json)


### ESP8266 Project

I'm using NodeMCU 1.0 v3 with ESP8266 module in board and TL1838 IR-sensor.

The IR sensor connection diagram is shown below

![Connection diagram](https://github.com/shmelevdi/iRemote/blob/main/ESP8266%20-%20Arduino%20IDE%20Project/nodemcu.PNG)


