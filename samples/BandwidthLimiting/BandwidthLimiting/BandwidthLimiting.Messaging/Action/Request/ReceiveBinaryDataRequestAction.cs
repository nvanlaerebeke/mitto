using BandwidthLimiting.Messaging.Request;
using Mitto.IMessaging;
using Mitto.Messaging.Action;
using Mitto.Messaging.Response;

namespace BandwidthLimiting.Messaging.Action.Request {
    public class ReceiveBinaryDataRequestAction : RequestAction<ReceiveBinaryDataRequest, ACKResponse> {
        public ReceiveBinaryDataRequestAction(IClient pClient, ReceiveBinaryDataRequest pRequest) : base(pClient, pRequest) { }

        public override ACKResponse Start() {
            return new ACKResponse();
        }
    }
}
