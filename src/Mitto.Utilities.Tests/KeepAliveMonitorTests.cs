using NSubstitute;
using NUnit.Framework;
using System;

namespace Mitto.Utilities.Tests {
	[TestFixture]
	public class KeepAliveMonitorTests {
		/// <summary>
		/// Test the KeepAliveMonitor Start method
		/// This means that the Start on the keepalive timer is called
		/// and the stop on the ping timer
		/// </summary>
		[Test]
		public void StartTest() {
			//Arrange
			var objKeepAliveTimer = Substitute.For<ITimer>();
			var objPingTimer = Substitute.For<ITimer>();

			//Act
			var obj = new KeepAliveMonitor(objKeepAliveTimer, objPingTimer);
			obj.Start();

			//Assert
			objKeepAliveTimer.Received(1).Start();
			objPingTimer.Received(1).Stop();
		}

		/// <summary>
		/// Test the Stop method
		/// This means that the stop method is called on both timers
		/// </summary>
		[Test]
		public void StopTest() {
			//Arrange
			var objKeepAliveTimer = Substitute.For<ITimer>();
			var objPingTimer = Substitute.For<ITimer>();

			//Act
			var obj = new KeepAliveMonitor(objKeepAliveTimer, objPingTimer);
			obj.Stop();

			//Assert
			objKeepAliveTimer.Received(1).Stop();
			objPingTimer.Received(1).Stop();
		}

		/// <summary>
		/// Test the reset method
		/// This means that the Reset method is called on the keepalive timer
		/// and the stop method on the ping timer
		/// </summary>
		[Test]
		public void ResetTest() {
			//Arrange
			var objKeepAliveTimer = Substitute.For<ITimer>();
			var objPingTimer = Substitute.For<ITimer>();

			//Act
			var obj = new KeepAliveMonitor(objKeepAliveTimer, objPingTimer);
			obj.Reset();

			//Assert
			objKeepAliveTimer.Received(1).Reset();
			objPingTimer.Received(1).Stop();
		}

		/// <summary>
		/// Test the Timeout event when the KeepAlive timer elapses
		/// This means that the TimeOut event is raised when the KeepAlive timer Elapsed is triggered
		/// </summary>
		[Test]
		public void TimeoutTest() {
			//Arrange
			var objKeepAliveTimer = Substitute.For<ITimer>();
			var objPingTimer = Substitute.For<ITimer>();
			var objHandler = Substitute.For<EventHandler>();
			var obj = new KeepAliveMonitor(objKeepAliveTimer, objPingTimer);
			obj.TimeOut += objHandler;

			//Act
			objKeepAliveTimer.Elapsed += Raise.Event<EventHandler>(obj, new EventArgs());

			//Assert
			objHandler
				.Received(1)
				.Invoke(Arg.Is(obj), Arg.Any<EventArgs>());
		}

		/// <summary>
		/// Test the UnResponsive event when the Ping timer expires
		/// This means that the UnResponsive event is raised whtne the ping timer Elapsed is triggered
		/// </summary>
		[Test]
		public void UnResponsiveTest() {
			//Arrange
			var objKeepAliveTimer = Substitute.For<ITimer>();
			var objPingTimer = Substitute.For<ITimer>();
			var objHandler = Substitute.For<EventHandler>();
			var obj = new KeepAliveMonitor(objKeepAliveTimer, objPingTimer);
			obj.UnResponsive += objHandler;

			//Act
			objPingTimer.Elapsed += Raise.Event<EventHandler>(obj, new EventArgs());

			//Assert
			objHandler
				.Received(1)
				.Invoke(Arg.Is(obj), Arg.Any<EventArgs>());
		}
	}
}
