using Mitto.IMessaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging.Json {
	public class MessageConverter : IMessageConverter {
		/// <summary>
		/// Returns the IMessage represented by the byte array
		/// </summary>
		/// <param name="pData"></param>
		public IMessage GetMessage(byte[] pData) {
			var objFrame = new Frame(pData);
			if (objFrame.Format == MessageFormat.Bson) {
				using (MemoryStream ms = new MemoryStream(objFrame.Data)) {
					using (BsonReader reader = new BsonReader(ms)) {
						return new JsonSerializer()
							.Deserialize(
								reader,
								MessagingFactory.Provider.GetType(objFrame.Type, objFrame.Code)
							) as IMessage;
					}
				}
			} else {
				return JsonConvert.DeserializeObject(
					Encoding.UTF32.GetString(objFrame.Data),
					MessagingFactory.Provider.GetType(objFrame.Type, objFrame.Code)
				) as IMessage;
			}
		}

		/// <summary>
		/// Returns the Frame representation of the IMessage
		/// 
		/// ToDo: dynamically switch between json & bson based on message size
		/// this is due to that json becomes less performant to more bytes are 
		/// being serialized/deserialized 
		/// </summary>
		/// <param name="pMessage"></param>
		/// <returns>byte[]</returns>
		public byte[] GetByteArray(IMessage pMessage) {
			return new Frame(
				MessageFormat.Json,
				pMessage.Type,
				pMessage.GetCode(),
				Encoding.UTF32.GetBytes(
					JsonConvert.SerializeObject(pMessage)
				)
			).GetByteArray();
		}

		/// <summary>
		/// Creates a response message based on the IMessage and desired response code
		/// </summary>
		/// <param name="pMessage"></param>
		/// <param name="pCode"></param>
		/// <returns>IResponseMessage</returns>
		public IResponseMessage GetResponseMessage(IMessage pMessage, ResponseCode pCode) {
			var objResponseType = MessagingFactory.Provider.GetResponseType(pMessage.Name);
			if (objResponseType != null) {
				return ((IResponseMessage)Activator.CreateInstance(objResponseType, pMessage, pCode));
			}
			return null;
		}
	}
}
