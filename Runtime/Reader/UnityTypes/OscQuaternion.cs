using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Quaternion Reader")]
    public class OscQuaternion : UnityOscAdapter {
        public Quaternion lastValue;
        public QuaternionEvent eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Quaternion((float) message.Arguments[0],
                                       (float) message.Arguments[1],
                                       (float) message.Arguments[2],
                                       (float) message.Arguments[3]);
            eventListener.Invoke(lastValue);
        }
    }
}