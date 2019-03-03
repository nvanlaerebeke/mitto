using System;
using System.Timers;

namespace Mitto.Utilities {
	class Timer : ITimer {
		private System.Timers.Timer _objTimer;

		public Timer(int pInterval) {
			_objTimer = new System.Timers.Timer() {
				Interval = pInterval * 1000,
				AutoReset = false,
				Enabled = true
			};
			_objTimer.Elapsed += _objTimer_Elapsed;
		}

		public event EventHandler Elapsed;

		public void Dispose() {
			_objTimer.Elapsed -= _objTimer_Elapsed;
			_objTimer.Stop();
			_objTimer.Dispose();
			_objTimer = null;
		}

		public void Reset() {
			_objTimer.Stop();
			_objTimer.Start();
		}

		public void SetTimeout(int pSeconds) {
			_objTimer.Interval = pSeconds * 1000;
			Reset();
		}

		public void Start() {
			_objTimer.Start();
		}

		public void Stop() {
			_objTimer.Stop();
		}

		private void _objTimer_Elapsed(object sender, ElapsedEventArgs e) {
			Elapsed?.Invoke(this, e);
		}
	}
}
