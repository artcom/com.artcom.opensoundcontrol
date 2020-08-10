using Artcom.OpenSoundControl.Library;

namespace Artcom.OpenSoundControl.Components {
    public class OscVoid : OscAdapter {
        public VoidEvent eventListener;
        public override void Process(OscMessage message, string remoteAddress, int port) {
            eventListener.Invoke();
        }
    }
}