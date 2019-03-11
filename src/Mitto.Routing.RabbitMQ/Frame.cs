using System;
using System.Linq;
using System.Text;
using Mitto.IMessaging;

namespace Mitto.Routing.RabbitMQ {
	/// <summary>
	/// Encapsulates and provides information about the data being transmitted
	/// 
	/// The frame looks like this:
	/// 
	/// ------------------------------------------------------------------------------------------------------------------------------
	/// | byte MessageType | byte client id  length| byte[length] client id | byte message id  length| byte[length] id | byte[] data |
	/// ------------------------------------------------------------------------------------------------------------------------------
	/// 
	/// </summary>
	public class Frame {
		private byte[] _arrByteArray;

		public Frame(string pClientID, string pMessageID, byte[] data) {
			var arrQueueID = Encoding.ASCII.GetBytes(pClientID);
			var arrMessageID = Encoding.ASCII.GetBytes(pMessageID);

			//Create new array with total length of the frame (see above)
			_arrByteArray = new byte[1 + 1 + arrQueueID.Length + 1 + arrMessageID.Length + data.Length ];
			_arrByteArray[0] = data.ElementAt(0); // message type
			_arrByteArray[1] = (byte)arrQueueID.Length; // client id length
			Array.Copy(arrQueueID, 0, _arrByteArray, 2, arrQueueID.Length); // add clientid itself
			_arrByteArray[arrQueueID.Length + 2] = (byte)arrMessageID.Length; // message id Length
			Array.Copy(arrMessageID, 0, _arrByteArray, (3 + arrQueueID.Length), arrMessageID.Length); // add message id itself
			Array.Copy(data, 1, _arrByteArray, (3 + arrQueueID.Length + arrMessageID.Length), data.Length - 1);
		}

		public Frame(byte[] data) {
			_arrByteArray = data;
		}

		public MessageType MessageType {
			get {
				return (MessageType)_arrByteArray.ElementAt(0);
			}
		}

		public string QueueID {
			get {
				var newArray = new byte[_arrByteArray.ElementAt(1)];
				Array.Copy(
					_arrByteArray, // -- source array
					2,  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public string MessageID {
			get {
				var newArray = new byte[_arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))];
				Array.Copy(
					_arrByteArray, // -- source array
					2 + _arrByteArray.ElementAt(1) + 1,  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public byte[] Data {
			get {
				var singles = 3;
				var queueid = _arrByteArray.ElementAt(1);
				var messageid = _arrByteArray.ElementAt(2 + queueid);
				var newArray = new byte[(_arrByteArray.Length - (singles + queueid + messageid))];

				newArray[0] = _arrByteArray[0];
				Array.Copy(
					_arrByteArray, // -- source array
					(singles + queueid + messageid),  // -- start index in the source
					newArray, // -- destination array
					1, // -- start index on the destination array
					newArray.Length - 1 // -- # bytes that needs to be read
				);
				return newArray;
			}
		}

		public byte[] GetBytes() {
			return _arrByteArray;
		}
	}
}