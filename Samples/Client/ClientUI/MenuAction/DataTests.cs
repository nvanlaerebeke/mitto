using ConnectionClient.MenuAction.DataTests;
using ConsoleManager;

namespace ClientUI.MenuAction {
	class DataTests : IMenuAction {
		public Menu GetMenu() {
			Menu objMenu = new Menu();

			//Main Menu Actions
			objMenu.Add(new MenuItem("s", "Send Packet", "Send single packet over a connection", delegate () {
				var objTest = new SendSinglePacketTest();
				objTest.Test();
			}));
			return objMenu;
		}
	}
}