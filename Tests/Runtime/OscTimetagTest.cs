using System;
using Artcom.OpenSoundControl.Library;
using NUnit.Framework;

namespace Artcom.OpenSoundControl.Editor.Tests {
    public class OscTimetagTest {
        [Test]
        public void TestTimetagParsing() {
            var time = 60 * (ulong) 60 * 24 * 365 * 108;
            time = time << 32;
            time = time + (ulong) (Math.Pow(2, 32) / 2);
            var date = Utils.TimetagToDateTime(time);

            Assert.AreEqual(DateTime.Parse("2007-12-06 00:00:00.500"), date);
        }

        [Test]
        public void TestDateTimeToTimetag() {
            var dt = DateTime.Now;

            var l = Utils.DateTimeToTimetag(dt);
            var dtBack = Utils.TimetagToDateTime(l);

            Assert.AreEqual(dt.Date, dtBack.Date);
            Assert.AreEqual(dt.Hour, dtBack.Hour);
            Assert.AreEqual(dt.Minute, dtBack.Minute);
            Assert.AreEqual(dt.Second, dtBack.Second);
            Assert.AreEqual(dt.Millisecond, dtBack.Millisecond);
        }
    }
}