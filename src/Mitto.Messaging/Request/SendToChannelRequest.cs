﻿using Mitto.Messaging.Response;

namespace Mitto.Messaging.Request {
	public class SendToChannelRequest : RequestMessage, ISendToChannelRequest {
		public string ChannelName { get; set; }
		public string Message { get; set; }

		public SendToChannelRequest(string pName, string pMessage) {
			ChannelName = pName;
			Message = pMessage;
		}
	}
}