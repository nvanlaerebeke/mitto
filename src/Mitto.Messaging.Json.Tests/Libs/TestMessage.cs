﻿using Mitto.IMessaging;
using Mitto.IRouting;

namespace Mitto.Messaging.Json.Tests.Libs {
	public class TestMessage : IMessage {
		public string ID => "MyID";

		public string Name => "TestMessage";

		public MessageType Type => MessageType.Request;
	}
}
