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
		public IMessage GetMessage(Type pType, byte[] pData) {
			var objFrame = new Frame(pData);
			if (objFrame.Format == MessageFormat.Bson) {
				using (MemoryStream ms = new MemoryStream(objFrame.Data)) {
					using (BsonReader reader = new BsonReader(ms)) {
						return new JsonSerializer()
							.Deserialize(
								reader,
								pType
							) as IMessage;
					}
				}
			} else {
				return JsonConvert.DeserializeObject(
					Encoding.UTF32.GetString(objFrame.Data),
					pType
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
				Encoding.UTF32.GetBytes(
					JsonConvert.SerializeObject(pMessage)
				)
			).GetByteArray();
		}
	}
}
