using System;
using Artcom.OpenSoundControl.Library;
using NUnit.Framework;

namespace Artcom.OpenSoundControl.Editor.Tests {
    public class OscParsingTest {
        // translates to:
        // /oscillator/4/frquency ,f  440.0
        private static readonly byte[] ExampleSimpleData = {
            0x2f, 0x6f, 0x73, 0x63,
            0x69, 0x6c, 0x6c, 0x61,
            0x74, 0x6f, 0x72, 0x2f,
            0x34, 0x2f, 0x66, 0x72,
            0x65, 0x71, 0x75, 0x65,
            0x6e, 0x63, 0x79, 0x00,
            0x2c, 0x66, 0x00, 0x00,
            0x43, 0xdc, 0x00, 0x00
        };

        // translates to:
        // /foo    ,iisff    1000 -1 "hello" 1.234 5.678
        private static readonly byte[] ExampleMultiData = {
            0x2f, 0x66, 0x6f, 0x6f,
            0x00, 0x00, 0x00, 0x00,
            0x2c, 0x69, 0x69, 0x73,
            0x66, 0x66, 0x00, 0x00,
            0x00, 0x00, 0x03, 0xe8,
            0xff, 0xff, 0xff, 0xff,
            0x68, 0x65, 0x6c, 0x6c,
            0x6f, 0x00, 0x00, 0x00,
            0x3f, 0x9d, 0xf3, 0xb6,
            0x40, 0xb5, 0xb2, 0x2d
        };

        private static readonly byte[] ExampleNoData = {
            0x2f, 0x70, 0x69, 0x6E,
            0x67, 0x00, 0x00, 0x00,
            0x2c, 0x00, 0x00, 0x00
        };

        [Test]
        public void ParsingSimple() {
            var oscPacket = OscPacket.GetPacket(ExampleSimpleData);
            var message = (OscMessage) oscPacket;
            Assert.AreEqual(message.Address, "/oscillator/4/frequency");
            Assert.AreEqual(message.Arguments.Count, 1);
            Assert.AreEqual((float) message.Arguments[0], 440f);
        }

        [Test]
        public void ParsingMulti() {
            var oscPacket = OscPacket.GetPacket(ExampleMultiData);
            var message = (OscMessage) oscPacket;
            Assert.AreEqual(message.Address, "/foo");
            Assert.AreEqual(message.Arguments.Count, 5);
            Assert.AreEqual((int) message.Arguments[0], 1000);
            Assert.AreEqual((int) message.Arguments[1], -1);
            Assert.AreEqual((string) message.Arguments[2], "hello");
            Assert.AreEqual((float) message.Arguments[3], 1.234f);
            Assert.AreEqual((float) message.Arguments[4], 5.678f);
        }

        [Test]
        public void ParsingNoArguments() {
            var oscPacket = OscPacket.GetPacket(ExampleNoData);
            var message = (OscMessage) oscPacket;
            Assert.AreEqual(message.Address, "/ping");
            Assert.AreEqual(message.Arguments.Count, 0);
        }

        [Test]
        public void BinaryConversionSimple() {
            var msg = new OscMessage("/oscillator/4/frequency", 440f);
            var bytes = msg.GetBytes();
            Assert.AreEqual(bytes.Length, ExampleSimpleData.Length);
            for(var i = 0; i < ExampleSimpleData.Length; i++) {
                Assert.AreEqual(ExampleSimpleData[i], bytes[i]);
            }
        }

        [Test]
        public void BinaryConversionMulti() {
            var msg = new OscMessage("/foo", 1000, -1, "hello", 1.234f, 5.678f);
            var bytes = msg.GetBytes();
            Assert.AreEqual(bytes.Length, ExampleMultiData.Length);
            for(var i = 0; i < ExampleMultiData.Length; i++) {
                Assert.AreEqual(ExampleMultiData[i], bytes[i]);
            }
        }

        [Test]
        public void BinaryConversionNoData() {
            var msg = new OscMessage("/ping");
            var bytes = msg.GetBytes();
            Assert.AreEqual(bytes.Length, ExampleNoData.Length);
            for(var i = 0; i < ExampleNoData.Length; i++) {
                Assert.AreEqual(ExampleNoData[i], bytes[i]);
            }
        }

        /// <summary>
        ///     Takes a full trip by creating two messages, combines them to a bundle,
        ///     gets bytes, parses the bytes back into a bundle and checks all bundle
        ///     values.
        /// </summary>
        [Test]
        public void BundleFullConversion() {
            var msg1 = new OscMessage("/test/address1", 23, 42.42f, "hello world", new byte[] { 2, 3, 4 });
            var msg2 = new OscMessage("/test/address2", 34, 24.24f, "hello again", new byte[] { 5, 6, 7, 8, 9 });
            var dt = DateTime.Now;
            var bundle = new OscBundle(Utils.DateTimeToTimetag(dt), msg1, msg2);
            var transposedBundle = (OscBundle) OscPacket.GetPacket(bundle.GetBytes());

            Assert.AreEqual(dt.Date, transposedBundle.Timestamp.Date);
            Assert.AreEqual(dt.Hour, transposedBundle.Timestamp.Hour);
            Assert.AreEqual(dt.Minute, transposedBundle.Timestamp.Minute);
            Assert.AreEqual(dt.Second, transposedBundle.Timestamp.Second);

            Assert.AreEqual("/test/address1", transposedBundle.Messages[0].Address);
            Assert.AreEqual(4, transposedBundle.Messages[0].Arguments.Count);
            Assert.AreEqual(23, transposedBundle.Messages[0].Arguments[0]);
            Assert.AreEqual(42.42f, transposedBundle.Messages[0].Arguments[1]);
            Assert.AreEqual("hello world", transposedBundle.Messages[0].Arguments[2]);
            Assert.AreEqual(new byte[] { 2, 3, 4 }, transposedBundle.Messages[0].Arguments[3]);

            Assert.AreEqual("/test/address2", transposedBundle.Messages[1].Address);
            Assert.AreEqual(4, transposedBundle.Messages[1].Arguments.Count);
            Assert.AreEqual(34, transposedBundle.Messages[1].Arguments[0]);
            Assert.AreEqual(24.24f, transposedBundle.Messages[1].Arguments[1]);
            Assert.AreEqual("hello again", transposedBundle.Messages[1].Arguments[2]);
            Assert.AreEqual(new byte[] { 5, 6, 7, 8, 9 }, transposedBundle.Messages[1].Arguments[3]);
        }

        /// <summary>
        ///     Tests a full trip through the pipeline, from object creation back to
        ///     bytes parsing.
        /// </summary>
        [Test]
        public void MessageFullConversion() {
            var message = new OscMessage(
                                         "/test/address",
                                         23,
                                         42.42f,
                                         "hello world",
                                         new byte[] { 2, 3, 4 },
                                         -123456789123,
                                         new Timetag(DateTime.Now.Date).tag,
                                         new Timetag(DateTime.Now.Date.AddMonths(1)),
                                         1234567.890,
                                         new Symbol("valid message"),
                                         'x',
                                         new RGBA(20, 40, 60, 255),
                                         new Midi(3, 110, 55, 66),
                                         true,
                                         false,
                                         null,
                                         double.PositiveInfinity
                                        );
            var transposedMessage = (OscMessage) OscPacket.GetPacket(message.GetBytes());
            Assert.NotNull(transposedMessage);

            Assert.AreEqual("/test/address", transposedMessage.Address);
            Assert.AreEqual(16, transposedMessage.Arguments.Count);

            Assert.AreEqual(23, transposedMessage.Arguments[0]);
            Assert.AreEqual(42.42f, transposedMessage.Arguments[1]);
            Assert.AreEqual("hello world", transposedMessage.Arguments[2]);
            Assert.AreEqual(new byte[] { 2, 3, 4 }, transposedMessage.Arguments[3]);
            Assert.AreEqual(-123456789123, transposedMessage.Arguments[4]);
            Assert.AreEqual(new Timetag(DateTime.Now.Date), transposedMessage.Arguments[5]);
            Assert.AreEqual(new Timetag(DateTime.Now.Date.AddMonths(1)), transposedMessage.Arguments[6]);
            Assert.AreEqual(1234567.890, transposedMessage.Arguments[7]);
            Assert.AreEqual(new Symbol("valid message"), transposedMessage.Arguments[8]);
            Assert.AreEqual('x', transposedMessage.Arguments[9]);
            Assert.AreEqual(new RGBA(20, 40, 60, 255), transposedMessage.Arguments[10]);
            Assert.AreEqual(new Midi(3, 110, 55, 66), transposedMessage.Arguments[11]);
            Assert.AreEqual(true, transposedMessage.Arguments[12]);
            Assert.AreEqual(false, transposedMessage.Arguments[13]);
            Assert.AreEqual(null, transposedMessage.Arguments[14]);
            Assert.AreEqual(double.PositiveInfinity, transposedMessage.Arguments[15]);
        }
    }
}