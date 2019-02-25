using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Mitto.Connection.Websocket {
	interface ITimer {
		event EventHandler Elapsed;
		void Start();
		void Stop();
		void Reset();

		void Dispose();
	}
}