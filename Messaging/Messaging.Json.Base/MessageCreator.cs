using IMessaging;
using Messaging.Base;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Messaging.Json {
	public class MessageCreator : IMessageCreator {

		public IMessage Create(byte[] pData) {
			Console.WriteLine("Creating Message");
			try {
				List<byte> lstData = new List<byte>(pData);

				MessageFormat enuFormat = (MessageFormat)lstData.First<byte>();
				Console.WriteLine("Detected message format " + enuFormat.ToString());
				lstData.RemoveAt(0);

				MessageType enuType = (MessageType)lstData.First<byte>();
				Console.WriteLine("Detected message type " + enuType.ToString());
				lstData.RemoveAt(0);

				Type type = MessageProvider.GetType(enuType, lstData.First<byte>());
				lstData.RemoveAt(0);

				if (type != null) {
					IMessage objMessage;
					if (enuFormat == MessageFormat.Bson) {
						using (MemoryStream ms = new MemoryStream(lstData.ToArray())) {
							using (BsonReader reader = new BsonReader(ms)) {
								objMessage = new JsonSerializer().Deserialize(reader, type) as IMessage;
							}
						}
					} else {
						string json = System.Text.Encoding.UTF8.GetString(lstData.ToArray());
						Console.WriteLine("Recieved Message");
						Console.WriteLine(json);
						objMessage = JsonConvert.DeserializeObject(json, type) as Message;
					}
					return objMessage;
				}
			} catch (Exception ex) {
				Console.WriteLine("Failed to create message, ignoring");
				Console.WriteLine(ex);
			}
			return null;
		}

		public byte[] GetBytes(IMessage pMessage) {
			if (MessageFormat.Json == MessageFormat.Bson) {
				using (MemoryStream stream = new MemoryStream()) {
					stream.Write(new byte[2] { (byte)pMessage.Type, (byte)pMessage.GetCode() }, 0, 2);
					using (BsonWriter writer = new BsonWriter(stream)) {
						new JsonSerializer().Serialize(writer, this);
						return stream.ToArray();
					}
				}
			} else {
				var strJson = JsonConvert.SerializeObject(pMessage);
				Console.WriteLine("Creating Message Json:");
				Console.WriteLine(strJson);

				List<byte> lstRawMessage = new List<byte>();
				lstRawMessage.Add((byte)MessageFormat.Json);
				lstRawMessage.Add((byte)pMessage.Type);
				lstRawMessage.Add((byte)pMessage.GetCode());
				lstRawMessage.AddRange(System.Text.Encoding.UTF8.GetBytes(strJson));
				return lstRawMessage.ToArray();
			}
		}
	}
}
