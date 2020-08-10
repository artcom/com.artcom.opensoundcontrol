using System;
using System.Net;
using System.Net.Sockets;

namespace Artcom.OpenSoundControl.Library {
    public sealed class OscClient : IDisposable {
        private readonly IPEndPoint endpoint;
        private Socket sock;

        public OscClient(string address, int outPort) {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            // Buffer can be 64 kbytes big
            // sock.SendBufferSize = 1024 * 64;
            // sock.NoDelay = true;
            var addresses = Dns.GetHostAddresses(address);
            if(addresses.Length == 0) {
                throw new ArgumentException("Unable to find IP Address");
            }

            endpoint = new IPEndPoint(addresses[0], outPort);
        }

        public void Dispose() {
            sock.Close();
            sock = null;
        }

        public void Send(byte[] data) {
            if(data == null) {
                throw new ArgumentNullException("data");
            }

            sock.SendTo(data, endpoint);
        }

        public void Send(OscPacket packet) {
            if(packet == null) {
                throw new ArgumentNullException("packet");
            }

            Send(packet.GetBytes());
        }
    }
}