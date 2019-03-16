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
	/// ---------------------------------------------------------------------------------------------
	/// | byte MessageType | byte client id  length| byte[length] id | byte[] IMessaging.Frame data |
	/// ---------------------------------------------------------------------------------------------
	/// 
	/// </summary>
	public class Frame {
		private byte[] _arrByteArray;

		public Frame(string pQueueID, string pConnectionID, byte[] data) {
			var arrQueueID = Encoding.ASCII.GetBytes(pQueueID);
			var arrConnectionID = Encoding.ASCII.GetBytes(pConnectionID);

			//Create new array with total length of the frame (see above)
			_arrByteArray = new byte[1 + 1 + arrQueueID.Length + 1 + + arrConnectionID.Length + data.Length ];
			_arrByteArray[0] = data.ElementAt(0); // message type
			_arrByteArray[1] = (byte)arrQueueID.Length; // client id length
			Array.Copy(arrQueueID, 0, _arrByteArray, 2, arrQueueID.Length); // add clientid itself
			_arrByteArray[1 + 1 + arrQueueID.Length] = (byte)arrConnectionID.Length;
			Array.Copy(arrConnectionID, 0, _arrByteArray, 1 + 1 + arrQueueID.Length + 1, arrConnectionID.Length);
			Array.Copy(data, 0, _arrByteArray, (1 + 1 + arrQueueID.Length + 1 + arrConnectionID.Length), data.Length);
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

		public string ConnectionID {
			get {
				var newArray = new byte[_arrByteArray.ElementAt((1 + 1 + _arrByteArray.ElementAt(1)))];
				Array.Copy(
					_arrByteArray, // -- source array
					(1 + 1 + _arrByteArray.ElementAt(1) + 1),  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public string MessageID {
			get {
				return new IMessaging.Frame(Data).ID;
			}
		}

		public byte[] Data {
			get {
				var newArray = new byte[(
					_arrByteArray.Length - 
					(
						1 + 
						1 + 
						_arrByteArray.ElementAt(1) +
						1 +
						_arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))
					)
				)];
				Array.Copy(
					_arrByteArray, // -- source array
					(1 + 1 + _arrByteArray.ElementAt(1) + 1 + _arrByteArray.ElementAt(1 + 1 + _arrByteArray.ElementAt(1))),  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return newArray;
			}
		}

		public byte[] GetBytes() {
			return _arrByteArray;
		}
	}
}