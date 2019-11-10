using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Connection.WebsocketSharp {
	interface ICloseEventArgs {
		ushort Code { get; }
		string Reason { get; }
		bool WasClean { get; }
	}
}
