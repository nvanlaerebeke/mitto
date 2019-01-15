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
			Mitto.IConnection.ConnectionFactory.UnityContainer.RegisterType<Mitto.IConnection.IServer, Mitto.Connection.Websocket.Server.WebsocketServer>();
			Mitto.IQueue.QueueFactory.UnityContainer.RegisterType<Mitto.IQueue.IQueue, Mitto.Queue.PassThrough.PassThrough>();
			Mitto.IMessaging.MessagingFactory.UnityContainer.RegisterType<Mitto.IMessaging.IMessageCreator, Mitto.Messaging.Json.MessageCreator>();
			Mitto.IMessaging.MessagingFactory.UnityContainer.RegisterType<Mitto.IMessaging.IMessageProvider, Messaging.App.Server.ServerMessageProvider>();

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