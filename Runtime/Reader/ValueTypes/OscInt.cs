using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC Int Reader")]
    public class OscInt : OscAdapter {
        public int lastValue;
        public IntEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (int) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}