using Mitto;
using Mitto.Routing.RabbitMQ;
using Mitto.Routing.RabbitMQ.Publisher;
using System.Collections.Generic;
using System.Net;

namespace RabbitMQ.Publisher {
	public static class RabbitMQ {
		static List<ClientConnection> _lstClients = new List<ClientConnection>();
		public static void Start() {
			Config.Initialize(new Config.ConfigParams() {
				RouterProvider = new RouterProvider(new RabbitMQParams() { Hostname = "test.crazyzone.be" })
			});

			var objServer = new Server();
			objServer.Start(new Mitto.Connection.Websocket.ServerParams(IPAddress.Any, 8080),  c => {
				c.Disconnected += C_Disconnected;
				_lstClients.Add(c);
			});
		}

		private static void C_Disconnected(object sender, ClientConnection e) {
			_lstClients.Remove(e);
		}
	}
}
