using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BandwidthLimiting.Messaging.Request {
    public class ReceiveBinaryDataRequest : Mitto.Messaging.RequestMessage {
        public byte[] Data { get; set; }

        public ReceiveBinaryDataRequest() { }
        public ReceiveBinaryDataRequest(byte[] pData) {
            Data = pData;
        }
    }
}
