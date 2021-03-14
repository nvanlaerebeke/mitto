using CommandLineMenu;

namespace Channel.Client.Menus {

    internal class Basic : BaseMenu {

        public Basic(State pState) : base(pState) {
        }

        public override Menu GetMenu() {
            Menu objMenu = new Menu();
            objMenu.Add(new MenuItem("e", "Echo", "send a message and expect the text sent back in the response", () => {
                new Actions.Message.SendEcho(State).Run();
            }));
            objMenu.Add(new MenuItem("b", "BigEcho", "send a big message and expect the text sent back in the response", () => {
                new Actions.Message.BigEcho(State).Run();
            }));
            objMenu.Add(new MenuItem("s", "SpamEcho", "spams messages and expect the text sent back in the response", () => {
                new Actions.Message.SpamEcho(State).Run();
            }));
            objMenu.Add(new MenuItem("p", "Ping", "Send a Ping and expect a Pong back in the response", () => {
                new Actions.Message.Ping(State).Run();
            }));
            return objMenu;
        }
    }
}