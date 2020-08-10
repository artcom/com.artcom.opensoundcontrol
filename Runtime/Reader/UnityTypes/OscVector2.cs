using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Vector2 Reader")]
    public class OscVector2 : OscAdapter {
        public Vector2 lastValue;
        public Vec2Event eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Vector3((float) message.Arguments[0],
                                    (float) message.Arguments[1]);
            eventListener.Invoke(lastValue);
        }
    }
}