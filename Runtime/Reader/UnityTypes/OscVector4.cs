using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Vector4 Reader")]
    public class OscVector4 : OscAdapter {
        public Vector4 lastValue;
        public Vec4Event eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Vector4((float) message.Arguments[0],
                                    (float) message.Arguments[1],
                                    (float) message.Arguments[2],
                                    (float) message.Arguments[3]);
            eventListener.Invoke(lastValue);
        }
    }
}