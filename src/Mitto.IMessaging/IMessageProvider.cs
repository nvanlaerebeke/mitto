using System;
using System.Collections.Generic;

namespace Mitto.IMessaging {
	public interface IMessageProvider {
		List<Type> GetTypes();
		Type GetResponseType(string pName);
		Type GetType(MessageType pMessageType, byte pCode);
		Type GetActionType(IMessage pMessage);
	}
}