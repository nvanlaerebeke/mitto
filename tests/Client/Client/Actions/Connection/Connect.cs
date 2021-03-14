using System;

namespace Channel.Client.Actions.Connection {

    internal class Connect : BaseAction {

        public Connect(State pState) : base(pState) {
        }

        public override void Run() {
            if (State.Client != null) {
                try {
                    State.Client.Disconnect();
                    State.Client = new Mitto.Client();
                } catch { }
            }
            State.Client.Connected += Client_Connected;
            State.Client.Disconnected += Client_Disconnected;

            State.Client.ConnectAsync(State.Settings.WebSocketParams);
        }

        private void Client_Disconnected(object sender, Mitto.Client e) {
            State.Client.Connected -= Client_Connected;
            State.Client.Disconnected -= Client_Disconnected;
            Console.WriteLine("Client Disconnected");
        }

        private void Client_Connected(object sender, Mitto.Client e) {
            Console.WriteLine("Client Connected");
        }
    }
}