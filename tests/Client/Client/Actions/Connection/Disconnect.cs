namespace Channel.Client.Actions.Connection {
	class Disconnect : BaseAction {
		public Disconnect(State pState) : base(pState) { }

		public override void Run() {
			State.Client.Disconnect();
		}
	}
}
