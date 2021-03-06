﻿using Mitto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace ChatServer {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static List<ClientConnection> _lstClients = new List<ClientConnection>();
		static void Main(string[] args) {
			//Load Mitto and add our own messages and actions to the MessageProvider
			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = new Mitto.Messaging.MessageProvider(new string[] {
					"ChatSample.Messaging",
					"ChatSampleServer"
				})
			});

			new Server()
				.Start(
					new Mitto.Connection.Websocket.ServerParams(IPAddress.Any, 8080),
					c => {
						c.Disconnected += PClient_Disconnected;
						_lstClients.Add(c);
						Console.WriteLine("Client Connected");
					}
				)
			;

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_quit.Set();
			};
			_quit.WaitOne();
		}

		private static void PClient_Disconnected(object sender, ClientConnection pClient) {
			pClient.Disconnected -= PClient_Disconnected;
			_lstClients.Remove(pClient);
		}
	}
}