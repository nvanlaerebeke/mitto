using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Mitto;
using Mitto.Connection.Websocket;

namespace Quickstart.Server {

    internal class Program {
        private static ManualResetEvent _quit = new ManualResetEvent(false);
        private static List<ClientConnection> _lstClients = new List<ClientConnection>();

        private static void Main(string[] args) {
            Config.Initialize(new Config.ConfigParams() {
                ConnectionProvider = new WebSocketConnectionProvider() {
                    ServerConfig = new ServerParams() {
                        IP = IPAddress.Any,
                        Port = 80,
                        Path = "/",
                        FragmentSize = 512,
                    }
                }
            });

            new Mitto.Server().Start(null, c => {
                c.Disconnected += PClient_Disconnected;
                _lstClients.Add(c);
                Console.WriteLine("Client Connected");
            });

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                _quit.Set();
            };
            _quit.WaitOne();
            Console.WriteLine("Exiting...");
        }

        private static void PClient_Disconnected(object sender, ClientConnection pClient) {
            pClient.Disconnected -= PClient_Disconnected;
            _lstClients.Remove(pClient);
        }
    }
}