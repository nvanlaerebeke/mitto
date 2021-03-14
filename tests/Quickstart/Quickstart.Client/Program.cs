using System;

namespace Quickstart.Client {

    internal class Program {
        private static Mitto.Client _objClient;

        private static void Main(string[] args) {
            Mitto.Config.Initialize();

            _objClient = new Mitto.Client();
            _objClient.Connected += delegate (object sender, Mitto.Client e) {
                Console.WriteLine("Client Connected");
                _objClient.Request<Mitto.Messaging.Response.EchoResponse>(new Mitto.Messaging.Request.EchoRequest("My Message"), (r => {
                    Console.WriteLine($"Received: {r.Message}");
                }));
            };
            _objClient.ConnectAsync(new Mitto.Connection.Websocket.ClientParams() { Hostname = "localhost", Port = 8080, Secure = false });

            Console.ReadKey();
        }
    }
}