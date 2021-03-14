using Mitto.Connection.Websocket;

namespace Channel.Client {

    public class Settings {

        public ClientParams WebSocketParams { get; set; } = new ClientParams() {
            HostName = "localhost",
            Port = 80,
            Secure = false,
        };
    }
}