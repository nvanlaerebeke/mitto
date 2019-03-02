using Mitto.IMessaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Tests")]
namespace Mitto.Messaging {
	/// <summary>
	/// Provides the Messages and Actions for the application
	/// 
	/// The messages are expected to have the following structure in the given namespace
	///     - Action
	///         - Notification
	///         - Request
	///     - Message
	///         - Notification
	///         - Request
	///         - Response
	///         
	/// Examples:
	///     <Namespace>.Request.Echo
	///     <Namespace>.Response.Echo
	///     <Namespace>.Action.Request.Echo
	///     
	/// 
	/// ToDo: Move messages under Mitto.Messaging.Message so that it's uniform with actions
	/// </summary>
	public class MessageProvider : IMessageProvider {
		/// <summary>
		/// The available IMessage classes
		/// </summary>
		internal Dictionary<MessageType, Dictionary<string, Type>> Types { get; } = new Dictionary<MessageType, Dictionary<string, Type>>();
		/// <summary>
		/// The available IAction classes
		/// </summary>
		internal Dictionary<MessageType, Dictionary<string, Type>> Actions { get; } = new Dictionary<MessageType, Dictionary<string, Type>>();

		/// <summary>
		/// Loads the types present in the specified Namespace
		/// The build-in types will be automatically added first
		/// 
		/// It is possible to overwrite the build-in message types and actions by your 
		/// own implementation when creating the class in your own namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		public MessageProvider(string pNamespace) {
			if(pNamespace != typeof(MessageProvider).Namespace) {
				LoadTypes(typeof(MessageProvider).Namespace);
			}
			LoadTypes(pNamespace);
		}

		/// <summary>
		/// Loads the base types only
		/// </summary>
		public MessageProvider() {
			LoadTypes(typeof(MessageProvider).Namespace);
		}

		/// <summary>
		/// Loads the types based on the provided namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		private void LoadTypes(string pNamespace) {
			//Messages
			foreach (var objType in GetByNamespace(pNamespace + ".Request")) {
				AddMessageType(MessageType.Request, objType);
			}
			foreach (var objType in GetByNamespace(pNamespace + ".Response")) {
				AddMessageType(MessageType.Response, objType);
			}
			foreach (var objType in GetByNamespace(pNamespace + ".Notification")) {
				AddMessageType(MessageType.Notification, objType);
			}
			//Actions
			foreach (var objType in GetByNamespace(pNamespace + ".Action.Notification")) {
				AddActionType(MessageType.Notification, objType);
			}
			foreach (var objType in GetByNamespace(pNamespace + ".Action.Request")) {
				AddActionType(MessageType.Request, objType);
			}
		}

		/// <summary>
		/// Adds the provided IMessage type to the cached types
		/// 
		/// Messages must implement the IMessage interface and may not be abstract classes
		///
		/// When passing a message with a name that already exists it will not overwrite 
		/// the already existing cache. 
		/// MessageType/Name combination should be unique
		/// </summary>
		/// <param name="pMessageType"></param>
		/// <param name="pType"></param>
		private void AddMessageType(MessageType pMessageType, Type pType) {
			if (pType.IsAbstract || !pType.GetInterfaces().Contains(typeof(IMessage))) { return; }

			if (!Types.ContainsKey(pMessageType)) {
				Types.Add(pMessageType, new Dictionary<string, Type> { { pType.Name, pType } });
			} else {
				if (!Types[pMessageType].ContainsKey(pType.Name)) {
					Types[pMessageType].Add(pType.Name, pType);
				}
			}
		}

		/// <summary>
		/// Adds the provided IAction type to the cached types
		/// 
		/// Actions must implement the IRequestAction or INotification interfaces
		/// 
		/// When passing an action with a name that already exists it will not overwrite 
		/// the already existing cache. 
		/// MessageType/Name combination should be unique
		/// </summary>
		/// <param name="pMessageType"></param>
		/// <param name="pType"></param>
		private void AddActionType(MessageType pMessageType, Type pType) {
			if (pType.IsAbstract || !pType.GetInterfaces().Contains(typeof(IAction))) { return; }

			if (!Actions.ContainsKey(pMessageType)) {
				Actions.Add(pMessageType, new Dictionary<string, Type> { { pType.Name, pType } });
			} else {
				if (!Actions[pMessageType].ContainsKey(pType.Name)) {
					Actions[pMessageType].Add(pType.Name, pType);
				}
			}
		}

		/// <summary>
		/// Gets all Request/Response/Notification messages and the Actions from the provided namespace
		/// </summary>
		/// <param name="pNamespace"></param>
		/// <returns>List<Type></returns>
		private List<Type> GetByNamespace(string pNamespace) {
			var lstTypes = new List<Type>();

			//Loop over all loaded assemblies so no classes are missed
			foreach (
				var ass in AppDomain.CurrentDomain.GetAssemblies()
			) {
				foreach (
					//foreach loaded assembly, get all it's types that match the given namespace
					var objType in (from t in ass.GetTypes() where t.IsClass && t.Namespace == pNamespace select t)
				) {
					if (
						objType.IsSubclassOf(typeof(RequestMessage)) ||
						objType.IsSubclassOf(typeof(ResponseMessage)) ||
						objType.IsSubclassOf(typeof(NotificationMessage)) ||
						objType.Namespace.Contains(".Action") //is a generic type, easy solution is to just check the namespace string instead of  IsSubclassOf(typeof(Action.BaseAction<T>))
					) {
						lstTypes.Add(objType);
					}
				}
			}
			return lstTypes;
		}

		/// <summary>
		/// Gets an IMessage based on it's data (byte array)
		/// See the Frame object for more information on what the bytes are represent
		/// </summary>
		/// <param name="pData"></param>
		/// <returns>IMessage</returns>
		public IMessage GetMessage(byte[] pData) {
			var objFrame = new Frame(pData);
			if (
				Types.ContainsKey(objFrame.Type) &&
				Types[objFrame.Type].ContainsKey(objFrame.Name)
			) {
				return MessagingFactory.Converter.GetMessage(Types[objFrame.Type][objFrame.Name], objFrame.Data);
			}
			return null;
		}

		/// <summary>
		/// Returns the response for a specific request message with the provided response code
		/// 
		/// When no specific response class can be found, the response will be a Response.ACK
		/// 
		/// 
		/// ToDo: will have to convert the enum to an actual int so that the application can pass it's errors and 
		/// so that there can be more than 255 return codes
		/// </summary>
		/// <param name="pMessage"></param>
		/// <param name="pCode"></param>
		/// <returns></returns>
		public IResponseMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode) {
			Type objResponseType = typeof(Response.ACK); // -- default
			if (
				Types.ContainsKey(MessageType.Response) &&
				Types[MessageType.Response].ContainsKey(pMessage.Name)
			) {
				objResponseType = Types[MessageType.Response][pMessage.Name];
			}
			return ((IResponseMessage)Activator.CreateInstance(objResponseType, pMessage, pCode));
		}

		/// <summary>
		/// Gets the IAction for a specific IMessage (Request/Notification)
		/// </summary>
		/// <param name="pClient"></param>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public IAction GetAction(IClient pClient, IMessage pMessage) {
			if (
				Actions.ContainsKey(pMessage.Type) &&
				Actions[pMessage.Type].ContainsKey(pMessage.Name)
			) {
				var type = (Actions[pMessage.Type][pMessage.Name]);
				var obj = Activator.CreateInstance(Actions[pMessage.Type][pMessage.Name], pClient, pMessage);
				return (IAction)obj;
			}
			return null;
		}

		/// <summary>
		/// Gets the byte[] representation for the given IMessage
		/// 
		/// Note: uses the configured IMessageConverter
		/// See frame documentation on how a frame should look
		/// </summary>
		/// <param name="pMessage"></param>
		/// <returns></returns>
		public byte[] GetByteArray(IMessage pMessage) {
			return new Frame(
				pMessage.Type, 
				pMessage.Name, 
				MessagingFactory.Converter.GetByteArray(pMessage)
			).GetByteArray();
		}
	}
}