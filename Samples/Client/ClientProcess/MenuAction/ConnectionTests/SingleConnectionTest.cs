using Mitto.IConnection;
using System;

namespace ClientProcess.MenuAction.ConnectionTests {
	class SingleConnectionTest : ITest {
		private IClient _objClient;

		public void Test() {
			if (_objClient != null) {
				_objClient.Close();
			}
			_objClient = ConnectionFactory.GetClient();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;

			Console.WriteLine("Connect client");
			_objClient.ConnectAsync("localhost", 80, false);
		}

		private void ObjClient_Connected(IConnection pClient) {
			Console.WriteLine("Client Connected");
		}

		private void ObjClient_Disconnected(IConnection pClient) {
			Console.WriteLine("Client Disconnected");
		}

		internal void Close() {
			_objClient.Close();
		}
	}
}