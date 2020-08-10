using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC Bool Reader")]
    public class OscBool : OscAdapter {
        public bool lastValue;
        public BoolEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            if(message.Arguments[0] is float) {
                lastValue = ((float) message.Arguments[0] != 0);
            } else {
                lastValue = (bool) message.Arguments[0];
            }

            eventListener.Invoke(lastValue);
            
        }
    }
}