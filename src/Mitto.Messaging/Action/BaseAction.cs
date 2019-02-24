﻿using Mitto.IMessaging;

namespace Mitto.Messaging.Action {
	/// <summary>
	/// ToDo: Improve the inheritence of IAction/INotificationAction and IRequestAction
	/// IAction should have an internal? Execute method and the INotification & IRequestAction
	/// their own Start method
	/// </summary>
	/// <typeparam name="T"></typeparam>
	 public abstract class BaseAction<T> : IAction where T : IMessage {
		protected T Request { get; private set; }

		protected IQueue.IQueue Client { private set; get; }

		public BaseAction(IQueue.IQueue pClient, IMessage pRequest) {
			Client = pClient;
			Request = (T)pRequest;
		}
	}
}