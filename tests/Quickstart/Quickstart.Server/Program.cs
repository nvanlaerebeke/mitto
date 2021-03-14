using Mitto;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Quickstart.Server {

    internal class Program {
        private static ManualResetEvent _quit = new ManualResetEvent(false);
        private static List<ClientConnection> _lstClients = new List<ClientConnection>();

        private static void Main(string[] args) {
            Mitto.Config.Initialize();

            ThreadPool.QueueUserWorkItem((s) => {
                Mitto.Server objServer = new Mitto.Server();
                objServer.Start(new Mitto.Connection.Websocket.ServerParams(IPAddress.Any, 8080), delegate (ClientConnection pClient) {
                    pClient.Disconnected += PClient_Disconnected;
                    _lstClients.Add(pClient);
                    Console.WriteLine("Client Connected");
                });
            });

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                _quit.Set();
            };
            _quit.WaitOne();
        }

        private static void PClient_Disconnected(object sender, ClientConnection e) {
            e.Disconnected -= PClient_Disconnected;
            _lstClients.Remove(e);
        }
    }
}