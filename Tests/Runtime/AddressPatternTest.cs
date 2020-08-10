using Artcom.OpenSoundControl.Library;
using NUnit.Framework;

namespace Artcom.OpenSoundControl.Editor.Tests {
    public class AddressPatternTest {
        [Test]
        public void MatchSimple() {
            var pattern = new AddressPattern("/foo/bar");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsFalse(pattern.IsMatch("/foo/bAr"));
        }

        [Test]
        public void MatchAny() {
            var pattern = new AddressPattern("/*");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/bar/baz"));
            Assert.IsFalse(pattern.IsMatch("foo"));
            Assert.IsTrue(pattern.IsMatch("/maxbrew"));
        }

        [Test]
        public void MatchSingleAny() {
            var pattern = new AddressPattern("/foo/b?r");
            Assert.IsTrue(pattern.IsMatch("/foo/bAr"));
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/bor"));
            Assert.IsFalse(pattern.IsMatch("/foo/bahr"));
        }

        [Test]
        public void MatchCharacterGroup() {
            var pattern = new AddressPattern("/foo/b[aAeiou]r");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/bAr"));
            Assert.IsTrue(pattern.IsMatch("/foo/ber"));
            Assert.IsTrue(pattern.IsMatch("/foo/bir"));
            Assert.IsTrue(pattern.IsMatch("/foo/bor"));
            Assert.IsTrue(pattern.IsMatch("/foo/bur"));
            Assert.IsFalse(pattern.IsMatch("/foo/bEr"));
        }

        [Test]
        public void MatchCharacterGroupRange() {
            var pattern = new AddressPattern("/foo/b[a-z]r");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/bbr"));
            Assert.IsTrue(pattern.IsMatch("/foo/bcr"));
            Assert.IsTrue(pattern.IsMatch("/foo/bxr"));
            Assert.IsTrue(pattern.IsMatch("/foo/byr"));
            Assert.IsTrue(pattern.IsMatch("/foo/bzr"));
            Assert.IsFalse(pattern.IsMatch("/foo/bAr"));
        }

        [Test]
        public void MatchCharacterGoupNegation() {
            var pattern = new AddressPattern("/foo/b[!Aeiou]r");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/bEr"));
            Assert.IsTrue(pattern.IsMatch("/foo/bzr"));
            Assert.IsFalse(pattern.IsMatch("/foo/bAr"));
            Assert.IsFalse(pattern.IsMatch("/foo/ber"));
            Assert.IsFalse(pattern.IsMatch("/foo/bir"));
            Assert.IsFalse(pattern.IsMatch("/foo/bor"));
            Assert.IsFalse(pattern.IsMatch("/foo/bur"));
        }

        [Test]
        public void MatchCharacterGroupNegation() {
            var pattern = new AddressPattern("/foo/{bar,baz}");
            Assert.IsTrue(pattern.IsMatch("/foo/bar"));
            Assert.IsTrue(pattern.IsMatch("/foo/baz"));
            Assert.IsFalse(pattern.IsMatch("/foo/Bar"));
            Assert.IsFalse(pattern.IsMatch("/foo/qux"));
        }

        [Test]
        public void MatchComplexPattern() {
            var pattern = new AddressPattern("/?o*/{ba,qu}[rzx]/[!void][a-z]r");
            Assert.IsTrue(pattern.IsMatch("/foot/bar/bar"));
            Assert.IsTrue(pattern.IsMatch("/bor/quz/zer"));
            Assert.IsTrue(pattern.IsMatch("/foo/qux/jar"));
            Assert.IsFalse(pattern.IsMatch("/ffo/bal/var"));
            Assert.IsFalse(pattern.IsMatch("/foo/dar/bar"));
            Assert.IsFalse(pattern.IsMatch("/foo/bar/var"));
            Assert.IsFalse(pattern.IsMatch("/foo/bar/vAr"));
        }
    }
}