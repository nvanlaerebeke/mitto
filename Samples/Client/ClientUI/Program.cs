using Connection.Websocket.Client;
using Connection.Websocket.Server;
using IConnection;
using IMessaging;
using IQueue;
using Messaging.Json;
using Queue.PassThrough;
using System.Threading;
using Unity;

namespace ClientUI {
	class Program {
		static ManualResetEvent _quit = new ManualResetEvent(false);
		static void Main(string[] args) {
			//Configure the App
			new Thread(() => {
				QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, PassThrough>();
				ConnectionFactory.UnityContainer.RegisterType<IClient, WebsocketClient>();
				ConnectionFactory.UnityContainer.RegisterType<IServer, WebsocketServer>();
				MessagingFactory.UnityContainer.RegisterType<IMessageCreator, Messaging.Json.MessageCreator>();
				MessagingFactory.UnityContainer.RegisterType<IMessageProvider, Messaging.App.Client.ClientMessageProvider>();

				Controller.Start();
			}) { IsBackground = true }.Start();

			System.Console.CancelKeyPress += Console_CancelKeyPress;
			_quit.WaitOne();
	}

		private static void Console_CancelKeyPress(object sender, System.ConsoleCancelEventArgs e) {
			_quit.Set();
		}
	}
}
