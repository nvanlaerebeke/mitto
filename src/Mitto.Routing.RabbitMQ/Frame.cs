using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Routing.RabbitMQ {
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
	public class Frame {
		public Frame(byte[] data) {

		}

		public byte[] Data {
			get {
				return new byte[] { };
			}
		}
	}
}