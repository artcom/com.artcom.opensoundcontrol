using System.Net.Sockets;
using System.Threading;
using Artcom.OpenSoundControl.Library;
using NUnit.Framework;

namespace Artcom.OpenSoundControl.Editor.Tests {
    public class OscListenerTest {
        /// <summary>
        ///     Opens a listener on a specified port, then closes it and attempts to open another on the same port
        ///     Opening the second listener will fail unless the first one has been properly closed.
        /// </summary>
        [Test]
        public void CloseListener() {
            var l1 = new UdpListener(55555);
            l1.Receive();
            l1.Close();

            var l2 = new UdpListener(55555);
            l2.Receive();
            l2.Close();
        }

        /// <summary>
        ///     Tries to open two listeners on the same port, results in an exception
        /// </summary>
        [Test]
        public void CloseListenerException() {
            UdpListener l1 = null;
            var ex = false;
            try {
                l1 = new UdpListener(55555);
                l1.Receive();
                // ReSharper disable once ObjectCreationAsStatement
                new UdpListener(55555);
            } catch(SocketException) {
                ex = true;
            }

            Assert.IsTrue(ex);
            if(l1 != null) {
                l1.Close();
            }
        }

        /// <summary>
        ///     Single message receive
        /// </summary>
        [Test]
        public void ListenerSingleMsg() {
            var listener = new UdpListener(55555);
            var sender = new OscClient("127.0.0.1", 55555);
            var msg = new OscMessage("/test/", 23.42f);

            sender.Send(msg.GetBytes());

            while(true) {
                var pack = listener.Receive();
                if(pack == null) {
                    Thread.Sleep(100);
                } else {
                    break;
                }
            }

            listener.Dispose();
            sender.Dispose();
        }

        /// <summary>
        ///     Bombard the listener with messages, check if they are all received
        /// </summary>
        [Test]
        public void ListenerLoadTest() {
            var listener = new UdpListener(55555);

            var sender = new OscClient("127.0.0.1", 55555);

            var msg = new OscMessage("/test/", 23.42f);

            for(var i = 0; i < 1000; i++) {
                sender.Send(msg.GetBytes());
            }

            Thread.Sleep(100);

            for(var i = 0; i < 1000; i++) {
                var receivedMessage = listener.Receive();
                Assert.NotNull(receivedMessage);
            }

            listener.Dispose();
            sender.Dispose();
        }
    }
}