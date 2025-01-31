﻿using Microsoft.VisualBasic;
using System.Collections.Concurrent;
using System.Data.Common;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;
using Softata.Enums;
using System.Collections.Generic;
using System.Linq;


namespace Softata
{
    public partial class SoftataLib
    {
        //Ref: https://datasheets.raspberrypi.com/pico/Pico-R3-A4-Pinout.pdf
        public const int PinMax = 28;

        public const byte nullData = 0xfe;

        public static  int port { get; set; } = 4242;
        public static string ipAddresStr { get; set; } = "192.168.0.9";

        private static Socket? _client;

        private static Socket? client
        {
            get
            {
                if (_client == null)
                {
                };
                return _client;
            }
            set => _client = value;
        }
        public static Socket? Client { get => client; set => client = value; }// { get => Client1; set => Client1 = value; }
        /*public static Socket? Client1 { get => client; set => client = value; }*/

        private static bool Inited = false;

        public static void Disconnect()
        {
 
            try
            {
                if (Client != null)
                {
                    if (Client.Connected)
                    {
                        Client.Close();
                    }
                    Client.Dispose();
                }
            }
            catch (Exception ex) { }
        }

        public static void Reconnect()
        {
            try
            {
                if (Client != null)
                {
                    if (Client.Connected)
                    {
                        Client.Close();
                    }
                    Client.Dispose();
                }
            }
            catch (Exception ex) { }

            Connect(ipAddresStr, port);
        }
        public  static bool Connect(string _ipAddresStr, int _port)
        {
            Console.WriteLine("Connecting to Softata Server from .NET");
            ipAddresStr = _ipAddresStr;
            port = _port;
            //IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAdress = IPAddress.Parse(ipAddresStr);

            IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, port);

            Console.WriteLine("Connecting to Softata Server.");
            Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                Client.Connect(ipEndpoint);
                while (!Client.Connected)
                {
                    Thread.Sleep(500);
                }

                Console.WriteLine("Socket created to {0}", Client.RemoteEndPoint?.ToString());
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (Client.Connected)
                {
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Close();
                }
                Client = null;
            }
            if (Client == null)
                return false;
            else
                return true;;
        }



        static SoftataLib()
        {

        }

        // Add commands here that use param = 0xff
        // For others 0xff is not sent
        private static List<Commands> WritingCmds = new List<Commands> { Commands.pwmWrite, Commands.serialWriteChar};




        // For simplicity add items to lists below using their ordinal:
        public static Dictionary<DeviceCategory, List<byte>> DeviceCategoryMembers = new Dictionary<DeviceCategory, List<byte>>()
        {
            { DeviceCategory.digital,new List<byte>(){0}},
            { DeviceCategory.analog,new List<byte>(){1,7,9,0xA }},
            { DeviceCategory.sensor,new List<byte>(){4} },
            { DeviceCategory.actuator,new List<byte>(){9,0xA}},
            { DeviceCategory.display,new List<byte>(){5}},
            { DeviceCategory.serial,new List<byte>(){6,0xB}}
        };

    //////////////////////////////////////////
    // NOTE enum order of DisplayDevice must match that returned by GroveDisplayCmds.getDisplays()
        //public enum DisplayDevice { OLEDSO096, LCD1602Display, NeopixelDisplay }
        //////////////////////////////////////////

        public enum PinMode
        {
            Undefined = -1,
            DigitalInput = 0,
            DigitalOutput = 1,
            AnalogInput = 2,
            PwmOutput = 3,
            ServoControl = 4,
            I2C = 6,
            OneWire = 7,
            StepperControl = 8,
            Encoder = 9,
            Serial = 10,
            InputPullup = 11
        }

        public enum PinState
        {
            LOW = 0x00,
            HIGH = 0x01
        }
        public static string SendMessageCmd(string cmd)
        {
            if (Client == null)
                throw new Exception("SendMessageCmd: Client is null");

            else if (!Client.Connected)
                throw new Exception("SendMessageCmd: Not connected");

            string result = "";


                // Construct command and parameters as list of bytes
                List<byte> sendmsg = new List<byte> { 1, (byte)cmd[0] };

            byte[] sendBytes = sendmsg.ToArray<byte>();
            Console.WriteLine($"Sending {sendBytes.Length} data bytes");
            int sent = Client.Send(sendBytes, 0, sendBytes.Length, SocketFlags.None);
            if (sent != sendBytes.Length)
                throw new Exception($"SendMessage: Sent {sent} bytes, expected {sendBytes.Length} bytes");
            Console.WriteLine($"Sent {sent} bytes");

            //Wait for response
            while (Client.Available == 0) ;
            byte[] data = new byte[100];
            int recvd = Client.Receive(data);

            result = Encoding.ASCII.GetString(data).Substring(0, recvd).Trim();
            Console.WriteLine($"Received {result} [{recvd}] bytes\n");

            switch (cmd)
            {
                case "Begin":
                    return result;
                    break;
                case "Version":
                    return result;
                    //break
                case "Devices":
                    return result;
                    //break
                case "Null":
                    break;
                case "End":
                    Thread.Sleep(2000);
                    Client.Shutdown(SocketShutdown.Both);
                    Client.Close();
                    Client.Dispose();
                    Client = null;
                    break;
                case "Reset":
                    break;
            }
            return "";
        }
        public static string SendMessage(Commands MsgType, byte pin = 0xff, byte state = 0xff, string expect = "OK", byte other=0xff, byte[]? Data=null, bool debug=true )
        {
            if (Client == null)
                throw new Exception("SendMessageCmd: Client is null");
            else if (!Client.Connected)
                throw new Exception("SendMessageCmd: Not connected");


            // Construct command and parameters as list of bytes
            List<byte> sendmsg = new List<byte> { 0,(byte)MsgType };
            if (pin != 0xff)
            {
                sendmsg.Add(pin);
                if ((state != 0xff) || (WritingCmds.Contains(MsgType)))
                {
                    sendmsg.Add(state);
                    if (other != 0xff)
                    {
                        sendmsg.Add(other);
                    }
                }
            }
            if (Data!= null)
            {
                foreach (int d in Data)
                {
                    sendmsg.Add((byte)d);
                }
            }
              
            sendmsg[0] = (byte)(sendmsg.Count-1);
            // Get bytes from list.
            byte[] sendBytes = sendmsg.ToArray<byte>();
            /* If msg is Display setup nondefault:
             * =======================================
             * msg should now be:
             * number of bytes -1 eg 6 (if total num bytes is7)
             * 241, 0xf1 = Displays                 ... maps to cmd in Arduino
             * pin  (eg 16)                         ... maps to pin in Arduino
             * subcommand = 3  for nondefault setup ... maps toparam in Arduino
             * enum index of display (eg 2 for Neopixel) ... maps to other in Arduino
             *     Nb: Once instatiated, other is its index in the display list as returned from setup() 
             * Length of otherData                  ... maps to otherData in Arduino
             * other data                           ...
             */
            if(debug)
                Console.WriteLine($"Sending {sendBytes.Length} data bytes");

            int sent = Client.Send(sendBytes, 0, sendBytes.Length, SocketFlags.None);
            if (sent != sendBytes.Length)
                throw new Exception($"SendMessage: Sent {sent} bytes, expected {sendBytes.Length} bytes");

            if(debug)
                Console.WriteLine($"Sent {sent} bytes");

            //Wait for response
            while (Client.Available == 0) ;
            byte[] data = new byte[256];
            int recvd = Client.Receive(data);

            string result = Encoding.UTF8.GetString(data).Substring(0, recvd);

            // Make the check work in both ways.
            if ((!expect.Contains(result)) && (!result.Contains(expect)))
            {
                Console.WriteLine($"Expected {expect} got {result}... rebooting");
                SendMessageCmd("Reset");
                recvd = Client.Receive(data);
                result = Encoding.ASCII.GetString(data).Substring(0, recvd).Trim();
                Console.WriteLine($"Received {result} [{recvd}] bytes\n");
                return "Reset";
            }
            if(debug)
                 Console.WriteLine($"Received {result} [{recvd}] bytes\n");
            return result.Replace(expect,"");
        }
        //static void Main(string[] args)
        //{
        //    byte[] data = new byte[100];
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        data[i] = (byte)0;
        //    }

        //    IPHostEntry iphostInfo = Dns.GetHostEntry(Dns.GetHostName());
        //    IPAddress ipAdress = IPAddress.Parse(ipAddresStr);

        //    IPEndPoint ipEndpoint = new IPEndPoint(ipAdress, port);

        //    Console.WriteLine("Hello, World!");
        //    Client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //    try
        //    {
        //        Client.Connect(ipEndpoint);
        //        while (!Client.Connected)
        //        {
        //            Thread.Sleep(500);
        //        }

        //        Console.WriteLine("Socket created to {0}", Client.RemoteEndPoint?.ToString());

        //        return;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.ToString());
        //        if (Client.Connected)
        //        {
        //            Client.Shutdown(SocketShutdown.Both);
        //            Client.Close();
        //        }
        //    }
        //}

    }
}
