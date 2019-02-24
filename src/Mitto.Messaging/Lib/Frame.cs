using Mitto.IMessaging;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging {
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
	internal class Frame : IFrame {
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
				return _arrByteArray.Skip(2 + _arrByteArray.ElementAt(1)).Take(_arrByteArray.Length - (2 + _arrByteArray.ElementAt(1))).ToArray();
			}
		}

		public byte[] GetByteArray() {
			return _arrByteArray;
		}
	}
}