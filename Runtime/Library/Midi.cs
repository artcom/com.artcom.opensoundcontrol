using System;

namespace Artcom.OpenSoundControl.Library {
    public struct Midi : IEquatable<Midi> {
        public readonly byte port;
        public readonly byte status;
        public readonly byte data1;
        public readonly byte data2;

        public Midi(byte port, byte status, byte data1, byte data2) {
            this.port = port;
            this.status = status;
            this.data1 = data1;
            this.data2 = data2;
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }

            if(obj is Midi) {
                return Equals((Midi) obj);
            }

            if(obj.GetType() == typeof(byte[])) {
                var bytes = (byte[]) obj;
                return port == bytes[0] && status == bytes[1] && data1 == bytes[2] && data2 == bytes[3];
            }

            return false;
        }

        public bool Equals(Midi other) {
            return port == other.port && status == other.status && data1 == other.data1 && data2 == other.data2;
        }

        public static bool operator ==(Midi a, Midi b) {
            return a.Equals(b);
        }

        public static bool operator !=(Midi a, Midi b) {
            return !a.Equals(b);
        }

        public override int GetHashCode() {
            return (port << 24) + (status << 16) + (data1 << 8) + data2;
        }
    }
}