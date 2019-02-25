using System;

namespace Mitto.Connection.Websocket {
	public interface IKeepAliveMonitor {
		event EventHandler TimeOut;
		event EventHandler UnResponsive;
		void Reset();
		void Start();
		void Stop();
		void StartCountDown();
	}
}