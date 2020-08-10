using System;

namespace Artcom.OpenSoundControl.Library {
    public class Symbol : IEquatable<Symbol> {
        public Symbol() {
            Value = string.Empty;
        }

        public Symbol(string value) {
            Value = value;
        }

        public string Value { get; private set; }


        public override string ToString() {
            return Value;
        }

        public bool Equals(Symbol other) {
            if(ReferenceEquals(null, other)) return false;
            if(ReferenceEquals(this, other)) return true;
            return string.Equals(Value, other.Value);
        }
    
        public override bool Equals(object obj) {
            if(ReferenceEquals(null, obj)) return false;
            if(ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Symbol) obj);
        }

        public static bool operator ==(Symbol a, Symbol b) {
            return a != null && a.Equals(b);
        }

        public static bool operator !=(Symbol a, Symbol b) {
            return !(a == b);
        }

        public override int GetHashCode() {
            return (Value != null ? Value.GetHashCode() : 0);
        }
    }
}