using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts {
    [AddComponentMenu("OSC/Reader/OSC Char Reader")]
    public class OscChar : OscAdapter {
        public char lastValue;
        public CharEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (char) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}