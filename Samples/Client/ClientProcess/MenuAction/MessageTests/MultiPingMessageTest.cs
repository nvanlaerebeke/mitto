using ClientProcess;
using System;
using System.Threading.Tasks;
using Mitto.Messaging.Base.Response;
using Mitto.Messaging.Base.Request;
using Mitto.ClientManager;

namespace ConnectionClient.MenuAction.MessageTests {
	public class MultiPingMesasgeTest : ITest {
		private Client _objClient;

		public void Test() {
			_objClient = new Client();
			_objClient.Connected += ObjClient_Connected;
			_objClient.Disconnected += ObjClient_Disconnected;

			_objClient.ConnectAsync(Controller.Host, Controller.Port, Controller.Secure);
		}

		private void ObjClient_Connected(Client pClient) {
			Console.WriteLine("Client Connected, sending some data");

			Parallel.For(1, 5000, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i) => {
				_objClient.Request<Pong>(new Ping(), (Pong pResponse) => {
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