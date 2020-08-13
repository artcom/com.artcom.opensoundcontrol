using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Vector3 Reader")]
    public class OscVector3 : UnityOscAdapter {
        public Vector3 lastValue;
        public Vec3Event eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Vector3((float) message.Arguments[0],
                                    (float) message.Arguments[1],
                                    (float) message.Arguments[2]);
            eventListener.Invoke(lastValue);
        }
    }
}