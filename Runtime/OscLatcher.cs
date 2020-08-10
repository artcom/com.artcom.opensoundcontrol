using System.Collections.Generic;
using System.Linq;
using Artcom.OpenSoundControl.Interfaces;
using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Scripts {
    /// <summary>
    /// Automatic endpoint latcher that acts like a sender but has no fixed endpoint.
    /// </summary>
    public class OscLatcher : MonoBehaviour, IOscSender, IOscAdapter {
        private GameObject senderHostObject;
        private List<IPAddress> endpoints;
        [SerializeField] private string enableListeningPattern = "/debug/enable";
        [SerializeField] private string disableListeningPattern = "/debug/disable";

        [SerializeField] private AddressPattern enablePattern;
        [SerializeField] private AddressPattern disablePattern;

        [SerializeField] private OscReceiver targetReceiver;
        
        private void Awake() {
            endpoints = new List<IPAddress>();
        }

        private void Start() {
            enablePattern = new AddressPattern(enableListeningPattern);
            disablePattern = new AddressPattern(disableListeningPattern);
            if(targetReceiver == null) {
                Debug.LogErrorFormat("No target receiver on this Adapter [{0}] given.", GetType().Name);
                return;
            }
            targetReceiver.RegisterReader(this);
        }

        public bool IsMatch(string address) {
            return enablePattern.IsMatch(address) || disablePattern.IsMatch(address);
        }

        public bool MatchAndProcess(OscMessage message, string remoteAddress, int port) {
            if (!IsMatch(message.Address)) {
                return false;
            }

            Process(message, remoteAddress, port);
            return true;

        }

        public void Process(OscMessage message, string remoteAddress, int port) {
            if (message.Arguments.Count <= 0) {
                return;
            }
            
            int resolvePort;
            switch (message.Arguments[0]) {
                case int x:
                    resolvePort = x;
                    break;
                case float x:
                    resolvePort = (int) x;
                    break;
                default:
                    return;
            }

            if (enablePattern.IsMatch(message.Address)) {



                AttachSender(remoteAddress, resolvePort);
            } else {
                DetachSender(remoteAddress, resolvePort);
            }
        }

        private void AttachSender(string remoteAddress, int port) {
            // check if we're already sending to that endpoint.
            if (endpoints.Any(x => x.remoteAddress == remoteAddress && x.port == port)) {
                return;
            }
            EnsureHost();
            var sender = senderHostObject.AddComponent<OscSender>();
            sender.Address = remoteAddress;
            sender.OutPort = port;
            sender.enabled = true;
            endpoints.Add(new IPAddress(remoteAddress, port, sender));
        }

        private void DetachSender(string remoteAddress, int port) {
            var index = endpoints.FindIndex(x => x.remoteAddress == remoteAddress && x.port == port);
            if (index < 0) {
                return;
            }
            var endpoint = endpoints[index];
            Destroy((UnityEngine.Object) endpoint.sender);
            endpoints.RemoveAt(index);
        }

        private void EnsureHost() {
            if (senderHostObject != null) {
                return;
            }

            senderHostObject = new GameObject("OSC Debug Sender Host") {hideFlags = HideFlags.DontSave};
            DontDestroyOnLoad(senderHostObject);
        }

        private struct IPAddress {
            public string remoteAddress;
            public int port;
            public IOscSender sender;

            public IPAddress(string remoteAddress, int port, IOscSender sender) {
                this.remoteAddress = remoteAddress;
                this.port = port;
                this.sender = sender;
            }
        }

        public int OutPort { get; set; }
        public string Address { get; set; }
        public void Send(byte[] data) {
            foreach (var endpoint in endpoints) {
                endpoint.sender.Send(data);
            }
        }

        public void Send(OscPacket packet) {
            foreach (var endpoint in endpoints) {
                endpoint.sender.Send(packet);
            }
        }
    }
}