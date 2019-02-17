using System;

namespace Mitto.Connection.Websocket.Server {
	internal interface IWebSocketBehavior {
		event EventHandler<IMessageEventArgs> OnMessageReceived;
		event EventHandler<ICloseEventArgs> OnCloseReceived;
		event EventHandler<IErrorEventArgs> OnErrorReceived;

		string ID { get; }

		void Send(byte[] pData);
	}
}