/****************************************************************************************************************************
  Project Shmelev iRemote 1.0
  ESP8266-Sketch.ino
  For ESP8266

  Licensed under MIT license

  Originally Created on: 23.03.2021
  Original Author: Dmitrii Shmelev
*****************************************************************************************************************************/

#if !defined(ESP8266)
#error This code is intended to run only on the ESP8266 boards ! Please check your Tools->Board setting.
#endif

#define _WEBSOCKETS_LOGLEVEL_     3
#define Serial Serial

#include <ESP8266WiFi.h>
#include <ESP8266WiFiMulti.h>
#include <WebSocketsServer_Generic.h>
#include <IRremoteESP8266.h>
#include <IRrecv.h>
#include <IRutils.h>
#include <Hash.h>

const uint16_t kRecvPin = D3; // ------------------ DataPIN for IRDA Module
IRrecv irrecv(kRecvPin);
decode_results results;
ESP8266WiFiMulti WiFiMulti;
WebSocketsServer webSocket = WebSocketsServer(8080); // ------------- WebSocket Listen port


void webSocketEvent(uint8_t num, WStype_t type, uint8_t * payload, size_t length)
{
  switch (type)
  {
    case WStype_DISCONNECTED:
      Serial.printf("[%u] Disconnected!\n", num);
      break;

    case WStype_CONNECTED:
      {
        IPAddress ip = webSocket.remoteIP(num);
        Serial.printf("[%u] Connected from %d.%d.%d.%d url: %s\n", num, ip[0], ip[1], ip[2], ip[3], payload);
        webSocket.sendTXT(num, "{\"status\":\"connected\"}");
      }
      break;

    case WStype_TEXT:
      Serial.printf("[%u] get Text: %s\n", num, payload);
      handle_req(payload);
      break;

    case WStype_BIN:
      Serial.printf("[%u] get binary length: %u\n", num, length);
      hexdump(payload, length);
      break;

    default:
      break;
  }

}

void handle_req(uint8_t * payload)
{


}


void setup()
{
  pinMode(LED_BUILTIN, OUTPUT);
  digitalWrite(LED_BUILTIN, HIGH);
  Serial.begin(115200);
  irrecv.enableIRIn();
  Serial.println("\nStart ESP8266_WebSocketServer on " + String(ARDUINO_BOARD));
  Serial.println("Version " + String(WEBSOCKETS_GENERIC_VERSION));
  
  WiFiMulti.addAP("SSID", "PASSWORD"); //------------------------------------------------------- YOUR SSID AND PASSWORD

  while (WiFiMulti.run() != WL_CONNECTED)
  {
    Serial.print(".");
    delay(100);
  }

  Serial.println();
  Serial.print("WebSockets Server started @ IP Address: ");
  Serial.println(WiFi.localIP());

  webSocket.begin();
  webSocket.onEvent(webSocketEvent);

}

String last_recieve = "";

void loop()
{
  webSocket.loop();
  if (irrecv.decode(&results)) {
    digitalWrite(LED_BUILTIN, LOW);
    serialPrintUint64(results.value, HEX);
    String res = String((int)results.value, HEX);
    String response = "";

    if (res == "ffffffff")
    {
      response = last_recieve;
    }
    else
    {
      last_recieve = res;
      response = res;
    }
    response = "{\"ir_code\":\"" + response + "\"}";
    webSocket.broadcastTXT(response);
    digitalWrite(LED_BUILTIN, HIGH);
    irrecv.resume();
  }
}
