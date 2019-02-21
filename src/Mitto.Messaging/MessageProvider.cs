using Mitto.IMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Mitto.Messaging {
	public class MessageProvider : IMessageProvider {
		private static Mutex _objTypeLock = new Mutex();
		private static List<Type> _lstTypes = new List<Type>();
		private static Dictionary<byte, List<KeyValuePair<MessageType, Type>>> _dicTypes = new Dictionary<byte, List<KeyValuePair<MessageType, Type>>>();

		public MessageProvider() { }

		public List<Type> GetTypes() {
			lock (_objTypeLock) {
				if (!_lstTypes.Any()) {
					_lstTypes = GetAppTypes();
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Notification"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Request"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Response"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Notification"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Request"));
					lock (_dicTypes) {
						foreach (Type objType in _lstTypes) {
							if (objType.IsSubclassOf(typeof(Message)) && !objType.IsAbstract) {
								Message objMessage = (Message)Activator.CreateInstance(objType);
								if (objMessage != null) {
									var key = objMessage.GetCode();

									//Add to the list of messages
									if (!_dicTypes.ContainsKey(key)) {
										_dicTypes.Add(key, new List<KeyValuePair<MessageType, Type>> { { new KeyValuePair<MessageType, Type>(objMessage.Type, objType) } });
									} else {
										var obj = new KeyValuePair<MessageType, Type>(objMessage.Type, objType);
										if (!_dicTypes[key].Contains(obj)) {
											_dicTypes[key].Add(obj);
										}
									}
								} else {
									//error
								}
							}
						}
					}
				}
			}
			return _lstTypes;
		}

		private List<Type> GetAppTypes() {
			return MessagingFactory.Provider.GetTypes();
		}

		private List<Type> GetByNamespace(string pNamespace) {
			var lstTypes = new List<Type>();
			var lstAll = (from t in Assembly.GetExecutingAssembly().GetTypes()
						  where t.IsClass && t.Namespace == pNamespace
						  select t).ToList();

			foreach (Type objType in lstAll) {
				if (
					objType.IsSubclassOf(typeof(RequestMessage)) ||
					objType.IsSubclassOf(typeof(ResponseMessage)) ||
					objType.IsSubclassOf(typeof(NotificationMessage)) ||
					objType.Namespace.Contains(".Action") //is a generic type, easy solution is to just check the namespace string instead of  IsSubclassOf(typeof(Action.BaseAction<T>))
				) {
					lstTypes.Add(objType);
				}
			}
			return lstTypes;
		}

		public Type GetResponseType(string pName) {
			Type objResponseType = typeof(Response.ACK); // -- default
			foreach (Type objType in GetTypes()) {
				if (objType.IsSubclassOf(typeof(ResponseMessage)) && objType.Name == pName) {
					return objType;
				}
			}
			return objResponseType;
		}

		public IAction GetAction(IQueue.IQueue pClient, IMessage pMessage) {
			foreach (Type objType in GetTypes()) {
				if (objType.Namespace.Contains(".Action." + pMessage.Type) && objType.Name == pMessage.Name) {
					return (IAction)Activator.CreateInstance(objType, pClient, pMessage);
				}
			}
			return null;
		}

		public Type GetType(MessageType pMessageType, byte pCode) {
			if (_dicTypes.Count == 0) { GetTypes(); }

			if (_dicTypes.ContainsKey(pCode)) {
				foreach (KeyValuePair<MessageType, Type> kvpType in _dicTypes[pCode]) {
					if (kvpType.Key == pMessageType) {
						return kvpType.Value;
					}
				}
			}
			return null;
		}
	}
}