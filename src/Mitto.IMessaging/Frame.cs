using System;
using System.Linq;

namespace Mitto.IMessaging {
	/// <summary>
	/// Encapsulates and provides information about the data being transmitted
	/// 
	/// The frame looks like this:
	/// 
	/// ----------------------------------------------------------------------------------------
	/// | byte MessageType | byte message name length | byte[length] UTF-32 name | byte[] data |
	/// ----------------------------------------------------------------------------------------
	/// 
	/// </summary>
	public class Frame : IFrame {
		private byte[] _arrByteArray;
		public Frame(byte[] pData) {
			_arrByteArray = pData;
		}

		public Frame(MessageType pType, string pName, byte[] pData) {
			var arrName = System.Text.Encoding.UTF32.GetBytes(pName);
			_arrByteArray = new byte[2 + arrName.Length + pData.Length];

			_arrByteArray[0] = (byte)pType;
			_arrByteArray[1] = (byte)arrName.Length;
			arrName.CopyTo(_arrByteArray, 2);
			pData.CopyTo(_arrByteArray, 2 + arrName.Length);
		}
			
		public MessageType Type {
			get {
				return (MessageType)_arrByteArray.ElementAt(0);
			}
		}

		public string Name {
			get {
				byte bytLength = _arrByteArray.ElementAt(1);
				byte[] arrName = _arrByteArray.Skip(2).Take(bytLength).ToArray();
				string strName = System.Text.Encoding.UTF32.GetString(arrName);
				return strName;
			}
		}

		public byte[] Data {
			get {
				//Don't do this as it's slower
				/*return _arrByteArray.Skip(
					2 + _arrByteArray.ElementAt(1)
				).Take(
					_arrByteArray.Length - (2 + _arrByteArray.ElementAt(1))
				).ToArray();*/

				//-- this is faster
				var newArray = new byte[_arrByteArray.Length - (2 + _arrByteArray.ElementAt(1))];
				Array.Copy(
					_arrByteArray, // -- source array
					_arrByteArray.Length - newArray.Length,  // -- start index in the source
					newArray, // -- destination array
					0, // -- start index on the destination array
					newArray.Length // -- # bytes that needs to be read
				);
				return newArray;
			}
		}

		public byte[] GetByteArray() {
			return _arrByteArray;
		}
	}
}