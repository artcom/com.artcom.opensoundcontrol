using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC String Reader")]
    public class OscString : UnityOscAdapter {
        public string lastValue;
        public StringEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (string) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}