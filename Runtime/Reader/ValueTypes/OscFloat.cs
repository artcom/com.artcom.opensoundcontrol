using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts {
    [AddComponentMenu("OSC/Reader/OSC Float Reader")]
    public class OscFloat : OscAdapter {
        public float lastValue;
        public FloatEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (float) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}