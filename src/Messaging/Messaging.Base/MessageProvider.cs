using IMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Messaging.Base {
	public static class MessageProvider {
		private static Mutex _objTypeLock = new Mutex();
		private static List<Type> _lstTypes = new List<Type>();
		private static Dictionary<byte, List<KeyValuePair<MessageType, Type>>> _dicTypes = new Dictionary<byte, List<KeyValuePair<MessageType, Type>>>();
		private static Dictionary<string, SubscriptionHandler> _dicSubscriptionHandlers = new Dictionary<string, SubscriptionHandler>();

		public static List<Type> GetTypes() {
			lock (_objTypeLock) {
				if (!_lstTypes.Any()) {
					_lstTypes = GetAppTypes();
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Event"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Notification"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Request"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Response"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Subscribe"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".UnSubscribe"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Event"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Notification"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Request"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.Subscribe"));
					_lstTypes.AddRange(GetByNamespace(typeof(MessageProvider).Namespace + ".Action.UnSubscribe"));

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
							} else if (objType.IsSubclassOf(typeof(SubscriptionHandler))) {
								if (!_dicSubscriptionHandlers.ContainsKey(objType.Name)) {
									SubscriptionHandler objHandler = (SubscriptionHandler)Activator.CreateInstance(objType);
									_dicSubscriptionHandlers.Add(objType.Name, objHandler);
								}
							}
						}
					}
				}
			}
			return _lstTypes;
		}

		private static List<Type> GetAppTypes() {
			return MessagingFactory.GetMessageProvider().GetTypes();
		}

		private static List<Type> GetByNamespace(string pNamespace) {
			var lstTypes = new List<Type>();
			var lstAll = (from t in Assembly.GetExecutingAssembly().GetTypes()
						  where t.IsClass && t.Namespace == pNamespace
						  select t).ToList();

			foreach (Type objType in lstAll) {
				if (
					objType.IsSubclassOf(typeof(RequestMessage)) ||
					objType.IsSubclassOf(typeof(ResponseMessage)) ||
					objType.IsSubclassOf(typeof(NotificationMessage)) ||
					objType.IsSubclassOf(typeof(EventMessage)) ||
					objType.IsSubclassOf(typeof(SubscribeMessage)) ||
					objType.IsSubclassOf(typeof(SubscriptionHandler)) ||
					objType.IsSubclassOf(typeof(UnSubscribeMessage)) ||
					objType.Namespace.Contains(".Action") //is a generic type, easy solution is to just check the namespace string instead of  IsSubclassOf(typeof(Action.BaseAction<T>))
				) {
					lstTypes.Add(objType);
				}
			}
			return lstTypes;
		}

		private static Type GetResponseType(string pName) {
			Type objResponseType = typeof(Response.ACK); // -- default
			foreach (Type objType in GetTypes()) {
				if (objType.IsSubclassOf(typeof(ResponseMessage)) && objType.Name == pName) {
					return objType;
				}
			}
			return objResponseType;
		}

		internal static Type GetActionType(IMessage pMessage) {
			foreach (Type objType in GetTypes()) {
				if (objType.Namespace.Contains(".Action." + pMessage.Type) && objType.Name == pMessage.Name) {
					return objType;
				}
			}
			return null;
		}

		public static Type GetType(MessageType pMessageType, byte pCode) {
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

		/// <summary>
		/// ToDo: Replace finding response type from Message to <T>, else we can't do Request<MyResponseMessage2>(new MyRequestMessage1())
		/// </summary>
		/// <param name="pMessage"></param>
		/// <param name="pCode"></param>
		/// <returns></returns>
		internal static ResponseMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode) {
			var objResponseType = GetResponseType(pMessage.Name);
			if (objResponseType != null) {
				var objResponse = ((ResponseMessage)Activator.CreateInstance(objResponseType, pMessage, pCode));
				return objResponse;
			}
			return null;
		}

		/*internal static SubscriptionHandler GetSubscriptionHandler(Message pMessage) {
			Contract.Ensures(Contract.Result<SubscriptionHandler>() != null);
			if (_dicSubscriptionHandlers.Count == 0) { GetTypes(); }
			if (_dicSubscriptionHandlers.ContainsKey(pMessage.Name + "Handler")) {
				return _dicSubscriptionHandlers[pMessage.Name + "Handler"];
			}
			return null;
		}*/
	}
}