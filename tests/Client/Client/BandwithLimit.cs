using Mitto;
using Mitto.Connection.Websocket;
using Mitto.Messaging.Action.Request;
using Mitto.Messaging.Request;
using Mitto.Messaging.Response;
using System;
using System.Threading.Tasks;

namespace Channel.Client {
	public static class BandwidthLimit {
		static Mitto.Client _objClient;

		public static void Start() {
			Config.Initialize(new Config.ConfigParams() {
				ConnectionProvider = new WebSocketConnectionProvider() { FragmentLength = (512 * 1024) - 14 } // use 512kb for a single frame
			});

			_objClient = new Mitto.Client();
			ReceiveOnChannelRequestAction.ChannelMessageReceived += ReceiveOnChannel_ChannelMessageReceived;

			_objClient.Connected += delegate (object sender, Mitto.Client pClient) {
				Console.WriteLine("Client Connected");
				//StartSending();
				StartReceiving();
			};

			_objClient.ConnectAsync(new ClientParams() {
				//Hostname = "192.168.0.111", // -- ubuntu server
				Hostname = "192.168.0.105", // -- desktop
				Port = 8080,
				Secure = false,
				MaxBytePerSecond = 1000 * 1000 * 1 // -- bytes - kilobyte - megabyte
			});
		}

		private static void ReceiveOnChannel_ChannelMessageReceived(string pChannel, string pMessage) {
			Console.WriteLine($"{pChannel} > {pMessage}");
		}

		public static void StartReceiving() {
			Task.Run(() => {
				while (true) {
					Console.WriteLine(_objClient.CurrentBytesPerSecond);
					System.Threading.Thread.Sleep(1000);
				}
			});
			Parallel.For(0, 5, (i) => {
				Get();
			});
		}

		private static void Get() {
			Console.WriteLine("Get Data Packet");
			_objClient.Request<BinaryDataResponse>(new GetBinaryDataRequest(), e => {
				Console.WriteLine("Get Next Data Packet");
				Get();
			});
		}

		private static void StartSending() {
			Task.Run(() => {
				while(true) {
					Console.WriteLine(_objClient.CurrentBytesPerSecond);
					System.Threading.Thread.Sleep(1000);
				}
			});
			Parallel.For(0, 5, (i) => {
				Send();
			});
		}

		private static void Send() {
			var arrBytes = new byte[1024 * 40000];
			new Random().NextBytes(arrBytes);

			Console.WriteLine("Sending Data Packet");
			_objClient.Request<ACKResponse>(new ReceiveBinaryDataRequest(arrBytes), e => {
				Console.WriteLine("Sending Next Data Packet");
				Send();
			});
		}
	}
}