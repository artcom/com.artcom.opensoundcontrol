using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// ReSharper disable PossibleNullReferenceException

namespace Artcom.OpenSoundControl.Library {
    public class OscMessage : OscPacket {
        public OscMessage(string address, params object[] args) {
            Address = address;
            Arguments = new List<object>();
            Arguments.AddRange(args);
        }

        public string Address { get; private set; }
        public List<object> Arguments { get; private set; }

        public override byte[] GetBytes() {
            var parts = new List<byte[]>();

            var currentList = Arguments;
            var argumentsIndex = 0;

            var typeString = ",";
            var i = 0;
            while(i < currentList.Count) {
                var arg = currentList[i];
                var type = arg != null ? arg.GetType().ToString() : "null";
                switch(type) {
                    case "System.Int32":
                        typeString += "i";
                        parts.Add(SetInt((int) arg));
                        break;
                    case "System.Single":
                        var floatValue = (float) arg;
                        if(float.IsPositiveInfinity(floatValue)) {
                            typeString += "I";
                        } else {
                            typeString += "f";
                            parts.Add(SetFloat(floatValue));
                        }

                        break;
                    case "System.String":
                        typeString += "s";
                        parts.Add(SetString((string) arg));
                        break;
                    case "System.Byte[]":
                        typeString += "b";
                        parts.Add(Set((byte[]) arg));
                        break;
                    case "System.Int64":
                        typeString += "h";
                        parts.Add(Set((long) arg));
                        break;
                    case "System.UInt64":
                        typeString += "t";
                        parts.Add(Set((ulong) arg));
                        break;
                    case "Artcom.OpenSoundControl.Library.Timetag":
                        typeString += "t";
                        parts.Add(Set(((Timetag) arg).tag));
                        break;
                    case "System.Double":
                        var doubleValue = (double) arg;
                        if(double.IsPositiveInfinity(doubleValue)) {
                            typeString += "I";
                        } else {
                            typeString += "d";
                            parts.Add(Set(doubleValue));
                        }

                        break;

                    case "Artcom.OpenSoundControl.Library.Symbol":
                        typeString += "S";
                        parts.Add(SetString(((Symbol) arg).Value));
                        break;

                    case "System.Char":
                        typeString += "c";
                        parts.Add(Set((char) arg));
                        break;
                    case "Artcom.OpenSoundControl.Library.RGBA":
                        typeString += "r";
                        parts.Add(Set((RGBA) arg));
                        break;
                    case "Artcom.OpenSoundControl.Library.Midi":
                        typeString += "m";
                        parts.Add(Set((Midi) arg));
                        break;
                    case "System.Boolean":
                        typeString += (bool) arg ? "T" : "F";
                        break;
                    case "null":
                        typeString += "N";
                        break;

                    // This part handles arrays. It points currentList to the array and reSets i
                    // The array is processed like normal and when it is finished we replace  
                    // currentList back with Arguments and continue from where we left off
                    case "System.Object[]":
                    case "System.Collections.Generic.List`1[System.Object]":
                        if(arg.GetType() == typeof(object[])) {
                            arg = ((object[]) arg).ToList();
                        }

                        if(Arguments != currentList) {
                            throw new ArgumentException("Nested Arrays are not supported");
                        }

                        typeString += "[";
                        currentList = (List<object>) arg;
                        argumentsIndex = i;
                        i = 0;
                        continue;

                    default:
                        throw new ArgumentException("Unable to transmit values of type " + type);
                }

                i++;
                if(currentList != Arguments && i == currentList.Count) {
                    // End of array, go back to main Argument list
                    typeString += "]";
                    currentList = Arguments;
                    i = argumentsIndex + 1;
                }
            }

            var addressLen = string.IsNullOrEmpty(Address) ? 0 : Utils.AlignedStringLength(Address);
            var typeLen = Utils.AlignedStringLength(typeString);

            var total = addressLen + typeLen + parts.Sum(x => x.Length);

            var output = new byte[total];
            i = 0;

            Encoding.ASCII.GetBytes(Address).CopyTo(output, i);
            i += addressLen;

            Encoding.ASCII.GetBytes(typeString).CopyTo(output, i);
            i += typeLen;

            foreach(var part in parts) {
                part.CopyTo(output, i);
                i += part.Length;
            }

            return output;
        }
    }
}