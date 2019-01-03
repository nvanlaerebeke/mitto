using ClientProcess;
using ClientManager;
using System;
using Messaging.Base.Response;
using System.Threading.Tasks;

namespace ConnectionClient.MenuAction.MessageTests {
	public class MultiPingMesasgeTest : ITest {
		private Client _objClient;

		public void Test() {
			_objClient = new Client();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;
			_objClient.ConnectAsync("localhost", 80, false);
		}

		private void ObjClient_Connected(Client pClient) {
			Console.WriteLine("Client Connected, sending some data");

			Parallel.For(1, 5000, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i) => {
				_objClient.Request<Pong>(new Messaging.Base.Request.Ping(), (Pong pResponse) => {
					Console.WriteLine("Pong Received: " + i);
				});
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