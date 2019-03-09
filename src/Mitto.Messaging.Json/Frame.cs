using Mitto.IMessaging;
using System;
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
				 //Don't do this as it's slower
				 //_arrByteArray.Skip(1).Take(_arrByteArray.Length - 1).ToArray();

				//-- this is faster
				var newArray = new byte[_arrByteArray.Length - 1];
				Array.Copy(
					_arrByteArray, // -- source array
					1,  // -- start index in the source
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