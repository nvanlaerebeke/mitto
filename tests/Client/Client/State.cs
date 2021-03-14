using Mitto;
using Mitto.Connection.Websocket;
using System;

namespace Channel.Client {

    internal class State {

        public event EventHandler Exit;

        public readonly Settings Settings;
        public Mitto.Client Client { get; set; }

        public State(Settings pSettings) {
            Settings = pSettings;
            Config.Initialize(new Config.ConfigParams() { ConnectionProvider = new WebSocketConnectionProvider() });
            Client = new Mitto.Client();
        }

        public void Close() {
            Exit?.Invoke(this, new EventArgs());
        }
    }
}