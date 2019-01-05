using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IMessaging;
using Messaging.Base;

namespace Messaging.App {
	public class AppMessageProvider : IMessageProvider {
		private List<Type> _lstCustomTypes = new List<Type>();

		public List<Type> GetTypes() {
			lock (_lstCustomTypes) {
				if (_lstCustomTypes.Count == 0) {
					_lstCustomTypes.Clear();
					string strAssemblyNamespace = "Messaging.App";
					foreach (string strNamespace in new string[] {
							strAssemblyNamespace + ".Event",
							strAssemblyNamespace + ".Notification",
							strAssemblyNamespace + ".Request",
							strAssemblyNamespace + ".Response",
							strAssemblyNamespace + ".Subscribe",
							strAssemblyNamespace + ".UnSubscribe",
							strAssemblyNamespace + ".Action.Event",
							strAssemblyNamespace + ".Action.Notification",
							strAssemblyNamespace + ".Action.Request",
							strAssemblyNamespace + ".Action.Subscribe",
							strAssemblyNamespace + ".Action.UnSubscribe",

					}) {
						var lstAll = (from t in Assembly.GetExecutingAssembly().GetTypes()
									  where t.IsClass && t.Namespace == strNamespace
									  select t).ToList();

						foreach (Type objType in lstAll) {
							if (
								objType.IsSubclassOf(typeof(RequestMessage)) ||
								objType.IsSubclassOf(typeof(ResponseMessage)) ||
								objType.IsSubclassOf(typeof(NotificationMessage)) ||
								objType.IsSubclassOf(typeof(EventMessage)) ||
								objType.IsSubclassOf(typeof(SubscribeMessage)) ||
								objType.IsSubclassOf(typeof(UnSubscribeMessage)) ||
								objType.IsSubclassOf(typeof(SubscriptionHandler)) ||
								objType.Namespace.Contains(strAssemblyNamespace + ".Action.") //objType.IsSubclassOf(typeof(WebSocketMessaging.Action.BaseAction)) <= generic type, this is easier
							) {
								_lstCustomTypes.Add(objType);
							}
						}
					}
				}
				return _lstCustomTypes;
			}
		}
	}
}