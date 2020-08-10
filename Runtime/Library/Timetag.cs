using System;

namespace Artcom.OpenSoundControl.Library {
    public struct Timetag : IEquatable<Timetag> {
        public ulong tag;

        public DateTime Timestamp {
            get { return Utils.TimetagToDateTime(tag); }
            set { tag = Utils.DateTimeToTimetag(value); }
        }

        /// <summary>
        ///     Gets or sets the fraction of a second in the timestamp. the double precision number is multiplied by 2^32
        ///     giving an accuracy down to about 230 picoseconds ( 1/(2^32) of a second)
        /// </summary>
        public double Fraction {
            get { return Utils.TimetagToFraction(tag); }
            set { tag = (tag & 0xFFFFFFFF00000000) + (uint) (value * 0xFFFFFFFF); }
        }

        public Timetag(ulong value) {
            tag = value;
        }

        public Timetag(DateTime value) {
            tag = 0;
            Timestamp = value;
        }

        public override bool Equals(object obj) {
            if(obj == null) {
                return false;
            }

            if(obj is Timetag) {
                return Equals((Timetag) obj);
            }

            return tag == obj as ulong?;
        }

        public bool Equals(Timetag other) {
            return tag == other.tag;
        }

        public static bool operator ==(Timetag a, Timetag b) {
            return a.Equals(b);
        }

        public static bool operator !=(Timetag a, Timetag b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return (int) (((uint) (tag >> 32) + (uint) (tag & 0x00000000FFFFFFFF)) / 2);
        }
    }
}