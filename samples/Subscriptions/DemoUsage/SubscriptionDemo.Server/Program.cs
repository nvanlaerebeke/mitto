using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Mitto;
using Mitto.Connection.Websocket;

namespace Quickstart.Server {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static List<ClientConnection> _lstClients = new List<ClientConnection>();
		static void Main(string[] args) {
			Config.Initialize();

			new Mitto.Server().Start(new ServerParams(IPAddress.Any, 8080),  c => {
				c.Disconnected += PClient_Disconnected;
				_lstClients.Add(c);
				Console.WriteLine("Client Connected");
			});

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