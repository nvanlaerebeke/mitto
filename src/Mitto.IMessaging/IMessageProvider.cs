using System;
using System.Collections.Generic;

namespace Mitto.IMessaging {
	/// <summary>
	/// ToDo: should return less types and more actual class instances
	/// Examples: 
	/// - IResponseMessage GetResponse(pName, ....)
	/// - IMessage Get(pMessageType, pCode, pData)
	/// </summary>
	public interface IMessageProvider {
		List<Type> GetTypes();
		Type GetResponseType(string pName);
		Type GetType(MessageType pMessageType, byte pCode);
		IAction GetAction(IQueue.IQueue pClient, IMessage pMessage);
	}
}