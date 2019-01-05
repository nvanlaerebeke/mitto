using ClientProcess;
using ClientManager;
using System;
using Messaging.Base.Response;
using Messaging.App.Response;
using System.Threading.Tasks;
using IConnection;

namespace ConnectionClient.MenuAction.MessageTests {
	public class MultiEchoMesasgeTest : ITest {
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

			Parallel.For(1, 5000, new ParallelOptions { MaxDegreeOfParallelism = 10 }, (i) => {
				_objClient.Request<Echo>(new Messaging.App.Request.Echo(i.ToString()), (Echo pResponse) => {
					Console.WriteLine("Received: " + pResponse.Response);
				});
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