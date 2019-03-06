using Mitto.Connection.Websocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mitto;

namespace BandwidthLimiting.Server {
    class Program {
        static ManualResetEvent _quit = new ManualResetEvent(false);
        static Mitto.Server _objServer;
        static List<ClientConnection> _lstClients = new List<ClientConnection>();

        static void Main(string[] args) {
            Config.Initialize(new Config.ConfigParams() {
                MessageProvider = new Mitto.Messaging.MessageProvider("BandwidthLimiting.Messaging")
            });

            _objServer = new Mitto.Server();
            _objServer.Start(new ServerParams(IPAddress.Any, 8080) {
                
            }, c => {
                Console.WriteLine("Client Connected");
                c.Disconnected += C_Disconnected;
                _lstClients.Add(c);
            });

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                _quit.Set();
            };
            _quit.WaitOne();
        }
        

        private static void C_Disconnected(object sender, ClientConnection e) {
            Console.WriteLine("Client Disconnected");
            _lstClients.Remove(e);
        }
    }
}
