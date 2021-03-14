namespace Channel.Client.Actions {
	abstract class BaseAction {
		protected readonly State State;
		public BaseAction(State pState) {
			State = pState;
		}

		public abstract void Run();
	}
}
