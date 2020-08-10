using Artcom.OpenSoundControl.Library;

namespace Artcom.OpenSoundControl.Interfaces {
    /// <summary>
    /// Basic interface which all OSC Adapter should implement
    /// </summary>
    public interface IOscAdapter {
        bool IsMatch(string address);
        bool MatchAndProcess(OscMessage message, string remoteAddress, int port);
        void Process(OscMessage message, string remoteAddress, int port);
    }

}