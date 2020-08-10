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
    /// 
    /// </summary>
    public class OscReceiver : MonoBehaviour, IOscReceiver {
        [SerializeField] private List<UnityEngine.Object> readers;
        [SerializeField] private int inPort;

        private UdpListener listener;
        [SerializeField] private Queue<OscBundledMessage> messageQueue;
        private object queueLock;

        public int InPort {
            get { return inPort; }
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
            if(listener != null) {
                return;
            }
            queueLock = new object();
            if(listener != null) {
                CloseListener();
            }

            lock(queueLock) {
                messageQueue = new Queue<OscBundledMessage>();
                listener = new UdpListener(inPort);
                listener.oscPacketCallback += OscMessageCallback;
            }
        }

        private void OscMessageCallback(OscPacket packet, string url, int port) {
            lock(queueLock) {
                 messageQueue.Enqueue(new OscBundledMessage{ packet = packet, url = url, port = port });
            }
        }

        private void CloseListener() {
            lock(queueLock) {
                if(listener != null) {
                    listener.Close();
                    listener.Dispose();
                    listener = null;
                }
            }
        }
        

        public void RegisterReader(IOscAdapter adapter) {
            //Debug.Log("Adapter added.");
            if(adapter == null) {
                return;
            }
            if(readers == null) {
                Debug.LogError("Reader List is not initialized");
                return;
            }
            
            readers.Add(adapter as UnityEngine.Object);
        }

        public void RemoveReader(IOscAdapter adapter) {
            if(adapter == null) {
                return;
            }

            if(readers == null) {
                Debug.LogError("Reader List is not initialized.");
                return;
            }

            readers.Remove(adapter as UnityEngine.Object);
        }

        void Awake() {
            readers = new List<UnityEngine.Object>();
        }

        void OnEnable() {
            InitListener();
        }

        void OnDisable() {
            CloseListener();
        }

        void OnDestroy() {
            if(listener != null) {
                listener.Close();
                listener.Dispose();
                listener = null;
            }
        }

        void Start() {
            OscReceiverManager.Instance.Add(this);
            if(listener != null) {
                InitListener();
            }
        }

        private void Update() {
            lock(queueLock) {
                while(messageQueue.Count > 0) {
                    var message = messageQueue.Dequeue();
                    var oscMessage = (OscMessage) message.packet;
                    var hasBeenMatched = false;
                    foreach(var reader in readers) {
                        if(reader == null) {
                           readers.Remove(reader);
                        }
                        try {
                            hasBeenMatched |= ((IOscAdapter) reader).MatchAndProcess(oscMessage, message.url, message.port);
                        } catch(Exception e) {
                            Debug.LogErrorFormat(
                                "API [{0}] seems to be malformed or parser [{1}] seems to be incorred and threw and error:",
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
