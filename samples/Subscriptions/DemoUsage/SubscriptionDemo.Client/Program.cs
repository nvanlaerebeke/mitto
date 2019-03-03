using System;
using System.Threading;
using Mitto;

namespace Quickstart.Client {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);

		static Mitto.Client _objClient;
		static void Main(string[] args) {
			//Initialize using the default config
			Config.Initialize();

			//When a message is received, display it on the console
			Mitto.Messaging.Action.Request.ReceiveOnChannel.ChannelMessageReceived += delegate (string pChannel, string pMessage) {
				Console.WriteLine($"{pChannel} > {pMessage}");
			};

			//Establish a connection and subscribe to the "MyChannel"
			_objClient = new Mitto.Client();
			_objClient.Connected += delegate (Mitto.Client pClient) {
				Console.WriteLine("Client Connected");
				_objClient.Request<Mitto.Messaging.Response.ACK>(
					new Mitto.Messaging.Subscribe.Channel("MyChannel"), r => {
						if (r.Status == Mitto.IMessaging.ResponseCode.Success) {
							Start();
						} else {
							Console.WriteLine("Failed Subscribing to Channel");
						}
					}
				);
			};
			_objClient.ConnectAsync("localhost", 8080, false);

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_quit.Set();
			};
			_quit.WaitOne();
		}

		static void Start() {
			ThreadPool.QueueUserWorkItem(s => {
				while (true) {
					var text = Console.ReadLine();
					_objClient.Request<Mitto.Messaging.Response.ACK>(
						new Mitto.Messaging.Request.SendToChannel("MyChannel", text),
						r => {
							if (r.Status != Mitto.IMessaging.ResponseCode.Success) {
								Console.WriteLine($"Failed Sending: {text}");
							}
						}
					);
				}
			});
		}
	}
}