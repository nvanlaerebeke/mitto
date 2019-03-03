﻿using Mitto.IMessaging;
using Mitto.Messaging.Action.SubscriptionHandler;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;

namespace Mitto.Messaging.Action.Request {
	public class SendToChannelRequestAction : RequestAction<SendToChannelRequest, ACKResponse> {
		public SendToChannelRequestAction(IClient pClient, SendToChannelRequest pMessage) : base(pClient, pMessage) { }

		public override IResponseMessage Start() {
			var obj = MessagingFactory.Provider.GetSubscriptionHandler<IChannelSubscriptionHandler>();
			if (obj.Notify(Client, Request)) {
				return new ACKResponse(Request);
			}
			return new ACKResponse(Request, new ResponseStatus(ResponseState.Error, $"Failed to send message to channel {Request.ChannelName}"));
		}
	}
}