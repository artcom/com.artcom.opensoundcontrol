using Artcom.OpenSoundControl.Interfaces;
using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    /// <summary>
    /// OSC Sender to a specified address and port.
    /// </summary>
    public class OscSender : MonoBehaviour, IOscSender {
        [SerializeField] private string address;
        private OscClient client;
        [SerializeField] private int outPort;

        public int OutPort {
            get { return outPort; }
            set {
                outPort = value;
                ReinitializeClient();
            }
        }

        public string Address {
            get { return address; }
            set {
                address = value;
                ReinitializeClient();
            }
        }

        public void Send(byte[] data) {
            if(data == null) {
                Debug.LogWarning("data cannot be null, omitting send request");
                return;
            }

            if (client == null) {
                Debug.LogWarning("client not initialized, omitting send request");
                return;
            }

            client.Send(data);
        }

        public void Send(OscPacket packet) {
            if(packet == null) {
                Debug.LogWarning("packet cannot be null, omitting send request");
                return;
            }
            
            Send(packet.GetBytes());
        }

        private void OnDestroy() {
            CleanupClient();
        }

        private void OnDisable() {
            CleanupClient();
        }

        /// <summary>
        ///     Destroys the currently client and cleans it up.
        /// </summary>
        private void CleanupClient() {
            if(client == null) {
                return;
            }

            client.Dispose();
            client = null;
        }

        /// <summary>
        ///     If we had a client before, we will have a new client that sends with
        ///     new settings.
        /// </summary>
        private void ReinitializeClient() {
            if(client == null) {
                return;
            }

            client.Dispose();
            client = new OscClient(address, outPort);
        }

        private void OnEnable() {
            if (address == null || outPort == 0) {
                enabled = false;
                return;
            } 
            client = new OscClient(address, outPort);
        }
    }
}