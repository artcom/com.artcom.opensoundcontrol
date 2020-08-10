using System;
using UnityEngine;
using UnityEngine.Events;

namespace Artcom.OpenSoundControl.Scripts {
    [Serializable] public class Vec2Event : UnityEvent<Vector2> { }
    [Serializable] public class Vec3Event : UnityEvent<Vector3> { }
    [Serializable] public class Vec4Event : UnityEvent<Vector4> { }
    [Serializable] public class Vec2IntEvent : UnityEvent<Vector2Int> { }
    [Serializable] public class Vec3IntEvent : UnityEvent<Vector3Int> { }
    [Serializable] public class QuaternionEvent : UnityEvent<Quaternion> { } 
}