
using Mitto.IRouting;
using System;

namespace Mitto.Routing {
	/// <summary>
	/// ToDo: move to IRouting
	/// </summary>
	public interface IRequest {
		string ID { get; }
		MessageStatus Status { get; }
		event EventHandler<IRequest> RequestTimedOut;
		/// <summary>
		/// ToDo: rename to Start()
		/// </summary>
		void Send();
		void SetResponse(RoutingFrame pFrame);
	}
}