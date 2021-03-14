using CommandLineMenu;

namespace Channel.Client.Menus {

    internal class Main : BaseMenu {

        public Main(State pState) : base(pState) {
        }

        public override Menu GetMenu() {
            Menu objMenu = new Menu();

            //Main Menu Actions
            objMenu.Add(new MenuItem("c", "Connect", "Connect to the server", () => {
                new Actions.Connection.Connect(State).Run();
            }));
            objMenu.Add(new MenuItem("d", "Disconnect", "Disconnect from the server", () => {
                new Actions.Connection.Disconnect(State).Run();
            }));

            objMenu.Add(new MenuItem("b", "Basic Actions", "Basic actions", new Basic(State).GetMenu(), () => { }));
            objMenu.Add(new MenuItem("s", "Subscription Actions", "Subscription actions", new Subscription(State).GetMenu(), () => { }));

            objMenu.Add(new MenuItem("q", "Exit", "Closes down the Client", () => {
                State.Close();
            }));
            return objMenu;
        }
    }
}