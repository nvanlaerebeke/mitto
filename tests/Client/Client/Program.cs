using System;
using System.Threading;
using System.Threading.Tasks;

namespace Channel.Client {

    internal class Program {
        private static ManualResetEvent _quit = new ManualResetEvent(false);
        private static Controller Controller;
        private static Settings Settings;

        private static void Main(string[] args) {
            UseDefaultSettings();

            Task.Run(() => {
                Controller = new Controller(Settings);
                Controller.Exit += (object sender, EventArgs e) => {
                    _quit.Set();
                };
                Controller.Start();
            });

            _quit.WaitOne();
        }

        private static void UseDefaultSettings() {
            Settings = new Settings() {
                WebSocketParams = new Mitto.Connection.Websocket.ClientParams() {
                    HostName = "localhost",
                    Port = 80,
                    Secure = false,
                }
            };
        }
    }
}