using CommandLineMenu;

namespace Channel.Client.Menus {

    internal abstract class BaseMenu {
        protected readonly State State;

        public BaseMenu(State pState) {
            State = pState;
        }

        public abstract Menu GetMenu();
    }
}