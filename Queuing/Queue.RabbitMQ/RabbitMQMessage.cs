using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Queue.RabbitMQ {
	public abstract class RabbitMQMessage {
		internal RabbitMQMessageFormat Format { get; }

		public string ClientID { get; }
		public string QueueID { get; }
		public byte[] Data { get; }

		public RabbitMQMessage(byte[] pData) {
			List<byte> lstData = new List<byte>(pData);

			Format = (RabbitMQMessageFormat)lstData.First<byte>();
			lstData.RemoveAt(0);

			byte bytQueueIDLength = lstData.First<byte>();
			lstData.RemoveAt(0);
			QueueID = Encoding.UTF8.GetString(lstData.GetRange(0, bytQueueIDLength).ToArray());
			lstData.RemoveRange(0, bytQueueIDLength);

			byte bytClientIDLength = lstData.First<byte>();
			lstData.RemoveAt(0);
			ClientID = Encoding.UTF8.GetString(lstData.GetRange(0, bytClientIDLength).ToArray());
			lstData.RemoveRange(0, bytClientIDLength);

			Data = lstData.ToArray();
		}

		public RabbitMQMessage(string pQueueID, string pClientID, RabbitMQMessageFormat pFormat, byte[] pData) {
			QueueID = pQueueID;
			ClientID = pClientID;

			Format = pFormat;
			Data = pData;
		}

		/// <summary>
		/// Returns the message in it's bytes representation
		/// A RabbitMQMessage exists of
		///		- byte: RabbitMQMessageFormat
		///		- byte: QueueID size, number of bytes the QueueID name(string) is
		///		- byte[]: QueueID, a range of bytes, length specified by the QueueID number of bytes
		///		- byte: ClientID size, number of bytes the ClientID name(string) is
		///		- byte[]: ClientID, a range of bytes, length specified by the ClientID number of bytes
		///		- byte[]: Data, starting here all the other bytes are the data being transmitted 
		/// </summary>
		/// <returns></returns>
		public byte[] GetBytes() {
			var arrQueueID = Encoding.UTF8.GetBytes(QueueID);
			var arrClientID = Encoding.UTF8.GetBytes(ClientID);

			List<byte> lstData = new List<byte>();
			lstData.Add((byte)Format);
			lstData.Add((byte)arrQueueID.Length);
			lstData.AddRange(arrQueueID);
			lstData.Add((byte)arrClientID.Length);
			lstData.AddRange(arrClientID);
			lstData.AddRange(Data);
			return lstData.ToArray();
		}
    }
}
