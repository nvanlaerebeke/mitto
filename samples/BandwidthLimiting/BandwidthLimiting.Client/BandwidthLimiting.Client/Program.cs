using BandwidthLimiting.Messaging.Request;
using Mitto.Connection.Websocket;
using Mitto.IMessaging;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using Mitto.Messaging.Subscribe;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BandwidthLimiting.Client {
	class Program {
        static ManualResetEvent _quit = new ManualResetEvent(false);
        static Mitto.Client _objClient;

        static void Main(string[] args) {
            Mitto.Config.Initialize(new Mitto.Config.ConfigParams() {
                MessageProvider = new Mitto.Messaging.MessageProvider("BandwidthLimiting.Messaging")
            });


			_objClient = new Mitto.Client();
            _objClient.Connected += delegate (object sender, Mitto.Client e) {
                Console.WriteLine("Client Connected");
                Start();
            };
            _objClient.ConnectAsync(new ClientParams() {
                Hostname = "localhost",
                Port = 8080,
                Secure = false
            });

            _quit.WaitOne();
        }

        static void Start() {
			Parallel.For(0, 2, (i) => {
                Send();
            });
			/*_objClient.Request<ACKResponse>(new ChannelSubscribe("MyChannel"), r => {
				if (r.Status.State == ResponseState.Success) {
					_objClient.Request<EchoResponse>(new EchoRequest("ABC"), e => {
						Console.WriteLine(e.Message);
					});
					Start();
				}
			});*/
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