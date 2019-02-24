using Mitto.IConnection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Quickstart.Server {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static List<Mitto.Server.Client> _lstClients = new List<Mitto.Server.Client>();
		static void Main(string[] args) {
			Mitto.Mitto.Initialize();

			ThreadPool.QueueUserWorkItem((s) => {

				Mitto.Server.Server objServer = new Mitto.Server.Server();
				objServer.Start(IPAddress.Any, 8080, delegate (Mitto.Server.Client pClient) {
					Console.WriteLine("Client " + pClient.ID + " Connected");
					_lstClients.Add(pClient);
				});
			});

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_quit.Set();
			};
			_quit.WaitOne();
		}
	}
}