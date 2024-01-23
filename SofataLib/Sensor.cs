﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Softata
{
    public enum GroveSensorCmds{getpins, getProperties, setupdefault, setup,  readall, read, getTelemetry, sendTelemetryBT, sendTelemetryToIoTHub, pause_sendTelemetry, continue_sendTelemetry, getSensors =255
      ,
    }
    // getpins, getProperties are specific sensor class static commands so send sensor type as other rather than linkedListNo
    // getSensors is a static sensor class command.
    public partial class SoftataLib
    {

        public static class Sensor
        {

            public static string[] GetSensors()
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.getSensors, "OK:", 0);
                result = result.Replace("SENSORS:","");
                if (!string.IsNullOrEmpty(result))
                    return result.Split(',');
                else
                    return new string[0];
            }

            public static string GetPins(byte sensorType, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.getpins, "OK:", sensorType, null, debug=true);
                return result;
            }

            public static string[] GetProperties(byte sensorType, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.getProperties, "OK:", sensorType, null, debug=true);
                if (!string.IsNullOrEmpty(result))
                    return result.Split(',');
                else
                    return new string[0];
            }

            public static int SetupDefault(byte sensorType, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.setupdefault, "OK:", sensorType, null, debug);
                if (int.TryParse(result, out int linkedListNo))
                    return linkedListNo;
                else
                    return -1;
            }

            public static double[]?  ReadAll(byte linkedListNo, bool debug=true )
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.readall , "OK:",linkedListNo,null,debug);
                try { 
                    string[] values = result.Split(',');
                    double[]? results = (from v in values
                             select double.Parse(v)).ToArray();
                    if(results != null)
                        return results;
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }

            public static string GetTelemetry(byte linkedListNo, bool debug=true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.getTelemetry, "OK:", linkedListNo, null,debug);
                return result;
            }

            public static double? Read(byte linkedListNo, byte property, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, property, (byte)GroveSensorCmds.read, "OK:", linkedListNo, null,debug);
                try
                {
                    if (double.TryParse(result, out double value))
                        return value;
                    else
                        return null;
                }
                catch
                {
                    return null;
                }
            }

            public static string SendTelemetryBT(byte linkedListNo, bool debug=true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.sendTelemetryBT, "OK:", linkedListNo, null, debug);
                if(int.TryParse(result, out int value))
                {
                   
                }
                return result;
            }

            public static string SendTelemetryToIoTHub(byte linkedListNo, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.sendTelemetryToIoTHub, "OK:", linkedListNo, null, debug);
                if (int.TryParse(result, out int value))
                {

                }
                return result;
            }

            public static string PauseSendTelemetry(byte linkedListNo, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.pause_sendTelemetry, "OK:", linkedListNo, null, debug = true);
                return result;
            }

            public static string ContinueSendTelemetry(byte linkedListNo, bool debug = true)
            {
                string result = SendMessage(Commands.groveSensor, 0, (byte)GroveSensorCmds.continue_sendTelemetry, "OK:", linkedListNo, null, debug= true);
                return result;
            }
        }


    }
}
