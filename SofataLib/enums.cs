﻿namespace Softata.Enums
{
    enum LCD1602MiscCmds : byte { home, autoscroll, noautoscroll, blink, noblink, LCD1602MiscCmds_MAX }
    enum NEOPIXELMiscCmds : byte { setpixelcolor, setpixelcolorAll, setpixelcolorOdds, setpixelcolorEvens, setBrightness, NEOPIXELMiscCmds_MAX }
    enum OLEDMiscCmds : byte { drawCircle, drawFrame, OLEDMiscCmds_MAX }
    public enum SensorDevice : byte { DHT11, BME280, UltrasonicRANGER, Undefined = 0xFF }
    public enum DisplayDevice : byte { OLED096, LCD1602, NEOPIXEL, Undefined = 0xFF }
    public enum ServoDevice : byte { Servo }

    public enum Commands
    {
        //DigitalButtonLED IO
        pinMode = 0xD0,
        digitalWrite = 0xD1,
        digitalRead = 0xD2,
        digitalToggle = 0xD3,

        //AnalogPotLED/PWM
        analogRead = 0xA2,
        pwmWrite = 0xB1,

        //Serial
        serialSetup = 0xE0, // Setup Serial1/2
        serialGetChar = 0xE1, // Get a char
        serialGetString = 0xE2, // Get a string
        serialGetStringUntil = 0xE3, // Get a string until char
        serialWriteChar = 0xE4, // Write a char
        serialGetFloat = 0xE5, // Get Flost
        serialGetInt = 0xE6, // Get Int
        serialReadLine = 0xE7,

        groveSensor = 0xF0,
        groveDisplay = 0xF1,
        groveActuator = 0xF2,

        Undefined = 0xFF
    }

    public enum CommandType : byte
    {
        DigitalButtonLED = 0,
        AnalogPotLED = 1,
        PWM = 2,
        Servo = 3,
        Sensors = 4,
        Displays = 0x5,
        Serial = 6,
        PotLightSoundAnalog = 0x7,
        USonicRange = 0x8,
        PotRelay = 0x9,
        PotServo = 0xA,
        MaxType = 0xB,

        Undefined = 0xFF
    }

    public enum DeviceCategory : byte
    {
        digital = 0,
        analog = 0x1,
        sensor = 0x2,
        actuator = 0x3,
        display = 0x4,
        MaxType = 0x5,
        //communication,
        Undefined = 0xFF
    }

    enum BAUDRATE : byte
    {
        bd50, bd75, bd110, bd134, bd150, bd200, bd300, bd600, bd1200, bd1800, bd2400, bd4800, bd9600, bd19200, bd38400, bd57600, bd115200
    };

    enum BAURATERanges : byte
    {
        up_to_300,
        from_600_to_4800,
        from_9600
    }

}
