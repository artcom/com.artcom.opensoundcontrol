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

            var typeStringBuilder = new StringBuilder(",");
            var i = 0;
            while(i < currentList.Count) {
                var arg = currentList[i];
                switch(arg) {
                    case int x:
                        typeStringBuilder.Append("i");
                        parts.Add(SetInt(x));
                        break;
                    case float x:
                        if(float.IsPositiveInfinity(x)) {
                            typeStringBuilder.Append("I");
                        } else {
                            typeStringBuilder.Append("f");
                            parts.Add(SetFloat(x));
                        }
                        break;
                    case string x:
                        typeStringBuilder.Append("s");
                        parts.Add(SetString(x));
                        break;
                    case byte[] x:
                        typeStringBuilder.Append("b");
                        parts.Add(Set(x));
                        break;
                    case long x:
                        typeStringBuilder.Append("h");
                        parts.Add(Set(x));
                        break;
                    case ulong x:
                        typeStringBuilder.Append("t");
                        parts.Add(Set(x));
                        break;
                    case Timetag x:
                        typeStringBuilder.Append("t");
                        parts.Add(Set(x.tag));
                        break;
                    case double x:
                        if(double.IsPositiveInfinity(x)) {
                            typeStringBuilder.Append("I");
                        } else {
                            typeStringBuilder.Append("d");
                            parts.Add(Set(x));
                        }

                        break;

                    case Symbol x:
                        typeStringBuilder.Append("S");
                        parts.Add(SetString(x.Value));
                        break;

                    case char x:
                        typeStringBuilder.Append("c");
                        parts.Add(Set(x));
                        break;
                    case RGBA x:
                        typeStringBuilder.Append("r");
                        parts.Add(Set(x));
                        break;
                    case Midi x:
                        typeStringBuilder.Append("m");
                        parts.Add(Set(x));
                        break;
                    case bool x:
                        typeStringBuilder.Append(x ? "T" : "F");
                        break;
                    case null:
                        typeStringBuilder.Append("N");
                        break;

                    // This part handles arrays. It points currentList to the array and reSets i
                    // The array is processed like normal and when it is finished we replace  
                    // currentList back with Arguments and continue from where we left off
                    case object[] x:
                        if (Arguments != currentList) {
                            throw new ArgumentException("Nested Arrays are not supported");
                        }

                        typeStringBuilder.Append("[");
                        currentList = x.ToList();
                        argumentsIndex = i;
                        i = 0;
                        continue;
                    case List<object> y:
                        typeStringBuilder.Append("[");
                        currentList = y;
                        argumentsIndex = i;
                        i = 0;
                        continue;
                    default:
                        throw new ArgumentException($"Unable to transmit values of type {arg.GetType().FullName}");
                }

                i++;
                if(currentList != Arguments && i == currentList.Count) {
                    // End of array, go back to main Argument list
                    typeStringBuilder.Append("]");
                    currentList = Arguments;
                    i = argumentsIndex + 1;
                }
            }

            var addressLen = string.IsNullOrEmpty(Address) ? 0 : Utils.AlignedStringLength(Address);
            var typeString = typeStringBuilder.ToString();
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