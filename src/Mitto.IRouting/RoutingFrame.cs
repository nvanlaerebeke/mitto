using System;
using System.Linq;
using System.Text;

namespace Mitto.IRouting {

	public class RoutingFrame {
		private byte[] _arrByteArray;

		public RoutingFrame(byte[] data) { _arrByteArray = data; }
		public RoutingFrame(RoutingFrameType pType, string pConnectionID, byte[] data) {
			var arrConnectionID = Encoding.ASCII.GetBytes(pConnectionID);

			//Create new array with total length of the frame (see above)
			_arrByteArray = new byte[1 + 1 + arrConnectionID.Length + data.Length];
			_arrByteArray[0] = (byte)pType; // RabbitMQ type
			_arrByteArray[1] = (byte)arrConnectionID.Length;
			Array.Copy(arrConnectionID, 0, _arrByteArray, 1 + 1, arrConnectionID.Length);
			Array.Copy(data, 0, _arrByteArray, (1 + 1 + arrConnectionID.Length), data.Length);
		}

		public RoutingFrameType FrameType {
			get { return (RoutingFrameType)_arrByteArray.ElementAt(0); }
		}

		public string ConnectionID {
			get {
				var newArray = new byte[_arrByteArray.ElementAt(1)];
				Array.Copy(
					_arrByteArray, // -- source array
					(1 + 1),  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public byte[] Data {
			get {
				var newArray = new byte[(
					_arrByteArray.Length -
					(
						1 +
						1 +
						_arrByteArray.ElementAt(1)
					)
				)];
				Array.Copy(
					_arrByteArray, // -- source array
					(1 + 1 + _arrByteArray.ElementAt(1)),  // -- start index in the source
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