using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Artcom.OpenSoundControl.Library {
    public class OscBundle : OscPacket {
        private Timetag timetag;

        private List<OscMessage> messages;

        public OscBundle(ulong timetag, params OscMessage[] args) {
            this.timetag = new Timetag(timetag);
            messages = new List<OscMessage>();
            messages.AddRange(args);
        }

        public ulong Timetag {
            get { return timetag.tag; }
            set { timetag = new Timetag(value); }
        }

        public DateTime Timestamp {
            get { return timetag.Timestamp; }
            set { timetag.Timestamp = value; }
        }

        public IList<OscMessage> Messages {
            get { return messages.AsReadOnly(); }
            private set { messages = value.ToList(); }
        }

        public override byte[] GetBytes() {
            const string bundle = "#bundle";
            var bundleTagLen = Utils.AlignedStringLength(bundle);
            var tag = Set(timetag.tag);

            var outMessages = new List<byte[]>();
            foreach(var msg in messages) {
                outMessages.Add(msg.GetBytes());
            }

            var len = bundleTagLen + tag.Length + outMessages.Sum(x => x.Length + 4);

            var i = 0;
            var output = new byte[len];
            Encoding.ASCII.GetBytes(bundle).CopyTo(output, i);
            i += bundleTagLen;
            tag.CopyTo(output, i);
            i += tag.Length;

            foreach(var msg in outMessages) {
                var size = SetInt(msg.Length);
                size.CopyTo(output, i);
                i += size.Length;

                msg.CopyTo(output, i);
                i += msg.Length; // msg size is always a multiple of 4
            }

            return output;
        }
    }
}