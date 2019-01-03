using ClientProcess;
using ClientManager;
using System;
using Messaging.Base.Response;

namespace ConnectionClient.MenuAction.MessageTests {
	public class PingMesasgeTest : ITest {
		private Client _objClient;

		public void Test() {
			if (_objClient != null) {
				_objClient.Close();
			}
			_objClient = new Client();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;

			_objClient.ConnectAsync("localhost", 80, false);
		}

		private void ObjClient_Connected(Client pClient) {
			Console.WriteLine("Client Connected, sending some data");

			_objClient.Request<Pong>(new Messaging.Base.Request.Ping(), (Pong pResponse) => {
				Console.WriteLine("Pong Received");
			});
		}

		private void ObjClient_Disconnected(Client pClient) {
			Console.WriteLine("Client Disconnected");
		}

		internal void Close() {
			_objClient.Close();
		}
	}
}