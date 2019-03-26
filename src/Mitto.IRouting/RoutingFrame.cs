using System;
using System.Text;

namespace Mitto.IRouting {
	/// <summary>
	/// ToDo: see where source and destination id's are used
	/// if not used anywhere but rabbitmq it might be a good idea to split it off
	/// into it's own frame type
	/// </summary>
	public class RoutingFrame {
		private byte[] _arrByteArray;

		public RoutingFrame(byte[] data) { _arrByteArray = data; }
		public RoutingFrame(RoutingFrameType pType, string pRequestID, string pSourceID, string pDestinationID, byte[] pData) {
			var arrRequestID = Encoding.ASCII.GetBytes(pRequestID);
			var arrSourceID = Encoding.ASCII.GetBytes(pSourceID);
			var arrDestinationID = Encoding.ASCII.GetBytes(pDestinationID);

			//Create new array with total length of the frame (see above)
			_arrByteArray = new byte[1 + 1 + arrRequestID.Length + 1 + arrSourceID.Length + 1 + arrDestinationID.Length + pData.Length];

			FrameType = pType;
			SetRequestID(arrRequestID);
			SetSourceID(arrSourceID);
			SetDestinationID(arrDestinationID);
			SetData(pData);
		}

		public RoutingFrameType FrameType {
			get { return (RoutingFrameType)_arrByteArray[0]; }
			set { _arrByteArray[0] = (byte)value; }
		}

		public string RequestID {
			get {
				var newArray = new byte[_arrByteArray[1]];
				Array.Copy(
					_arrByteArray,
					(1 + 1),
					newArray,
					0,
					newArray.Length
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public string SourceID {
			get {
				var newArray = new byte[
					_arrByteArray[
						1 + 1 + _arrByteArray[1] //After RequestID
					]
				];
				Array.Copy(
					_arrByteArray,
					(1 + 1 + _arrByteArray[1] + 1),
					newArray,
					0,
					newArray.Length
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public string DestinationID {
			get {
				var newArray = new byte[
					_arrByteArray[
						1 + 1 + _arrByteArray[1] + //After RequestID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1]] //After SourceID
					]];
				Array.Copy(
					_arrByteArray,
					(
						1 + 1 + _arrByteArray[1] + //After RequestID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1]] + //After SourceID
						1
					 ),
					newArray,
					0,
					newArray.Length
				);
				return Encoding.ASCII.GetString(newArray);
			}
		}

		public byte[] Data {
			get {
				var newArray = new byte[
					_arrByteArray.Length - 
					(
						1 + 1 + _arrByteArray[1] + // After RequestID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1]] + // After SourceID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1] + 1 + _arrByteArray[1 + 1 + _arrByteArray[1]]] // After DestinationID
					)
				];
				Array.Copy(
					_arrByteArray,
					(
						1 + 1 + _arrByteArray[1] + // After RequestID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1]] + // After SourceID
						1 + _arrByteArray[1 + 1 + _arrByteArray[1] + 1 + _arrByteArray[1 + 1 + _arrByteArray[1]]] // After DestinationID
					),
					newArray,
					0,
					newArray.Length
				);
				return newArray;
			}
		}

		public byte[] GetBytes() {
			return _arrByteArray;
		}

		private void SetRequestID(byte[] pData) {
			_arrByteArray[1] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1, pData.Length);
		}

		private void SetSourceID(byte[] pData) {
			_arrByteArray[1 + 1 + _arrByteArray[1]] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1 + _arrByteArray[1] + 1, pData.Length);
		}

		private void SetDestinationID(byte[] pData) {
			_arrByteArray[
				1 + 1 + _arrByteArray[1] + // After RequestID
				1 +	_arrByteArray[1 + 1 + _arrByteArray[1]] // After SourceID
			] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1 + _arrByteArray[1] + 1 + _arrByteArray[1 + 1 + _arrByteArray[1]] + 1, pData.Length);
		}

		private void SetData(byte[] pData) {
			Array.Copy(pData, 0, _arrByteArray, (_arrByteArray.Length - pData.Length), pData.Length);
		}
	}
}