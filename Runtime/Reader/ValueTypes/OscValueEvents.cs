using System;
using UnityEngine.Events;

namespace Artcom.OpenSoundControl.Scripts {
    [Serializable] public class VoidEvent : UnityEvent { }
    [Serializable] public class FloatEvent : UnityEvent<float> { }
    [Serializable] public class IntEvent : UnityEvent<int> { }
    [Serializable] public class BoolEvent : UnityEvent<bool> { }
    [Serializable] public class ByteEvent : UnityEvent<byte> { }
    [Serializable] public class CharEvent : UnityEvent<char> { }
    [Serializable] public class StringEvent : UnityEvent<string> { }
    [Serializable] public class DoubleEvent : UnityEvent<double> { }
}