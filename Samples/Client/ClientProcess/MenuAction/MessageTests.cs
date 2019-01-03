using ConsoleManager;
using ConnectionClient.MenuAction.MessageTests;

namespace ClientProcess.MenuAction {
	class MessageTests : IMenuAction {
		public Menu GetMenu() {
			Menu objMenu = new Menu();

			//Main Menu Actions
			objMenu.Add(new MenuItem("p", "Send Ping", "Sends a ping (expects a pong response)", delegate () {
				var objTest = new PingMesasgeTest();
				objTest.Test();
			}));

			objMenu.Add(new MenuItem("m", "Multi Ping", "Sends multiple pings in parallel", delegate () {
				var objTest = new MultiPingMesasgeTest();
				objTest.Test();
			}));

			return objMenu;
		}
	}
}