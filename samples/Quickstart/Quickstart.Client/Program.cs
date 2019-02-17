using Mitto.IConnection;
using System;
using System.Threading;
using Mitto.ClientManager;

namespace Quickstart.Client {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static void Main(string[] args) {
			Mitto.Mitto.Initialize();

			ThreadPool.QueueUserWorkItem((s) => {
				Mitto.ClientManager.Client objClient = new Mitto.ClientManager.Client();
				objClient.Connected += delegate (Mitto.ClientManager.Client pClient) {
					Console.WriteLine("Client Connected");
					while (true) {
						pClient.Request<Mitto.Messaging.Base.Response.Echo>(new Mitto.Messaging.Base.Request.Echo("TEST"), delegate (Mitto.Messaging.Base.Response.Echo pResponse) {
							Console.WriteLine(pResponse.Message);
							Thread.Sleep(5000);
						});
					}
				};
				
				objClient.ConnectAsync("localhost", 8080, false);
			});

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_quit.Set();
			};
			_quit.WaitOne();

		}
	}
}