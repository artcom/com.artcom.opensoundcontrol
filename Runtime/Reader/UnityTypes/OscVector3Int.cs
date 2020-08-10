using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Vector3Int Reader")]
    public class OscVector3Int : OscAdapter {
        public Vector3Int lastValue;
        public Vec3IntEvent eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Vector3Int((int) message.Arguments[0],
                                       (int) message.Arguments[1],
                                       (int) message.Arguments[2]);
            eventListener.Invoke(lastValue);
        }
    }
}