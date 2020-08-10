namespace Artcom.OpenSoundControl.Interfaces {
    public interface IOscReceiver {
        int InPort { get; set; }
        void RegisterReader(IOscAdapter adapter);
        void RemoveReader(IOscAdapter adapter);
    }
}