using Mitto.IMessaging;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging.Json {
	internal class Frame : IFrame {
		private byte[] _arrByteArray;
		public Frame(byte[] pData) {
			_arrByteArray = pData;
		}

		public Frame(MessageFormat pFormat, MessageType pType, byte pCode, byte[] pData) {
			_arrByteArray = new byte[3 + pData.Length];
			_arrByteArray[0] = (byte)pFormat;
			_arrByteArray[1] = (byte)pType;
			_arrByteArray[2] = pCode;
			pData.CopyTo(_arrByteArray, 3);
		}

		public MessageFormat Format {
			get {
				return (MessageFormat)_arrByteArray.ElementAt(0);
			}
		}
			
		public MessageType Type {
			get {
				return (MessageType)_arrByteArray.ElementAt(1);
			}
		}

		public byte Code {
			get {
				return _arrByteArray.ElementAt(2);
			}
		}
			
		public byte[] Data {
			get {
				return _arrByteArray.Skip(3).Take(_arrByteArray.Length - 3).ToArray();
			}
		}

		public byte[] GetByteArray() {
			return _arrByteArray;
		}
	}
}
