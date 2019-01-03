using ClientUI.MenuAction.ConnectionTests;
using ConsoleManager;
using System.Collections.Generic;

namespace ClientUI.MenuAction {
	class ConnectionTest : IMenuAction {
		private List<SingleConnectionTest> _lstSingleConnectionTests = new List<SingleConnectionTest>();
		private List<MultiConnectionTest> _lstMultiConnectionTests = new List<MultiConnectionTest>();
		public Menu GetMenu() {
			Menu objMenu = new Menu();

			//Main Menu Actions
			objMenu.Add(new MenuItem("c", "Connect", "Connect to the server", delegate () {
				var objTest = new SingleConnectionTest();
				_lstSingleConnectionTests.Add(objTest);

				objTest.Test();
			}));

			objMenu.Add(new MenuItem("m", "MultiConnect", "Connected multiple clients from multiple threads", delegate () {
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
			}));
			return objMenu;
		}
	}
}