using System;

namespace Quickstart.Client {
	class Program {
		static Mitto.Client _objClient;
		static void Main(string[] args) {
			Mitto.Config.Initialize();

			_objClient = new Mitto.Client();
			_objClient.Connected += delegate (Mitto.Client pClient) {
				Console.WriteLine("Client Connected");
				_objClient.Request<Mitto.Messaging.Response.Echo>(new Mitto.Messaging.Request.Echo("My Message"), (r => {
					Console.WriteLine($"Received: {r.Message}");
				}));
			};
			_objClient.ConnectAsync("localhost", 8080, false);

			Console.ReadKey();
		}
	}
}