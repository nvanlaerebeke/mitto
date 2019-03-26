using Mitto.IRouting;
using Mitto.Routing.Response;
using System;
using System.Threading.Tasks;

namespace Mitto.Routing {
	public class Request<T> : IRequest where T : IResponseMessage {
		private readonly IRouter Origin;

		private readonly RoutingFrame Frame;
		private readonly Action<T> Callback;

		public string ID { get { return Origin.ConnectionID; } }
		public MessageStatus Status { get; private set; } = MessageStatus.UnKnown;


		public Request(IRouter pConnection, RoutingFrame pRequest, Action<T> pAction) {
			Origin = pConnection;
			Frame = pRequest;
			Callback = pAction;
		}

		public void Send() {
			Status = MessageStatus.Busy;
			Origin.Transmit(Frame.GetBytes());
		}

		/// <summary>
		/// ToDo: make it so Messaging is not needed here
		/// Routing is on the layer below the Messaging functionality, so it shouldn't need 
		/// any info about it, Routing is responsible for routing frames (RoutingFrame) - not anything else
		/// </summary>
		/// <param name="pFrame"></param>
		public void SetResponse(RoutingFrame pFrame) {
			Task.Run(() => {
				if (pFrame.FrameType == RoutingFrameType.Control) {
					var objResponse = ControlFactory.Provider.GetMessage(new ControlFrame(pFrame.Data));
					Callback.DynamicInvoke(objResponse);
				} else {
					var obj = IMessaging.MessagingFactory.Provider.GetMessage(pFrame.Data);
					Callback.DynamicInvoke(obj);
				}
			});
		}
	}
}