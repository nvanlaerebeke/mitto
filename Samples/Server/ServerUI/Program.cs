using System;
using System.Net;
using System.Threading;
using log4net.Appender;
using log4net.Config;
using ServerManager;
using Unity;

namespace ConnectionServer {
	class Program {
		static ManualResetEvent _objClose = new ManualResetEvent(false);
		static Server _objServer;
		static void Main(string[] args) {
			IConnection.ConnectionFactory.UnityContainer.RegisterType<IConnection.IServer, Connection.Websocket.Server.WebsocketServer>();
			IQueue.QueueFactory.UnityContainer.RegisterType<IQueue.IQueue, Queue.PassThrough.PassThrough>();
			IMessaging.MessagingFactory.UnityContainer.RegisterType<IMessaging.IMessageCreator, Messaging.Json.MessageCreator>();
			IMessaging.MessagingFactory.UnityContainer.RegisterType<IMessaging.IMessageProvider, Messaging.App.Server.ServerMessageProvider>();

			ConfigureLogger();

			_objServer = new Server();
			_objServer.Start(IPAddress.Any, 80);

			Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e) {
				_objClose.Set();
			};
			_objClose.WaitOne();
		}

		static void ConfigureLogger() {
			BasicConfigurator.Configure(new ConsoleAppender() {
				Threshold = log4net.Core.Level.Info,
				Name = "ConsoleAppender",
				Layout = new log4net.Layout.SimpleLayout()
			});
		}
	}
}