using Artcom.OpenSoundControl.Library;

namespace Artcom.OpenSoundControl.Components {
    public class OscVoid : UnityOscAdapter {
        public VoidEvent eventListener;
        public override void Process(OscMessage message, string remoteAddress, int port) {
            eventListener.Invoke();
        }
    }
}