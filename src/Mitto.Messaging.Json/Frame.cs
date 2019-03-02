using Mitto.IMessaging;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Mitto.Messaging.Json.Tests")]
namespace Mitto.Messaging.Json {
	/// <summary>
	/// Encapsulates and provides information about the data being transmitted
	/// 
	/// The frame looks like this:
	/// ------------------------------------
	/// | byte MessageFormat | byte[] data |
	/// ------------------------------------
	/// 
	/// </summary>
	internal class Frame : IFrame {
		private byte[] _arrByteArray;
		public Frame(byte[] pData) {
			_arrByteArray = pData;
		}

		public Frame(MessageFormat pFormat, byte[] pData) {
			_arrByteArray = new byte[1 + pData.Length];
			_arrByteArray[0] = (byte)pFormat;
			pData.CopyTo(_arrByteArray, 1);
		}

		public MessageFormat Format {
			get {
				return (MessageFormat)_arrByteArray.ElementAt(0);
			}
		}

		public byte[] Data {
			get {
				return _arrByteArray.Skip(1).Take(_arrByteArray.Length - 1).ToArray();
			}
		}

		public byte[] GetByteArray() {
			return _arrByteArray;
		}
	}
}