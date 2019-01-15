using ClientProcess;
using System;
using Messaging.App.Response;
using Mitto.ClientManager;

namespace ConnectionClient.MenuAction.MessageTests {
	public class EchoMesasgeTest : ITest {
		private Client _objClient;

		public void Test() {
			if (_objClient != null) {
				Close();
			}
			_objClient = new Client();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;

			_objClient.ConnectAsync(Controller.Host, Controller.Port, Controller.Secure);
		}

		private void ObjClient_Connected(Client pClient) {
			Console.WriteLine("Client Connected, sending some data");

			_objClient.Request<Echo>(new Messaging.App.Request.Echo("echo"), (Echo pResponse) => {
				Console.WriteLine("Received: " + pResponse.Response);
			});
		}

		private void ObjClient_Disconnected(Client pClient) {
			Console.WriteLine("Client Disconnected");
		}

		internal void Close() {
			_objClient.Close();
			_objClient = null;
		}
	}
}