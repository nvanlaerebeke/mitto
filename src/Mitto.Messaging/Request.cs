using Mitto.IMessaging;
using Mitto.Utilities;
using System;
using System.Threading.Tasks;

namespace Mitto.Messaging {
	internal class Request<T> : IRequest where T : IResponseMessage {
		private Delegate _objAction;
		private IClient _objClient;
		private IKeepAliveMonitor _objKeepAliveMonitor;

		public event EventHandler<IRequest> RequestTimedOut;

		public IRequestMessage Message { get; private set; }

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback, IKeepAliveMonitor pKeepAliveMonitor) {
			init(pClient, pMessage, pCallback, pKeepAliveMonitor);
		}

		public Request(IClient pClient, IRequestMessage pMessage, Action<T> pCallback) {
			init(pClient, pMessage, pCallback, new KeepAliveMonitor(30));
		}

		private void init(IClient pClient, IRequestMessage pMessage, Action<T> pCallback, IKeepAliveMonitor pKeepAliveMonitor) {
			_objClient = pClient;
			Message = pMessage;
			_objAction = pCallback;
			_objKeepAliveMonitor = pKeepAliveMonitor;

			_objKeepAliveMonitor.TimeOut += _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive += _objKeepAliveMonitor_UnResponsive;
		}

		private void _objKeepAliveMonitor_UnResponsive(object sender, EventArgs e) {
			_objKeepAliveMonitor.Stop();
			RequestTimedOut?.Invoke(sender, this);
		}

		private void _objKeepAliveMonitor_TimeOut(object sender, EventArgs e) {
			_objKeepAliveMonitor.StartCountDown();

			_objClient.Request<Response.MessageStatusResponse>(new Request.MessageStatusRequest(Message.ID), (r => {
				if (
					r.RequestStatus == MessageStatusType.Busy ||
					r.RequestStatus == MessageStatusType.Queued
				) {
					_objKeepAliveMonitor.Reset();
				}
			}));
		}

		public void Transmit() {
			_objClient.Transmit(Message);
			_objKeepAliveMonitor.Start();

		}
		public void SetResponse(IResponseMessage pResponse) {
			_objKeepAliveMonitor.Stop();
			_objKeepAliveMonitor.TimeOut -= _objKeepAliveMonitor_TimeOut;
			_objKeepAliveMonitor.UnResponsive -= _objKeepAliveMonitor_UnResponsive;

			Task.Run(() => {
				_objAction.DynamicInvoke(pResponse);
			});
		}
	}
}