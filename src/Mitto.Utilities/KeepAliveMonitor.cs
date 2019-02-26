using System;
using System.Timers;

namespace Mitto.Utilities {
	
	public class KeepAliveMonitor : IDisposable, IKeepAliveMonitor {
		private ITimer _objKeepAliveTimer;
		private ITimer _objPingTimer;

		public event EventHandler TimeOut;
		public event EventHandler UnResponsive;


		public KeepAliveMonitor(ITimer pKeepAliveTimer, ITimer pPingTimer) {
			_objKeepAliveTimer = pKeepAliveTimer;
			_objKeepAliveTimer.Elapsed += _objKeepAliveTimerSender_Elapsed;

			_objPingTimer = pPingTimer;
			_objPingTimer.Elapsed += _objPingTimer_Elapsed;
		}

		/// <summary>
		/// Creates the KeepAliveMonitor with the provided timeout
		/// </summary>
		/// <param name="pTimeOut">Timeout in seconds</param>
		public KeepAliveMonitor(int pTimeOut) {
			_objKeepAliveTimer = new Timer(pTimeOut);
			_objKeepAliveTimer.Elapsed += _objKeepAliveTimerSender_Elapsed;

			_objPingTimer = new Timer(pTimeOut);
			_objPingTimer.Elapsed += _objPingTimer_Elapsed;
		}


		/// <summary>
		/// Starts the timer
		/// </summary>
		public void Start() {
			_objKeepAliveTimer.Start();
			_objPingTimer.Stop();
		}

		/// <summary>
		/// Stops the timer
		/// </summary>
		public void Stop() {
			_objKeepAliveTimer.Stop();
			_objPingTimer.Stop();
		}

		/// <summary>
		/// Starts the countdown for the unresponsive event
		/// </summary>
		public void StartCountDown() {
			_objKeepAliveTimer.Stop();
			_objPingTimer.Start();
		}

		/// <summary>
		/// Called when the keepalive timer elapsed event is triggered
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objKeepAliveTimerSender_Elapsed(object sender, EventArgs e) {
			TimeOut?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Called when the ping timer elapses 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void _objPingTimer_Elapsed(object sender, EventArgs e) {
			UnResponsive?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Resets the timer so it starts from 0 again
		/// </summary>
		public void Reset() {
			try {
				_objKeepAliveTimer.Reset();
				_objPingTimer.Stop();
			} catch (Exception) { }
		}

		public void Dispose() {
			if (_objKeepAliveTimer == null) { return; }

			try {
				_objKeepAliveTimer.Elapsed -= _objKeepAliveTimerSender_Elapsed;
				_objKeepAliveTimer.Stop();
				_objKeepAliveTimer.Dispose();
				_objKeepAliveTimer = null;
			} catch(Exception) { }

			try { 
				_objPingTimer.Elapsed -= _objPingTimer_Elapsed;
				_objPingTimer.Stop();
				_objPingTimer.Dispose();
				_objPingTimer = null;
			} catch (Exception) { }
		}

		/// <summary>
		/// Class deconstructor that cleans up so there is memory or timers in use that are not used
		/// </summary>
		~KeepAliveMonitor() {
			Dispose();
		}
	}
}