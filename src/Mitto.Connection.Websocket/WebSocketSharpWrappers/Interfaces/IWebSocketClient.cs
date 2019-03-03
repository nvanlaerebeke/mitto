﻿using System;
using System.Runtime.CompilerServices;
using WebSocketSharp;

[assembly: InternalsVisibleToAttribute("Mitto.Connection.Websocket.Tests")]
namespace Mitto.Connection.Websocket {
	interface IWebSocketClient {
		event EventHandler OnOpen;
		event EventHandler<ICloseEventArgs> OnClose;
		event EventHandler<IErrorEventArgs> OnError;
		event EventHandler<IMessageEventArgs> OnMessage;

		int ConnectionTimeoutSeconds { get; set; }

		WebSocketState ReadyState { get; }

		void ConnectAsync(string pUrl);
		void Close();
		void Send(byte[] pData);

		bool Ping();
	}
}