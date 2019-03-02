﻿using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
	public abstract class RequestAction<T> : BaseAction<T>, IRequestAction where T : IMessage {
		public RequestAction(IClient pClient, IMessage pRequest) : base(pClient, pRequest) { }
		public abstract IResponseMessage Start();
	}
}
