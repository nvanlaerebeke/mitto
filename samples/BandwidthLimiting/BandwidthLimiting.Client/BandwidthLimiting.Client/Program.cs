using BandwidthLimiting.Messaging.Request;
using Mitto.Connection.Websocket;
using Mitto.Messaging.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BandwidthLimiting.Client {
    class Program {
        static ManualResetEvent _quit = new ManualResetEvent(false);
        static Mitto.Client _objClient = new Mitto.Client();

        static void Main(string[] args) {

            Mitto.Config.Initialize(new Mitto.Config.ConfigParams() {
                MessageProvider = new Mitto.Messaging.MessageProvider("BandwidthLimiting.Messaging")
            });

            _objClient.Connected += delegate (object sender, Mitto.Client e) {
                Console.WriteLine("Client Connected");
                Start();
            };
            _objClient.ConnectAsync(new ClientParams() {
                Hostname = "localhost",
                Port = 8080,
                Secure = false
            });

            Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
                _quit.Set();
            };
            _quit.WaitOne();
        }

        static void Start() {
            Parallel.For(0, 2, (i) => {
                Send();
            });
        }

        static void Send() {
            Console.WriteLine("Sending Data");
            byte[] arrData = new byte[1024 * 1000]; // -- 1MB data
            _objClient.Request<ACKResponse>(new ReceiveBinaryDataRequest(arrData), r => {
                Send();
            });
        }
    }
}