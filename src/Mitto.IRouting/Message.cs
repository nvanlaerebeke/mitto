﻿using System.Linq;

namespace Mitto.IRouting {
	public class Message : System.IEquatable<Message> {
		/// <summary>
		/// Represends where the data is being sent to
		/// 
		/// ToDo: need a better name then ClientID 
		///       This is not always the client id, more like the 'who should i send this data to' id
		///       in case of rabbitmq, clientid is used as the 'sender queue name'
		///       
		///       It gets a little confusing when using the message6 between the connection, queuing and then message processing system
		/// 
		///       Best to check all the references and list out how it's used to find a good description
		///       
		/// ToDo: Make Message an IMessage interface and move the message class to the location where the implementation belongs
		/// </summary>
		public string ClientID { get; private set; }
		
		/// <summary>
		/// Represends the message being send 
		/// </summary>
		public byte[] Data { get; private set; }

		public Message(string pClientID, byte[] pData) {
			ClientID = pClientID;
			Data = pData;
		}

		public bool Equals(Message pObj) {
			return (
				this == pObj || //compare reference
				(
					ClientID.Equals(pObj.ClientID) && 
					Data.SequenceEqual(pObj.Data)
				)
			);
		}
	}
}