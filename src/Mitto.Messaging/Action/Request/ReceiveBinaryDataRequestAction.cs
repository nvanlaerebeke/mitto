using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using System;

namespace Mitto.Messaging.Action.Request {
	public class ReceiveBinaryDataRequestAction : RequestAction<ReceiveBinaryDataRequest, ACKResponse> {
		public ReceiveBinaryDataRequestAction(IClient pClient, ReceiveBinaryDataRequest pRequest) : base(pClient, pRequest) { }

		public override ACKResponse Start() {
			Console.WriteLine("Received Data");
			return new ACKResponse(Request);
		}
	}
}