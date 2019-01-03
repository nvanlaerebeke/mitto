using ConsoleManager;

namespace ClientUI {
	public static class Controller {
		public static void Start() {
			MenuManager objManager = new MenuManager(GetMainMenu());
			objManager.Start();
		}

		private static Menu GetMainMenu() {
			Menu objMenu = new Menu();

			//Main Menu Actions
			objMenu.Add(new MenuItem("c", "Connect", "Connection Tests", new MenuAction.ConnectionTest().GetMenu(), delegate() { }));
			objMenu.Add(new MenuItem("d", "Data", "Data Tests", new MenuAction.DataTests().GetMenu(), delegate () { }));
			objMenu.Add(new MenuItem("m", "Messaging", "Messaging Tests", new MenuAction.MessageTests().GetMenu(), delegate () { }));
			return objMenu;
		}
	}
}