using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitto.Connection.Websocket {
	interface ICloseEventArgs {
		ushort Code { get; }
		string Reason { get; }
		bool WasClean { get; }
	}
}
