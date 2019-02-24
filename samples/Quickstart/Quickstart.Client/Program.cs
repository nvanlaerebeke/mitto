using Mitto.IConnection;
using System;
using System.Threading;

namespace Quickstart.Client {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static void Main(string[] args) {
			Mitto.Mitto.Initialize();

			ThreadPool.QueueUserWorkItem((s) => {
				var objClient = new Mitto.Client();
				objClient.Connected += delegate (Mitto.Client pClient) {
					Console.WriteLine("Client Connected");
					while (true) {
						pClient.Request(new Mitto.Messaging.Request.Echo("TEST"), delegate (Mitto.Messaging.Response.Echo pResponse) {
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