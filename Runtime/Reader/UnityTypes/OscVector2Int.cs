using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components.UnityTypes {
    [AddComponentMenu("OSC/Reader/OSC Vector2Int Reader")]
    public class OscVector2Int : OscAdapter {
        public Vector2Int lastValue;
        public Vec2IntEvent eventListener;
        
        public override void Process(OscMessage message, string remoteAddress, int port) {
            lastValue = new Vector2Int((int) message.Arguments[0],
                                       (int) message.Arguments[1]);
            eventListener.Invoke(lastValue);
        }
    }
}