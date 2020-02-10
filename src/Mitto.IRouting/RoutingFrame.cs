using System;
using System.Text;

namespace Mitto.IRouting {
    /// <summary>
    /// 
    /// Represents a frame that is a wrapper around the actual data
    /// The Frame consists in:
    /// 
    /// - FrameType
    /// - MessageType
    /// - RequestID
    /// - SourceID
    /// - DestinationID
    /// - Data
    /// 
    /// Uses ASCII encoding for the ID's as it's expected that these will be a GUID
    /// 
    /// ToDo: 
    ///     See where source and destination id's are used 
    ///     if not used anywhere but rabbitmq it might be a good idea to split it off
    ///     into it's own frame type and use a 'internal' frame type that's 
    ///     for communication inside the app
    ///     
    ///     Note that this would be a much cleaner division between  how
    ///     the Client <=> Server communicates and how the Server-Connection <=> Server-Messaging communicates.
    ///     Would also be much more flexible when implementing new ways the internal routing is done and
    ///     can be optimized for that. For example a passthrough router needs no framing, while rabbitmq does
    ///     
    /// </summary>
    public class RoutingFrame {
		private byte[] _arrByteArray;

		public RoutingFrame(byte[] data) { _arrByteArray = data; }
        
        /// <summary>
        /// Constructs the RoutingFrame based on the given parameters
        /// </summary>
        /// <param name="pFrameType"></param>
        /// <param name="pMessageType"></param>
        /// <param name="pRequestID"></param>
        /// <param name="pSourceID"></param>
        /// <param name="pDestinationID"></param>
        /// <param name="pData"></param>
		public RoutingFrame(RoutingFrameType pFrameType, MessageType pMessageType, string pRequestID, string pSourceID, string pDestinationID, byte[] pData) {
			var arrRequestID = Encoding.ASCII.GetBytes(pRequestID);
			var arrSourceID = Encoding.ASCII.GetBytes(pSourceID);
			var arrDestinationID = Encoding.ASCII.GetBytes(pDestinationID);

			//Create new array with total length of the frame (see above)
			_arrByteArray = new byte[1 + 1 + 1 + arrRequestID.Length + 1 + arrSourceID.Length + 1 + arrDestinationID.Length + pData.Length];

			FrameType = pFrameType;
			MessageType = pMessageType;
			SetRequestID(arrRequestID);
			SetSourceID(arrSourceID);
			SetDestinationID(arrDestinationID);
			SetData(pData);
		}

		public RoutingFrameType FrameType {
			get { return (RoutingFrameType)_arrByteArray[0]; }
			set { _arrByteArray[0] = (byte)value; }
		}
		public MessageType MessageType {
			get { return (MessageType)_arrByteArray[1]; }
			set { _arrByteArray[1] = (byte)value; }
		}

		public string RequestID {
			get {
				var newArray = new byte[_arrByteArray[2]];
				Array.Copy(
					_arrByteArray,
					(1 + 1 + 1),
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
						1 + 1 + 1 + _arrByteArray[2] //After RequestID
					]
				];
				Array.Copy(
					_arrByteArray,
					(1 + 1 + 1 + _arrByteArray[2] + 1),
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
						1 + 1 + 1 + _arrByteArray[2] + //After RequestID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]] //After SourceID
					]];
				Array.Copy(
					_arrByteArray,
					(
						1 + 1 + 1 + _arrByteArray[2] + //After RequestID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]] + //After SourceID
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
						1 + 1 + 1 + _arrByteArray[2] + // After RequestID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]] + // After SourceID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2] + 1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]]] // After DestinationID
					)
				];
				Array.Copy(
					_arrByteArray,
					(
						1 + 1 + 1 + _arrByteArray[2] + // After RequestID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]] + // After SourceID
						1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2] + 1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]]] // After DestinationID
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
			_arrByteArray[2] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1 + 1, pData.Length);
		}

		private void SetSourceID(byte[] pData) {
			_arrByteArray[1 + 1 + 1 + _arrByteArray[2]] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1 + 1 + _arrByteArray[2] + 1, pData.Length);
		}

		private void SetDestinationID(byte[] pData) {
			_arrByteArray[
				1 + 1 + 1 + _arrByteArray[2] + // After RequestID
				1 +	_arrByteArray[1 + 1 + 1 + _arrByteArray[2]] // After SourceID
			] = (byte)pData.Length;
			Array.Copy(pData, 0, _arrByteArray, 1 + 1 + 1 + _arrByteArray[2] + 1 + _arrByteArray[1 + 1 + 1 + _arrByteArray[2]] + 1, pData.Length);
		}

		private void SetData(byte[] pData) {
			Array.Copy(pData, 0, _arrByteArray, (_arrByteArray.Length - pData.Length), pData.Length);
		}
	}
}