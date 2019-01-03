using ConsoleManager;
using ConnectionClient.MenuAction.MessageTests;

namespace ClientUI.MenuAction {
	class MessageTests : IMenuAction {
		public Menu GetMenu() {
			Menu objMenu = new Menu();

			//Main Menu Actions
			objMenu.Add(new MenuItem("p", "Send Ping", "Sends a ping (expects a pong response)", delegate () {
				var objTest = new PingMesasgeTest();
				objTest.Test();
			}));

			/*objMenu.Add(new MenuItem("m", "MultiConnect", "Connected multiple clients from multiple threads", delegate () {
				var objTest = new MultiConnectionTest();
				_lstMultiConnectionTests.Add(objTest);
				objTest.Test();
			}));

			objMenu.Add(new MenuItem("d", "Disconnect", "Disconnect all clients", delegate () {
				lock (_lstSingleConnectionTests) {
					_lstSingleConnectionTests.ForEach(t => t.Close());
					_lstSingleConnectionTests.Clear();
				}

				lock (_lstMultiConnectionTests) {
					_lstMultiConnectionTests.ForEach(t => t.Close());
					_lstMultiConnectionTests.Clear();
				}
			}));*/
			return objMenu;
		}
	}
}