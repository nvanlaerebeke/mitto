using ChatSampleClient.Action.Request;
using Mitto;
using Mitto.Messaging.Response;
using System;
using System.Threading;

namespace ChatSampleClient {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static Client _objClient;
		static void Main(string[] args) {
			//Load Mitto and add our own messages and actions to the MessageProvider
			Config.Initialize(new Config.ConfigParams() {
				MessageProvider = new Mitto.Messaging.MessageProvider(new string[] {
					"ChatSample.Messaging",
					"ChatSampleClient"
				})
			});

			//When a message is received, display it on the console
			ReceiveMessage.MessageReceived += delegate (string pChannel, string pMessage) {
				Console.WriteLine($"{pChannel} > {pMessage}");
			};

			//Establish a connection and subscribe to the "MyChannel"
			_objClient = new Client();
			_objClient.Connected += delegate (Mitto.Client pClient) {
				Console.WriteLine("Client Connected");
				_objClient.Request<ACK>(
					new ChatSample.Messaging.Subscribe.Chat("MyChannel"), (r => {
						if (r.Status == Mitto.IMessaging.ResponseCode.Success) {
							Start(); // -- start listening for messages to send to the channel
						} else {
							Console.WriteLine("Failed Subscribing to Channel");
						}
					})
				);
			};
			_objClient.ConnectAsync("localhost", 8080, false);


			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_quit.Set();
			};
			_quit.WaitOne();
		}

		/// <summary>
		/// Reads the Console for new lines on a separte thread
		/// </summary>
		static void Start() {
			ThreadPool.QueueUserWorkItem(s => {
				while (true) {
					var text = Console.ReadLine();
					//Send the text that was just entered on the console to the channel
					_objClient.Request<ACK>(
						new ChatSample.Messaging.Request.SendMessage(
							"MyChannel",
							text
						),
						(r => {
							//Show the failure on error
							if (r.Status != Mitto.IMessaging.ResponseCode.Success) {
								Console.WriteLine($"Send Failed: {text}");
							}
						})
					);
				}
			});
		}
	}
}