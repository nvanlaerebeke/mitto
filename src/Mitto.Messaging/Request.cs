using Mitto.ILogging;
using Mitto.IMessaging;
using Mitto.Utilities;
using System;
using System.Threading.Tasks;

namespace Mitto.Messaging {
	internal class Request<T> : IRequest where T : IResponseMessage {
		private readonly ILog Log = LogFactory.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private Delegate _objAction;
		private IClient _objClient;
		private IKeepAliveMonitor _objKeepAliveMonitor;

		public event EventHandler<IRequest> RequestTimedOut;

		public IRequestMessage Message { get; private set; }

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback) : this(pClient, pMessage, pCallback, new KeepAliveMonitor(30)) { }

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback, IKeepAliveMonitor pKeepAliveMonitor) {
			_objClient = pClient;
			Message = pMessage;
			_objAction = pCallback;
			_objKeepAliveMonitor = pKeepAliveMonitor;

			_objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;

			Log.Info($"Creating new request: {Message.ID}({Message.Name})");
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			_objKeepAliveMonitor.Stop();
			RequestTimedOut?.Invoke(sender, this);
			Log.Info($"Request {Message.ID}({Message.Name}) unresponsive, cleaning up...");
		}

		private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			Log.Info($"Request {Message.ID}({Message.Name}) timed out, checking status...");
			_objKeepAliveMonitor.StartCountDown();

			_objClient.Request<Response.MessageStatusResponse>(new Request.MessageStatusRequest(Message.ID), (r => {
				if (
					r.RequestStatus == MessageStatusType.Busy ||
					r.RequestStatus == MessageStatusType.Queued
				) {
					_objKeepAliveMonitor.Reset();
					Log.Info($"Request {Message.ID}({Message.Name}) still alive, resetting timer");
				}
			}));
		}

		public void Transmit() {
			Log.Info($"Sending request {Message.ID}({Message.Name}) still alive, resetting timer");
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
					String.Format("Response received for {0} took {1}",
						$"{Message.ID}({Message.Name})",
						String.Format(
							"{0}:{1}:{2}.{3}",
							$"{objSpan.Hours.ToString().PadLeft(2, '0')}",
							$"{objSpan.Minutes.ToString().PadLeft(2, '0')}",
							$"{objSpan.Seconds.ToString().PadLeft(2, '0')}",
							$"{objSpan.Milliseconds.ToString().PadLeft(4, '0')}"
						)
					)
				);
				_objAction.DynamicInvoke(pResponse);
			});
		}
	}
}