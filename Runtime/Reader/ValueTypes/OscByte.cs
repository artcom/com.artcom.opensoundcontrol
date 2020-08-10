using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    [AddComponentMenu("OSC/Reader/OSC Byte Reader")]
    public class OscByte : OscAdapter {
        public byte lastValue;
        public ByteEvent eventListener;

        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = (byte) message.Arguments[0];
            eventListener.Invoke(lastValue);
            
        }
    }
}