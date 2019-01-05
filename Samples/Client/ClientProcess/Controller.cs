using ConsoleManager;

namespace ClientProcess {
	public static class Controller {
		public static string Host { get; private set; }
		public static int Port { get; private set; }
		public static bool Secure { get; private set; }

		public static void Start(string pHostname, int pPort, bool pSecure) {
			Host = pHostname;
			Port = pPort;
			Secure = pSecure;

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