using CommandLineMenu;
using System;

namespace Channel.Client {

    internal class Controller {
        private readonly MenuManager MenuManager;
        private readonly State State;

        public event EventHandler Exit;

        public Controller(Settings pSettings) {
            State = new State(pSettings);
            State.Exit += State_Exit;
            MenuManager = new MenuManager(new Menus.Main(State).GetMenu());
        }

        private void State_Exit(object sender, EventArgs e) {
            Exit?.Invoke(this, new EventArgs());
        }

        public void Start() {
            MenuManager.Start();
        }
    }
}