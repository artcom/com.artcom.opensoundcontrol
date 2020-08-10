using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts {

    /// <summary>
    /// Basic interface which all OSC Adapter should implement
    /// </summary>
    public interface IOscAdapter {
        bool IsMatch(string address);
        bool MatchAndProcess(OscMessage message, string remoteAddress, int port);
        void Process(OscMessage message, string remoteAddress, int port);
    }
    
    public abstract class OscAdapter : MonoBehaviour, IOscAdapter {
        /// <summary>
        /// Matching pattern which is matched with by default. This needs to be set, otherwise default matching will not
        /// work. <see cref="pattern"/> is not used for matching!
        /// </summary>
        public AddressPattern pattern;
        /// <summary>
        /// This Property is not kept in sync with pattern outside of the Unity Editor. Please ensure to keep
        /// <see cref="pattern"/> always updated for accurate results.
        /// </summary>
        public string listeningPattern;
        /// <summary>
        /// Is the used receiver which will be matched against. 
        /// </summary>
        public OscReceiver targetReceiver;

        protected virtual void Start() {
            pattern = new AddressPattern(listeningPattern);
            if(targetReceiver == null) {
                Debug.LogErrorFormat("No target receiver on this Adapter [{0}] given.", GetType().Name);
                return;
            }
            targetReceiver.RegisterReader(this);
        }

        /// <summary>
        /// Creates an AddressPattern every time the Unity Editor modifies its value. This has to be done manually
        /// if this needs to be changed on runtime (<see cref="pattern"/>).
        /// </summary>
        protected virtual void OnValidate() {
            if(listeningPattern != null) {
                pattern = new AddressPattern(listeningPattern);
            }
        }
        
        /// <summary>
        /// Allows to be overwritten, otherwise simply matches the given address to
        /// the input pattern.
        /// </summary>
        public virtual bool IsMatch(string address) {
            return pattern.IsMatch(address);
        }

        /// <summary>
        /// If the message.Address is matched against, will directly start to receive the message.
        /// </summary>
        public virtual bool MatchAndProcess(OscMessage message, string remoteAddress, int port) {
            if(!IsMatch(message.Address)) {
                return false;
            }

            Process(message, remoteAddress, port);
            return true;

        }
        
        /// <summary>
        /// Processes the message and does its user defined logic
        /// </summary>
        public abstract void Process(OscMessage message, string remoteAddress, int port);
    }
}