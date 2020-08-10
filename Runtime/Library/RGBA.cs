using System;

namespace Artcom.OpenSoundControl.Library {
    public struct RGBA : IEquatable<RGBA> {
        public readonly byte r;
        public readonly byte g;
        public readonly byte b;
        public readonly byte a;

        public RGBA(byte red, byte green, byte blue, byte alpha) {
            r = red;
            g = green;
            b = blue;
            a = alpha;
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }

            if(obj is RGBA) {
                return Equals((RGBA) obj);
            }

            if(obj.GetType() == typeof(byte[])) {
                var bytes = (byte[]) obj;
                return r == bytes[0] && g == bytes[1] && b == bytes[2] && a == bytes[3];
            }

            return false;
        }

        public bool Equals(RGBA other) {
            return r == other.r && g == other.g && b == other.b && a == other.a;
        }

        public static bool operator ==(RGBA a, RGBA b) {
            return a.Equals(b);
        }

        public static bool operator !=(RGBA a, RGBA b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return (r << 24) + (g << 16) + (b << 8) + a;
        }
    }
}