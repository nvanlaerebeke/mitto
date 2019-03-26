using System;
using System.Threading.Tasks;
using Mitto.IRouting;
using Mitto.Routing.Request;
using Mitto.Routing.Response;

/// <summary>
/// ToDo: Add downstream KeepAlive that check if the request is still being processed by the IConnection
/// Will need to pass the actual connection instead of the connection id
///
/// Combine with Request
/// </summary>
namespace Mitto.Routing {

	public class ControlRequest<T> : IRequest where T: ControlResponse {
		public string ID => Guid.NewGuid().ToString();
		public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;

		private readonly IRouter Router;
		private readonly ControlRequestMessage Request;
		private Action<T> Callback;

		public ControlRequest(IRouter pRouter, ControlRequestMessage pRequest, Action<T> pAction) {
			Router = pRouter;
			Request = pRequest;
			Callback = pAction;
		}

		public void Send() {
			Status = MessageStatus.Busy;
			Router.Transmit(new RoutingFrame(RoutingFrameType.Control, Request.ID, Router.ConnectionID, "", Request.GetFrame().GetBytes()).GetBytes());
		}

		public void SetResponse(RoutingFrame pFrame) {
			Task.Run(() => {
				var obj = ControlFactory.Provider.GetMessage(new ControlFrame(pFrame.Data));
				Callback.DynamicInvoke(obj);
			});			
		}
	}
}