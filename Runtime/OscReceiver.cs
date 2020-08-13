using System;
using System.Collections.Generic;
using Artcom.OpenSoundControl.Interfaces;
using Artcom.OpenSoundControl.Library;
using UnityEngine;

namespace Artcom.OpenSoundControl.Components {
    /// <summary>
    /// OSC Bundles which should not be used - just visible for serialization reasons in Unity
    /// </summary>
    [Serializable]
    internal struct OscBundledMessage {
        public OscPacket packet;
        public string url;
        public int port;
    }

    /// <summary>
    /// Simple UDP receiver on which OscAdapters can register themselves
    /// </summary>
    public class OscReceiver : MonoBehaviour, IOscReceiver {
        [SerializeField] private int inPort;
        internal List<UnityOscAdapter> unityReaders;
        [SerializeField] private Queue<OscBundledMessage> messageQueue;
        private UdpListener listener;
        private List<IOscAdapter> readers;

        public int InPort {
            get => inPort;
            set {
                inPort = value;
                if(listener != null) {
                    CloseListener();
                }
                if(enabled) {
                    InitListener();
                }
            }
        }

        private void InitListener() {
            // we are seemingly already listening, no work has to be done.
            if(listener != null) {
                return;
            }

            if (unityReaders == null) {
                unityReaders = new List<UnityOscAdapter>();
            }

            if (readers == null) {
                readers = new List<IOscAdapter>(unityReaders);
            }

            if(listener != null) {
                CloseListener();
            }
            
            messageQueue = new Queue<OscBundledMessage>();
            lock(messageQueue) {
                listener = new UdpListener(inPort);
                listener.oscPacketCallback += OscMessageCallback;
            }
        }

        /// <summary>
        /// Called when a new packet has arrived at the socket. We merely store it in a queue to event it out on the
        /// main thread
        /// </summary>
        private void OscMessageCallback(OscPacket packet, string url, int port) {
            lock(messageQueue) {
                 messageQueue.Enqueue(new OscBundledMessage{ packet = packet, url = url, port = port });
            }
        }

        private void CloseListener() {
            if (listener == null) {
                return;
            }
            // locking here the queue ensures that the thread queue doesn't access the socket while disposing it
            lock(messageQueue) {
                listener.Close();
                listener.Dispose();
                listener = null;
            }
        }
        

        public void RegisterReader(IOscAdapter adapter) {
            if(adapter == null) {
                return;
            }
            if(readers == null) {
                Debug.LogError("Reader List is not initialized");
                return;
            }

            // we keep references of serializable Unity Adapters to survive recompilation and reimports
            if (adapter is UnityOscAdapter unityOscAdapter) {
                unityReaders.Add(unityOscAdapter);
            }
            
            readers.Add(adapter);
        }

        public void RemoveReader(IOscAdapter adapter) {
            if(adapter == null) {
                return;
            }

            if(readers == null) {
                Debug.LogError("Reader List is not initialized.");
                return;
            }

            readers.Remove(adapter);
        }

        private void OnEnable() {
            InitListener();
        }

        private void OnDisable() {
            CloseListener();
        }

        private void Start() {
            OscReceiverManager.Instance.Add(this);
            if(listener != null) {
                InitListener();
            }
        }

        private void OnDestroy() {
            OscReceiverManager.Instance.Remove(this);
        }

        private void Update() {
            lock(messageQueue) {
                readers.RemoveAll(x => x == null);
                while(messageQueue.Count > 0) {
                    var message = messageQueue.Dequeue();
                    var oscMessage = (OscMessage) message.packet;
                    var hasBeenMatched = false;
                    // clean up readers that have become invalid.
                    foreach(var reader in readers) {
                        // we really have a fault problem here, so we try-catch that instead.
                        try {
                            hasBeenMatched |= reader.MatchAndProcess(oscMessage, message.url, message.port);
                        } catch(Exception e) {
                            Debug.LogErrorFormat(
                                "API [{0}] seems to be malformed or parser [{1}] threw an error:",
                                oscMessage.Address, reader.GetType().Name);
                            Debug.LogError(e);
                        }
                    }
                    if(!hasBeenMatched) {
                        Debug.LogWarningFormat("OSC API Path [{0}] doesn't have a listener", oscMessage.Address);
                    }
                }
            }
        }
    }
}
