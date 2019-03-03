using System;

namespace Mitto.Utilities {
	public interface IKeepAliveMonitor {
		event EventHandler TimeOut;
		event EventHandler UnResponsive;
		void Reset();
		void Start();
		void Stop();
		void StartCountDown();
		void SetInterval(int pSeconds);
	}
}