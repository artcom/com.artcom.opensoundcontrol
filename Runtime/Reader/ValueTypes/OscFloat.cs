using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC Float Reader")]
    public class OscFloat : UnityOscAdapter {
        public float lastValue;
        public FloatEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (float) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}