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

			objMenu.Add(new MenuItem("mp", "Multi Ping", "Sends multiple pings in parallel", delegate () {
				var objTest = new MultiPingMesasgeTest();
				objTest.Test();
			}));

			objMenu.Add(new MenuItem("e", "Echo", "Sends a message and echo the msg send", delegate () {
				var objTest = new EchoMesasgeTest();
				objTest.Test();
			}));

			objMenu.Add(new MenuItem("me", "Multi Echo", "Sends messages in parallel and echo the msg send", delegate () {
				var objTest = new MultiEchoMesasgeTest();
				objTest.Test();
			}));
			return objMenu;
		}
	}
}