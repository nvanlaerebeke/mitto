using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.Utilities;
using System;
using System.Threading.Tasks;

namespace Mitto.Messaging {
	public class Request<T> : IRequest where T : IResponseMessage {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Delegate _objAction;
		private IClient _objClient;
		private IKeepAliveMonitor _objKeepAliveMonitor;

		public event EventHandler<IRequest> RequestTimedOut;

		public IRequestMessage Message { get; private set; }

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback) : this(pClient, pMessage, pCallback, new KeepAliveMonitor(5)) { }

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback, IKeepAliveMonitor pKeepAliveMonitor) {
			_objClient = pClient;
			Message = pMessage;
			_objAction = pCallback;
			_objKeepAliveMonitor = pKeepAliveMonitor;

			_objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;

			Log.Info($"Creating {Message.Name}({Message.ID}) on {_objClient.ID}");
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			_objKeepAliveMonitor.Stop();
			RequestTimedOut?.Invoke(sender, this);
			Log.Info($"Request {Message.Name}({Message.ID}) unresponsive on {_objClient.ID}, cleaning up...");
		}

		private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			Log.Info($"Request {Message.Name}({Message.ID}) timed out on {_objClient.ID}, checking status...");
			_objKeepAliveMonitor.StartCountDown();

			Task.Run(() => {
				if(_objClient.IsAlive(Message.ID)) {
					_objKeepAliveMonitor.Reset();
				}
				Log.Info($"Request {Message.Name}({Message.ID}) on {_objClient.ID}");
			});
		}

		public void Transmit() {
			Log.Info($"Sending request {Message.Name}({Message.ID}) on {_objClient.ID}");
			_objClient.Transmit(Message);
			_objKeepAliveMonitor.Start();

		}
		public void SetResponse(IResponseMessage pResponse) {
			_objKeepAliveMonitor.Stop();
			_objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

			Task.Run(() => {
				var objSpan = (pResponse.EndTime - pResponse.StartTime);

				Log.Info(
					String.Format(
						"Response {0} received for {1} took {2}",
						Message.Name,
						$"{Message.ID}",
						String.Format(
							"{0}:{1}:{2}.{3}",
							$"{objSpan.Hours.ToString().PadLeft(2, '0')}",
							$"{objSpan.Minutes.ToString().PadLeft(2, '0')}",
							$"{objSpan.Seconds.ToString().PadLeft(2, '0')}",
							$"{objSpan.Milliseconds.ToString().PadLeft(3, '0')}"
						)
					)
				);
				_objAction.DynamicInvoke(pResponse);
			});
		}
	}
}