using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Artcom.OpenSoundControl.Library {
    public class UdpListener : IDisposable {
        private readonly Action<byte[], string, int> BytePacketCallback;

        private readonly object callbackLock;
        private readonly ManualResetEvent closingEvent;

        private readonly Queue<byte[]> queue;

        private readonly UdpClient receivingUdpClient;
        private bool listenerClosing;
        public Action<OscPacket, string, int> oscPacketCallback;
        private IPEndPoint remoteIpEndPoint;

        public UdpListener(int port) {
            Port = port;
            queue = new Queue<byte[]>();
            closingEvent = new ManualResetEvent(false);
            callbackLock = new object();

            // try to open the port 10 times, else fail
            for(var i = 0; i < 10; i++) {
                try {
                    receivingUdpClient = new UdpClient(port) { Client = { ReceiveBufferSize = 1024 * 64 } };
                    // accept 64 kbyte
                    break;
                } catch(Exception) {
                    // Failed in ten tries, throw the exception and give up
                    if(i >= 9) {
                        throw;
                    }

                    Thread.Sleep(5);
                }
            }

            remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);

            // setup first async event
            AsyncCallback callBack = ReceiveCallback;
            receivingUdpClient.BeginReceive(callBack, null);
        }

        public UdpListener(int port, Action<OscPacket, string, int> callback) : this(port) {
            oscPacketCallback = callback;
        }

        public UdpListener(int port, Action<byte[], string, int> callback) : this(port) {
            BytePacketCallback = callback;
        }

        public int Port { get; private set; }

        public void Dispose() {
            Close();
        }

        private void ReceiveCallback(IAsyncResult result) {
            Monitor.Enter(callbackLock);
            byte[] bytes = null;

            try {
                bytes = receivingUdpClient.EndReceive(result, ref remoteIpEndPoint);
            } catch(ObjectDisposedException) {
                // Ignore if disposed. This happens when closing the listener
            }

            // Process bytes
            if(bytes != null && bytes.Length > 0) {
                if(BytePacketCallback != null) {
                    BytePacketCallback(bytes, remoteIpEndPoint.Address.ToString(), Port);
                } else if(oscPacketCallback != null) {
                    OscPacket packet = null;
                    try {
                        packet = OscPacket.GetPacket(bytes);
                    } catch(SocketException) {
                        // If there is an error reading the packet, null is sent to the callback
                    }

                    oscPacketCallback(packet, remoteIpEndPoint.Address.ToString(), Port);
                } else {
                    lock(queue) {
                        queue.Enqueue(bytes);
                    }
                }
            }

            if(listenerClosing) {
                closingEvent.Set();
            } else {
                // Setup next async event
                var callBack = new AsyncCallback(ReceiveCallback);
                receivingUdpClient.BeginReceive(callBack, null);
            }

            Monitor.Exit(callbackLock);
        }

        public void Close() {
            if(listenerClosing) {
                return;
            }

            lock(callbackLock) {
                closingEvent.Reset();
                listenerClosing = true;
                receivingUdpClient.Close();
            }

            closingEvent.WaitOne();
        }

        public OscPacket Receive() {
            if(listenerClosing) {
                throw new ArgumentException("UDPListener has been closed.");
            }

            lock(queue) {
                if(queue.Any()) {
                    var bytes = queue.Dequeue();
                    var packet = OscPacket.GetPacket(bytes);
                    return packet;
                }

                return null;
            }
        }

        public byte[] ReceiveBytes() {
            if(listenerClosing) {
                throw new ArgumentException("UDPListener has been closed.");
            }

            lock(queue) {
                if(queue.Any()) {
                    var bytes = queue.Dequeue();
                    return bytes;
                }

                return null;
            }
        }
    }
}