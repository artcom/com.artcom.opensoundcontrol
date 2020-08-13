using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC Double Reader")]
    public class OscDouble : UnityOscAdapter {
        public double lastValue;
        public DoubleEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (double) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}