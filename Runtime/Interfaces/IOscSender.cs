using Artcom.OpenSoundControl.Library;

namespace Artcom.OpenSoundControl.Interfaces {
    public interface IOscSender {
        int OutPort { get; set; }
        string Address { get; set; }
        void Send(byte[] data);
        void Send(OscPacket packet);
    }
}