// Placed in the public domain by Earle F. Philhower, III, 2022
#include "rpiboards.h"
#include <WiFi.h>
#include "rpiwatchdog.h"

#ifndef STASSID
#define STASSID "APQLZM"
#define STAPSK "silly1371"
#endif

const char* ssid = STASSID;
const char* password = STAPSK;

int port = 4242;

WiFiServer server(port);

void setup() {
  Serial.begin(115200);
  WiFi.mode(WIFI_STA);
  WiFi.setHostname("PicoW2");
  Serial.printf("Connecting to '%s' with '%s'\n", ssid, password);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    Serial.print(".");
    delay(100);
  }
  Serial.printf("\nConnected to WiFi\n\nConnect to server at %s:%d\n", WiFi.localIP().toString().c_str(), port);

  server.begin();
  watchdog_enable(WATCHDOG_SECS*1000, false);

}

void loop() {
  watchdog_update();
  static int i;
  delay(100);
  //Serial.printf("--loop %d\n", ++i);
  //delay(10);
  WiFiClient client = server.available();
  if (!client) {
    return;
  }
  Serial.println("WiFi Up");
  while(true)
  {
    watchdog_update();
    Serial.print("Get next command.");
    while (!client.available()) {
      delay(100);
      watchdog_update();
    }

    Serial.println("Connected.");
    while (client.available()) {
      watchdog_update();
      String req = client.readStringUntil('\n');
      if(req.length()==0)
      {
        Serial.println("Null cmd.");
        return;
      }
      Serial.print("Command:");
      Serial.println(req[0],HEX);
      switch(req[0])
      {
        // Escape simple string commands here. 
        // These commands sto start in uppercase
        // Need first letter of these ASCII values to not match commands
        // ie Softata commands not to be between 65 to 90, 0x41 to 0x5A
        case 'B':  // Begin
          Serial.println("Ready.");
          client.print("Ready"); // Sent at first connection.
          break;
        case 'E': //End
          Serial.println("Done.");
          client.print("Done"); 
          return;
          break;
        case 'N': //Null
          Serial.println("Null.");
          client.print("OK"); 
          return;
          break;
        default:
          // Get Softata command and parameters
          byte cmd = req[0];
          byte pin = 0xff;
          byte param = 0xff;
          byte other = 0xff;
          if(req.length()>1)
          {
            pin=req[1];
            if(req.length()>2)
            {
              param=req[2];
              if(req.length()>3)
              {
                other=req[3];
          } } }

          //Print commd and paramaters
          String str;
          byte vaue;
          Serial.print("cmd:");
          Serial.print(cmd);Serial.print(' ');
          if (pin!=0xff)
          {
            Serial.print("pin:");
            Serial.print(pin,HEX);Serial.print(' ');
            if (param!=0xff)
            {
              Serial.print("param:");
              Serial.print(param);Serial.print(' ');
              if (other!= 0xff)
              {
                Serial.print("other:");
                Serial.print(other);
          } } }
          Serial.println();

          // Action cmds
          int value;
          switch(cmd)
          {
            case 0xD0:
            case 0xD1:
            case 0xD2:
            case 0xD3:
            { 
              // Digital
              if(!IS_PIN_DIGITAL(pin))
              {
                Serial.print("Pin is not digital");
                client.print("NOK");
                continue;
              }
              switch (cmd)
              {
                case 0xD0:
                  Serial.print("pinMode:");
                  Serial.println(param);
                  pinMode(pin, (PinMode)param);
                  client.print("OK");
                  break;
                case 0xD1:
                  if(!IS_PIN_DIGITAL(pin))
                  Serial.println("digitalRead");
                  value = digitalRead(pin);
                  if(value)
                    client.print("ON");
                  else
                    client.print("OFF");
                  break;
                case 0xD2:
                  Serial.print("digitalWrite:");
                  Serial.println(param);
                  if(param)
                    digitalWrite(pin, HIGH); 
                  else
                    digitalWrite(pin, LOW);
                  client.print("OK");
                  break; 
                case 0xD3:
                  Serial.print("digitalToggle:");
                  value = digitalRead(pin);
                  if(value)
                    digitalWrite(pin, LOW); 
                  else
                    digitalWrite(pin, HIGH);
                  client.print("OK");
                  break;                   
                default:
                  Serial.println("Unknown digital cmd");
                  client.print("Unknown digital cmd"); 
                  break;              
              }
              break;           
            }
            case 0xA0:
            case 0xA1:
            case 0xA2: // Analog place holder
              if(!IS_PIN_ANALOG(pin))
              {
                Serial.print("Pin not Analog");
                client.print("NOK");
                continue;
              }
              Serial.println("Analog 2D cmds");
              client.print("Analog 2D cmds"); 
              break;
            case 0xB0:
            case 0xB1:
            case 0xB2: // PWM place holder
              if(!IS_PIN_PWM(pin))
              {
                Serial.print("Pin not PWM");
                client.print("NOK");
                continue;
              }
              Serial.println("PWD 2D cmds");
              client.print("PWD 2D cmds"); 
              break;
            case 0xC0:
            case 0xC1:
            case 0xC2: // Servo place holder
              if(!IS_PIN_SERVO(pin))
              {
                Serial.print("Pin not Servo");
                client.print("NOK");
                continue;
              }
              Serial.println("SERVO 2D cmds");
              client.print("SERVO 2D cmds"); 
              break;
            case 0xE0:
            case 0xE1:
            case 0xE2: // Serial place holder
              if(!IS_PIN_SERIAL(pin))
              {
                Serial.print("Pin not Serial");
                client.print("NOK");
                continue;
              }
              Serial.println("Serial 2D cmds");
              client.print("Serial 2D cmds"); 
              break;
            case 0xF0:
            case 0xF1:
            case 0xF2: // I2C place holder
              if(!IS_PIN_I2C(pin))
              {
                Serial.print("Pin not I2C");
                client.print("NOK");
                continue;
              }
              Serial.println("I2C 2D cmds");
              client.print("I2C 2D cmds"); 
              break;
            case 0xF3:
            case 0xF4:
            case 0xF5: // SPI place holder
              if(!IS_PIN_SPI(pin))
              {
                Serial.print("Pin not SPI");
                client.print("NOK");
                continue;
              }
              Serial.println("SPI 2D cmds");
              client.print("SPI 2D cmds"); 
              break;
            default:
              Serial.println("Unknown cmd");
              client.print("Unknown cmd"); 
              break; 
          }
          break;            
      }
      delay(500);
    }
  }

  client.printf("Done from Pico-W\r\n");
  client.flush();
}
